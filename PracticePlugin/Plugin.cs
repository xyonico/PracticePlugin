using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HMUI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;
//using CustomUI.GameplaySettings;
using IPA;
using BS_Utils.Gameplay;
using BeatSaberMarkupLanguage;
using ModestTree;
using BeatSaberMarkupLanguage.ViewControllers;
using UnityEngine.UIElements;
using BS_Utils.Utilities;

namespace PracticePlugin
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {

        public const float SpeedMaxSize = 5.05f;
        public const float SpeedStepSize = 0.05f;

        public const int NjsMaxSize = 100;
        public const int NjstepSize = 1;

        public const string MenuSceneName = "MenuViewControllers";
        public const string GameSceneName = "GameCore";
        public const string ContextSceneName = "GameplayCore";

        public string failTime { get; private set; }
        internal bool showFailTextNext { get; set; }
 //       public static GameObject SpeedSettingsObject { get; private set; }
 //       public static GameObject NjsSettingsObject { get; private set; }
 //       public static GameObject SpawnOffsetSettingsObject { get; private set; }
        internal static bool startWithFullEnergy = false;
        internal static bool disablePitchCorrection = false;
        internal static bool adjustNJSWithSpeed = false;
        internal static float TimeScale
        {
            get { return _timeScale; }
            set
            {
                _timeScale = value;
                if (!AudioTimeSync) return;
                if (!_spawnController) return;
                AudioTimeSyncController.InitData initData = AudioTimeSync.GetPrivateField<AudioTimeSyncController.InitData>("_initData");
                AudioTimeSyncController.InitData newInitData = new AudioTimeSyncController.InitData(initData.audioClip,
                    AudioTimeSync.songTime, initData.songTimeOffset, _timeScale);
                AudioTimeSync.SetPrivateField("_initData", newInitData);
                //Chipmunk Removal as per base game
                if(!disablePitchCorrection)
                {
                    if (_timeScale == 1f)
                        _mixer.musicPitch = 1;
                    else
                        _mixer.musicPitch = 1f / _timeScale;
                }
                else
                {
                    _mixer.musicPitch = 1f;
                }
                ResetTimeSync(AudioTimeSync, _timeScale, newInitData);
             //   AudioTimeSync.StartSong();


                return;
                //       AudioTimeSync.SetPrivateField("_timeScale", value);
                //        AudioTimeSync.Init(_songAudio.clip, _songAudio.time, AudioTimeSync.GetPrivateField<float>("_songTimeOffset"), value);
                if (_timeScale == 1f)
                    _mixer.musicPitch = 1;
                else
                    _mixer.musicPitch = 1f / _timeScale;
                if (!IsEqualToOne(_timeScale))
                {

                    if (AudioTimeSync != null)
                    {
                        //           AudioTimeSync.forcedNoAudioSync = true;
                    }
                }
                else
                {
                    if (AudioTimeSync != null)
                    {
                        //           AudioTimeSync.forcedNoAudioSync = false;
                    }
                }
                if (AudioTimeSync != null)
                {
               //     AudioTimeSync.SetPrivateField("_timeScale", _timeScale); // = _timeScale;
                                                                             //     AudioTimeSync.Init(_songAudio.clip, _songAudio.time, 
                                                                             //           AudioTimeSync.GetPrivateField<float>("_songTimeOffset") - AudioTimeSync.GetPrivateField<FloatSO>("_audioLatency").value, _timeScale);
                    Console.WriteLine("Called TimeScale");

                    if (_songAudio != null)
                    {
                        _songAudio.pitch = _timeScale;
                    }
           //         AudioTimeSync.forcedNoAudioSync = true;
                    //         float num = AudioTimeSync.GetPrivateField<float>("_startSongTime") + AudioTimeSync.GetPrivateField<float>("_songTimeOffset");
                    //     AudioTimeSync.SetPrivateField("_audioStartTimeOffsetSinceStart", (Time.timeSinceLevelLoad * _timeScale) - num);
                    //   AudioTimeSync.SetPrivateField("_fixingAudioSyncError", false);
                    //   AudioTimeSync.SetPrivateField("_prevAudioSamplePos", _songAudio.timeSamples);
                    //   AudioTimeSync.SetPrivateField("_playbackLoopIndex", 0);
                    //          AudioTimeSync.SetPrivateField("_dspTimeOffset", AudioSettings.dspTime - (double)num);
                    //    AudioTimeSync.SetPrivateField("_timeScale", _timeScale); // = _timeScale;
                }


            }
        }
        public static void ResetTimeSync(AudioTimeSyncController timeSync, float newTimeScale, AudioTimeSyncController.InitData newData)
        {

            timeSync.SetPrivateField("_timeScale", newTimeScale);
            timeSync.SetPrivateField("_startSongTime", timeSync.songTime);
            timeSync.SetPrivateField("_audioStartTimeOffsetSinceStart",
                timeSync.GetProperty<float>("timeSinceStart") - (timeSync.songTime + newData.songTimeOffset));
            timeSync.SetPrivateField("_fixingAudioSyncError", false);
            timeSync.SetPrivateField("_playbackLoopIndex", 0);
            timeSync.audioSource.pitch = newTimeScale;
        }

        private static float _timeScale = 1;

        public static bool PracticeMode { get; private set; }

        public static bool PlayingNewSong { get; private set; }

        private static bool _init;
        public static BS_Utils.Gameplay.LevelData _levelData { get; private set; }
        public static BS_Utils.Utilities.Config Config = new BS_Utils.Utilities.Config("PracticePlugin");
        public static BeatmapObjectSpawnController _spawnController { get; private set; }
        public static AudioTimeSyncController AudioTimeSync { get; private set; }
        private static AudioManagerSO _mixer;
        private static AudioSource _songAudio;
        private static string _lastLevelId;
        public static UIElementsCreator _uiElementsCreator;
        private static ResultsViewController resultsViewController;
        private static bool _resetNoFail;
        private static bool showTimeFailed = true;
        private static TextMeshProUGUI failText;
        [OnStart]
        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;

            startWithFullEnergy = Config.GetBool("PracticePlugin", "startWithFullEnergy", false, true);
            showTimeFailed = Config.GetBool("PracticePlugin", "Show Time Failed", true, true);
            disablePitchCorrection = Config.GetBool("PracticePlugin", "Disable Pitch Correction", false, true);
            adjustNJSWithSpeed = Config.GetBool("PracticePlugin", "Adjust NJS With Speed", false, true);
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
            
        }

        public void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "MenuCore")
            {

            }



        }

        public void OnApplicationQuit()
        {

        }

        public void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            Object.Destroy(Resources.FindObjectsOfTypeAll<UIElementsCreator>().FirstOrDefault()?.gameObject);
            if (newScene.name == MenuSceneName)
            {
                resultsViewController =
                Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();
                if (resultsViewController != null)
                {

                    resultsViewController.didActivateEvent -= ResultsViewController_didActivateEvent;
                    resultsViewController.didActivateEvent += ResultsViewController_didActivateEvent;
                }

            }
            else if (newScene.name == GameSceneName)
            {


                if (_levelData == null)
                {
                    _levelData = BS_Utils.Plugin.LevelData;
                    if (_levelData == null) return;
                    BS_Utils.Plugin.LevelDidFinishEvent += MainGameSceneSetupDataOnDidFinishEvent;
                }


                if (_lastLevelId != _levelData.GameplayCoreSceneSetupData.difficultyBeatmap.level.levelID &&
                    !string.IsNullOrEmpty(_lastLevelId))
                {
                    PlayingNewSong = true;
                   // TimeScale = 1;
                   _lastLevelId = _levelData.GameplayCoreSceneSetupData.difficultyBeatmap.level.levelID;
                }
                else
                {
                    PlayingNewSong = false;
                }

                
                _lastLevelId = _levelData.GameplayCoreSceneSetupData.difficultyBeatmap.level.levelID;
                _mixer = Resources.FindObjectsOfTypeAll<AudioManagerSO>().LastOrDefault();
                AudioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().LastOrDefault();
                _songAudio = AudioTimeSync.GetPrivateField<AudioSource>("_audioSource");

                PracticeMode = (_levelData.Mode == BS_Utils.Gameplay.Mode.Standard && _levelData.GameplayCoreSceneSetupData.practiceSettings != null && !BS_Utils.Gameplay.Gamemode.IsIsolatedLevel);


                if (!PracticeMode)
                {
                    _timeScale = Mathf.Clamp(TimeScale, 1, SpeedMaxSize);
                }
                if (PracticeMode)
                {
                    if (_levelData.GameplayCoreSceneSetupData.practiceSettings.songSpeedMul != 1f)
                        _timeScale = _levelData.GameplayCoreSceneSetupData.practiceSettings.songSpeedMul;
                    else
                        _timeScale = _levelData.GameplayCoreSceneSetupData.gameplayModifiers.songSpeedMul;
                    SharedCoroutineStarter.instance.StartCoroutine(DelayedSetup());
                }
                
            }
        }


        private void ResultsViewController_didActivateEvent(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (showFailTextNext && showTimeFailed)
            {
                if (failText == null)
                    failText = BeatSaberMarkupLanguage.BeatSaberUI.CreateText(resultsViewController.rectTransform, failTime, new Vector2(15f, -35f));
                else
                   failText.text = failTime;
                failText.richText = true;
            }
            else
            {
                if (failText != null)
                    failText.text = "";
            }
            showFailTextNext = false;
        }

        public System.Collections.IEnumerator DelayedSetup()
        {
            yield return new WaitForSeconds(0.1f);
            try
            {
                if (_spawnController == null)
                {
                    _spawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().LastOrDefault();

                }
                Console.WriteLine("Atemmpting Practice Plugin UI");

                var canvas = GameObject.Find("PauseMenu").transform.Find("Wrapper").transform.Find("MenuWrapper").transform.Find("Canvas");

                if (canvas == null)
                {
                    Console.WriteLine("Canvas Null");
                }

                BSMLParser.instance.Parse(BeatSaberMarkupLanguage.Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "PracticePlugin.PracticeUI.bsml"), canvas.gameObject, PracticeUI.instance);
                GameObject uiObj = new GameObject("PracticePlugin Seeker UI", typeof(RectTransform));
                
                (uiObj.transform as RectTransform).anchorMin = new Vector2(0, 0);
                (uiObj.transform as RectTransform).anchorMax = new Vector2(1, 1);
                (uiObj.transform as RectTransform).sizeDelta = new Vector2(0, 0);

                _uiElementsCreator = uiObj.AddComponent<UIElementsCreator>();
                _uiElementsCreator.Init();
                
                uiObj.transform.SetParent(canvas, false);

                uiObj.transform.localScale = new Vector3(1, 1, 1);
                uiObj.transform.localPosition = new Vector3(0f, -3f, 0f);
                
                 new GameObject("Practice Plugin Behavior").AddComponent<Behavior>();
                if (startWithFullEnergy)
                {
                    GameEnergyCounter energyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
                    if (energyCounter != null)
                        energyCounter.AddEnergy(1 - energyCounter.energy);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private void MainGameSceneSetupDataOnDidFinishEvent(StandardLevelScenesTransitionSetupDataSO levelData, LevelCompletionResults results)
        {
            if (results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Failed)
            {
                float endTime = results.endSongTime;
                float length = _songAudio.clip.length;
                failTime = $"<#ff0000>Failed At</color> - {Math.Floor(endTime / 60):N0}:{Math.Floor(endTime % 60):00}  /  {Math.Floor(length / 60):N0}:{Math.Floor(length % 60):00}";
                showFailTextNext = true;

            }

        }


        private static bool IsEqualToOne(float value)
        {
            return Math.Abs(value - 1) < 0.000000001f;
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {

        }


        public void OnFixedUpdate()
        {

        }

        public static void AdjustNJS(float njs)
        {

            float halfJumpDur = 4f;
            float maxHalfJump = _spawnController.GetPrivateField<float>("_maxHalfJumpDistance");
            float noteJumpStartBeatOffset = _levelData.GameplayCoreSceneSetupData.difficultyBeatmap.noteJumpStartBeatOffset;
            float moveSpeed = _spawnController.GetPrivateField<float>("_moveSpeed");
            float moveDir = _spawnController.GetPrivateField<float>("_moveDurationInBeats");
            float jumpDis;
            float spawnAheadTime;
            float moveDis;
            float bpm = _spawnController.GetPrivateField<float>("_beatsPerMinute");
            float num = 60f / bpm;
            moveDis = moveSpeed * num * moveDir;
            while (njs * num * halfJumpDur > maxHalfJump)
            {
                halfJumpDur /= 2f;
            }
            halfJumpDur += noteJumpStartBeatOffset;
            if (halfJumpDur < 1f) halfJumpDur = 1f;
            //        halfJumpDur = spawnController.GetPrivateField<float>("_halfJumpDurationInBeats");
            jumpDis = njs * num * halfJumpDur * 2f;
            spawnAheadTime = moveDis / moveSpeed + jumpDis * 0.5f / njs;
            _spawnController.SetPrivateField("_halfJumpDurationInBeats", halfJumpDur);
            _spawnController.SetPrivateField("_spawnAheadTime", spawnAheadTime);
            _spawnController.SetPrivateField("_jumpDistance", jumpDis);
            _spawnController.SetPrivateField("_noteJumpMovementSpeed", njs);
            _spawnController.SetPrivateField("_moveDistance", moveDis);


        }
        public static void AdjustSpawnOffset(float offset)
        {
            float njs = _spawnController.GetPrivateField<BeatmapObjectSpawnController.InitData>("_initData").noteJumpMovementSpeed;
            float halfJumpDur = 4f;
            float maxHalfJump = _spawnController.GetPrivateField<float>("_maxHalfJumpDistance");
            float noteJumpStartBeatOffset = offset;
            float moveSpeed = _spawnController.GetPrivateField<float>("_moveSpeed");
            float moveDir = _spawnController.GetPrivateField<float>("_moveDurationInBeats");
            float jumpDis;
            float spawnAheadTime;
            float moveDis;
            float bpm = _spawnController.GetPrivateField<float>("_beatsPerMinute");
            float num = 60f / bpm;
            moveDis = moveSpeed * num * moveDir;
            while (njs * num * halfJumpDur > maxHalfJump)
            {
                halfJumpDur /= 2f;
            }
            halfJumpDur += noteJumpStartBeatOffset;
            if (halfJumpDur < 1f) halfJumpDur = 1f;
            //        halfJumpDur = spawnController.GetPrivateField<float>("_halfJumpDurationInBeats");
            jumpDis = njs * num * halfJumpDur * 2f;
            spawnAheadTime = moveDis / moveSpeed + jumpDis * 0.5f / njs;
            _spawnController.SetPrivateField("_halfJumpDurationInBeats", halfJumpDur);
            _spawnController.SetPrivateField("_spawnAheadTime", spawnAheadTime);
            _spawnController.SetPrivateField("_jumpDistance", jumpDis);
            _spawnController.SetPrivateField("_noteJumpMovementSpeed", njs);
            _spawnController.SetPrivateField("_moveDistance", moveDis);


        }

        public static void UpdateSpawnMovementData(float njs, float noteJumpStartBeatOffset)
        {
            BeatmapObjectSpawnMovementData spawnMovementData =
    _spawnController.GetPrivateField<BeatmapObjectSpawnMovementData>("_beatmapObjectSpawnMovementData");

            float bpm = _spawnController.GetPrivateField<VariableBpmProcessor>("_variableBpmProcessor").currentBpm;

            
            if (adjustNJSWithSpeed)
            {
                float newNJS = njs * (1 / TimeScale);
                njs = newNJS; 
            }



            spawnMovementData.SetPrivateField("_startNoteJumpMovementSpeed", njs);
            spawnMovementData.SetPrivateField("_noteJumpStartBeatOffset", noteJumpStartBeatOffset);

            spawnMovementData.Update(bpm,
                _spawnController.GetPrivateField<float>("_jumpOffsetY"));


        }
        public void OnSceneUnloaded(Scene scene)
        {
        }

        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition)
        {
            return CreateText(parent, text, anchoredPosition, new Vector2(60f, 10f));
        }

        public static TextMeshProUGUI CreateText(RectTransform parent, string text, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            GameObject gameObj = new GameObject("CustomUIText");
            gameObj.SetActive(false);

            TextMeshProUGUI textMesh = gameObj.AddComponent<TextMeshProUGUI>();
            textMesh.font = UnityEngine.Object.Instantiate(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().First(t => t.name == "Teko-Medium SDF No Glow"));
            textMesh.rectTransform.SetParent(parent, false);
            textMesh.text = text;
            textMesh.fontSize = 4;
            textMesh.color = Color.white;

            textMesh.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            textMesh.rectTransform.sizeDelta = sizeDelta;
            textMesh.rectTransform.anchoredPosition = anchoredPosition;

            gameObj.SetActive(true);
            return textMesh;
        }
    }
}
