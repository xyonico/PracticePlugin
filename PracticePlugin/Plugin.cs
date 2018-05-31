using System;
using System.IO;
using System.Linq;
using IllusionPlugin;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PracticePlugin
{
	public class Plugin : IPlugin
	{
		public static GameObject SettingsObject;

		public static float TimeScale
		{
			get { return _timeScale; }
			set
			{
				_timeScale = value;
				if (_songAudio != null)
				{
					_songAudio.pitch = _timeScale;
				}

				if (_noteCutAudioSource != null)
				{
					_noteCutAudioSource.pitch = _timeScale;
				}
			}
		}
		private static float _timeScale = 1;

		public static bool Enabled
		{
			get { return _enabled; }
			set
			{
				_enabled = value;
				if (!_enabled)
				{
					TimeScale = 1;
				}
			}
		}

		private static bool _enabled;
		
		private bool _init;
		private MainGameSceneSetupData _mainGameSceneSetupData;
		private AudioTimeSyncController _audioTimeSync;
		private static AudioSource _songAudio;
		private static AudioSource _noteCutAudioSource;
		private string _lastLevelId;

        private GameSongController gameSongController;
        private SongObjectSpawnController songObjectSpawnController;
        private ScoreController scoreController;
        private static int rewindAmountBeats = 4;
        private static int tmpBeat = 0;
        private static int sampleRate          = 44100;
        private Dictionary<int, ScoreListItem> scoresDict;
        private static int previousBeat = -1;
        private static bool applyScoreAgain = false; // Hack to ensure score is applied on rewind

        public string Name
		{
			get { return "Practice Plugin"; }
		}

		public string Version
		{
			get { return "v1.2"; }
		}
		
		public void OnApplicationStart()
		{
			if (_init) return;
			_init = true;
			SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
		}

		public void OnApplicationQuit()
		{
			SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
		}

		private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
		{
			if (scene.buildIndex == 1)
			{
				if (SettingsObject == null)
				{
					var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
					volumeSettings.gameObject.SetActive(false);
					SettingsObject = Object.Instantiate(volumeSettings.gameObject);
					SettingsObject.SetActive(false);
					volumeSettings.gameObject.SetActive(true);
					var volume = SettingsObject.GetComponent<VolumeSettingsController>();
					ReflectionUtil.CopyComponent(volume, typeof(SimpleSettingsController), typeof(SpeedSettingsController), SettingsObject);
					Object.DestroyImmediate(volume);
					SettingsObject.GetComponentInChildren<TMP_Text>().text = "SPEED";
					Object.DontDestroyOnLoad(SettingsObject);
				}
            }
			else
			{
				if (_mainGameSceneSetupData == null)
				{
					_mainGameSceneSetupData = Resources.FindObjectsOfTypeAll<MainGameSceneSetupData>().FirstOrDefault();
				}
				
				if (_mainGameSceneSetupData == null || scene.buildIndex != 4)
				{
					return;
				}

				if (_lastLevelId != _mainGameSceneSetupData.levelId && !string.IsNullOrEmpty(_lastLevelId))
				{
					TimeScale = 1;
					_lastLevelId = _mainGameSceneSetupData.levelId;
				}

				_lastLevelId = _mainGameSceneSetupData.levelId;
				
				_audioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
				_audioTimeSync.forcedAudioSync = true;
				_songAudio = ReflectionUtil.GetPrivateField<AudioSource>(_audioTimeSync, "_audioSource");
				Enabled = _mainGameSceneSetupData.gameplayOptions.noEnergy;
				var noteCutSoundEffectManager = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>().FirstOrDefault();
				var noteCutSoundEffect =
					ReflectionUtil.GetPrivateField<NoteCutSoundEffect>(noteCutSoundEffectManager, "_noteCutSoundEffectPrefab");
				_noteCutAudioSource =
					ReflectionUtil.GetPrivateField<AudioSource>(noteCutSoundEffect, "_audioSource");

				var canvas = Resources.FindObjectsOfTypeAll<HorizontalLayoutGroup>().FirstOrDefault(x => x.name == "Buttons")
					.transform.parent;
				canvas.gameObject.AddComponent<SpeedSettingsCreator>();
				TimeScale = TimeScale;

                gameSongController = Resources.FindObjectsOfTypeAll<GameSongController>().FirstOrDefault();
                songObjectSpawnController = Resources.FindObjectsOfTypeAll<SongObjectSpawnController>().FirstOrDefault();
                scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault<ScoreController>();

                scoresDict = new Dictionary<int, ScoreListItem>();
            }
		}

		public void OnLevelWasLoaded(int level)
		{
			
		}

		public void OnLevelWasInitialized(int level)
		{

        }

        public void OnUpdate()
		{
            if (Enabled)
            {
                // songTime is in seconds
                int currentBeat = (int)((_audioTimeSync.songTime / 60f) * gameSongController.beatsPerMinute);

                //Log("currentBeat:" + currentBeat + " songTime" + _audioTimeSync.songTime);
                if (currentBeat > previousBeat)
                {
                    previousBeat = currentBeat;
                    Log("currentBeat:" + currentBeat + " songTime:" + _audioTimeSync.songTime);
                    
                    ScoreListItem scoreListItem = new ScoreListItem
                    {
                        baseScore = ReflectionUtil.GetPrivateField<int>(scoreController, "_baseScore"),
                        multiplier = ReflectionUtil.GetPrivateField<int>(scoreController, "_multiplier"),
                        multiplierIncreaseProgress = ReflectionUtil.GetPrivateField<int>(scoreController, "_multiplierIncreaseProgress"),
                        multiplierIncreaseMaxProgress = ReflectionUtil.GetPrivateField<int>(scoreController, "_multiplierIncreaseMaxProgress"),
                        combo = ReflectionUtil.GetPrivateField<int>(scoreController, "_combo"),
                        maxCombo = ReflectionUtil.GetPrivateField<int>(scoreController, "_maxCombo"),
                        playerHeadWasInObstacle = ReflectionUtil.GetPrivateField<bool>(scoreController, "_playerHeadWasInObstacle"),
                        afterCutScoreBuffers = ReflectionUtil.GetPrivateField<List<AfterCutScoreBuffer>>(scoreController, "_afterCutScoreBuffers")
                    };

                    // Add the data to the dictionary
                    scoresDict[currentBeat] = scoreListItem;
                }

                // Hack to ensure score is applied on rewind
                // prevFrameScore is applied on LateUpdate and the score will sometimes not be applied when rewinding.
                // This just applies the score again on the next frame to make sure that it changes.
                // May not be required
                if (applyScoreAgain)
                {
                    RewindScore(currentBeat);
                    applyScoreAgain = false;
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    // Trigger rewind here.
                    tmpBeat = currentBeat - rewindAmountBeats;
                    Rewind(tmpBeat < 0 ? 0 : tmpBeat);
                    applyScoreAgain = true;
                }
            }
		}

		public void OnFixedUpdate()
		{
			
		}

        private void Rewind(int destinationBeat)
        {
            previousBeat = destinationBeat;

            var destinationTime = destinationBeat * 60f / gameSongController.beatsPerMinute; // Convert beats to time in sec
            Log("Rewinding to beat:" + destinationBeat + " at time:" + destinationTime);

            DeleteExistingNotes();
            
            WalkBackIndexes(destinationTime);

            _songAudio.timeSamples = (int)(destinationTime * sampleRate);                                  // Rewind the audio file
            ReflectionUtil.SetPrivateField(_audioTimeSync, "_prevAudioSamplePos", _songAudio.timeSamples); // This must be set or song will end instantly

            RewindScore(destinationBeat);
        }

        private void RewindScore(int destinationBeat)
        {
            ScoreListItem scoreListItem;
            
            // Loop through and log the data for all scores saved in the scoresDict
            String message = "\n";
            foreach (int key in scoresDict.Keys)
            {
                scoreListItem = scoresDict[key];
                message = message + "key: " + key 
                    + " scoreListItem baseScore:" + scoreListItem.baseScore
                    + " previousScore:" + scoreListItem.previousScore
                    + " multiplier:" + scoreListItem.multiplier
                    + " multiplierIncreaseProgress:" + scoreListItem.multiplierIncreaseProgress
                    + " multiplierIncreaseMaxProgress:" + scoreListItem.multiplierIncreaseMaxProgress
                    + " combo:" + scoreListItem.combo
                    + " maxCombo:" + scoreListItem.maxCombo
                    + " playerHeadWasInObstacle:" + scoreListItem.playerHeadWasInObstacle
                    + "\n";
            }

            Log(message);

            // Get the score at the destination beat
            scoreListItem = scoresDict[destinationBeat];
            
            Log("destinationBeat: " + destinationBeat
                +  " scoreListItem baseScore:" + scoreListItem.baseScore
                + " previousScore:" + scoreListItem.previousScore
                + " multiplier:" + scoreListItem.multiplier
                + " multiplierIncreaseProgress:" + scoreListItem.multiplierIncreaseProgress
                + " multiplierIncreaseMaxProgress:" + scoreListItem.multiplierIncreaseMaxProgress
                + " combo:" + scoreListItem.combo
                + " maxCombo:" + scoreListItem.maxCombo
                + " playerHeadWasInObstacle:" + scoreListItem.playerHeadWasInObstacle
                + "\n");

            // Get the score data at the destination beat
            ReflectionUtil.SetPrivateField(scoreController, "_baseScore", scoreListItem.baseScore);
            ReflectionUtil.SetPrivateField(scoreController, "_prevFrameScore", scoreListItem.previousScore); // <-------- Trying to force scoreDidChangeEvent
            ReflectionUtil.SetPrivateField(scoreController, "_multiplier", scoreListItem.multiplier);
            ReflectionUtil.SetPrivateField(scoreController, "_multiplierIncreaseProgress", scoreListItem.multiplierIncreaseProgress);
            ReflectionUtil.SetPrivateField(scoreController, "_multiplierIncreaseMaxProgress", scoreListItem.multiplierIncreaseMaxProgress);
            ReflectionUtil.SetPrivateField(scoreController, "_combo", scoreListItem.combo);
            ReflectionUtil.SetPrivateField(scoreController, "_maxCombo", scoreListItem.maxCombo);
            ReflectionUtil.SetPrivateField(scoreController, "_playerHeadWasInObstacle", scoreListItem.playerHeadWasInObstacle);
            ReflectionUtil.SetPrivateField(scoreController, "_afterCutScoreBuffers", scoreListItem.afterCutScoreBuffers);

            // Trigger the scoreController's late update to unsure a scoreDidChangeEvent
            scoreController.LateUpdate();
        }
        
        // Change the indexes for the next objects to hit, to account for the rewind
        private void WalkBackIndexes(float destinationTime)
        {
            // Move indexes back for all object arrays
            var _songObjectCallbackData = ReflectionUtil.GetPrivateField<List<SongController.SongObjectCallbackData>>(gameSongController, "_songObjectCallbackData");
            var _songData = ReflectionUtil.GetPrivateField<SongData>(gameSongController, "_songData");
            var _nextEventIndex = ReflectionUtil.GetPrivateField<int>(gameSongController, "_nextEventIndex");

            float songTime = _audioTimeSync.songTime;
            for (int i = 0; i < _songObjectCallbackData.Count; i++)
            {
                SongController.SongObjectCallbackData songObjectCallbackData = _songObjectCallbackData[i];
                for (int j = 0; j < gameSongController.noteLinesCount; j++)
                {
                    // Loop backwards from the current "nextObjectIndexInLine"
                    // Break when we hit an object that is less than the current time, thus storing the next object that should be hit
                    while (songObjectCallbackData.nextObjectIndexInLine[j] > 0)
                    {
                        SongObjectData songObjectData = _songData.SongLinesData[j].songObjectsData[songObjectCallbackData.nextObjectIndexInLine[j]];
                        if (songObjectData.time < destinationTime)
                        {
                            break;
                        }
                        songObjectCallbackData.nextObjectIndexInLine[j]--;
                    }
                }
            }

            // Loop backwards from the current nextEventIndex.
            // Break when we hit an index that is less than the current time, thus storing the next event that should be played
            while (_nextEventIndex > 0)
            {
                SongEventData songEventData = _songData.SongEventData[_nextEventIndex];
                if (songEventData.time < destinationTime)
                {
                    break;
                }
                _nextEventIndex--;
            }

        }

        private void DeleteExistingNotes()
        {
            var _gameNotePrefab = ReflectionUtil.GetPrivateField<NoteController>(songObjectSpawnController, "_gameNotePrefab");
            var _bombNotePrefab = ReflectionUtil.GetPrivateField<BombNoteController>(songObjectSpawnController, "_bombNotePrefab");
            var _obstacleBottomPrefab = ReflectionUtil.GetPrivateField<ObstacleController>(songObjectSpawnController, "_obstacleBottomPrefab");
            var _obstacleTopPrefab = ReflectionUtil.GetPrivateField<ObstacleController>(songObjectSpawnController, "_obstacleTopPrefab");

            // Same duration used in game
            float duration = 1.4f;
            List<NoteController> cubeNoteControllers = _gameNotePrefab.GetSpawned<NoteController>();
            foreach (NoteController noteController in cubeNoteControllers)
            {
                noteController.Dissolve(duration);
            }
            List<BombNoteController> bombNoteControllers = _bombNotePrefab.GetSpawned<BombNoteController>();
            foreach (BombNoteController bombNoteController in bombNoteControllers)
            {
                bombNoteController.Dissolve(duration);
            }
            List<ObstacleController> obstacleBottomControllers = _obstacleBottomPrefab.GetSpawned<ObstacleController>();
            foreach (ObstacleController obstacleController in obstacleBottomControllers)
            {
                obstacleController.Dissolve(duration);
            }
            List<ObstacleController> obstacleTopControllers = _obstacleTopPrefab.GetSpawned<ObstacleController>();
            foreach (ObstacleController obstacleController2 in obstacleTopControllers)
            {
                obstacleController2.Dissolve(duration);
            }
        }
        
        public static void Log(string message)
        {
            Debug.Log("PracticePlugin: " + message);
            Console.WriteLine("PracticePlugin: " + message);
        }
    }
}