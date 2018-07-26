using System;
using System.Linq;
using IllusionPlugin;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PracticePlugin
{
	public class Plugin : IPlugin
	{
		public const float MaxSize = 5f;
		public const float StepSize = 0.05f;
		
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

		public static bool NoFail { get; private set; }
		
		private bool _init;
		private MainGameSceneSetupData _mainGameSceneSetupData;
		private AudioTimeSyncController _audioTimeSync;
		private static AudioSource _songAudio;
		private static AudioSource _noteCutAudioSource;
		private string _lastLevelId;
		
		public string Name
		{
			get { return "Practice Plugin"; }
		}

		public string Version
		{
			get { return "v2.0"; }
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
					ReflectionUtil.CopyComponent(volume, typeof(IncDecSettingsController), typeof(SpeedSettingsController), SettingsObject);
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
				
				if (_mainGameSceneSetupData == null || scene.buildIndex != 5)
				{
					return;
				}

				if (_lastLevelId != _mainGameSceneSetupData.difficultyLevel.level.levelID && !string.IsNullOrEmpty(_lastLevelId))
				{
					TimeScale = 1;
					_lastLevelId = _mainGameSceneSetupData.difficultyLevel.level.levelID;
				}

				_lastLevelId = _mainGameSceneSetupData.difficultyLevel.level.levelID;
				
				_audioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
				_audioTimeSync.forcedAudioSync = true;
				_songAudio = ReflectionUtil.GetPrivateField<AudioSource>(_audioTimeSync, "_audioSource");
				NoFail = !_mainGameSceneSetupData.gameplayOptions.validForScoreUse;
				
				if (!NoFail)
				{
					TimeScale = Mathf.Clamp(TimeScale, 1, MaxSize);
				}
				
				NoteHitPitchChanger.ReplacePrefab();

				var canvas = Resources.FindObjectsOfTypeAll<HorizontalLayoutGroup>().FirstOrDefault(x => x.name == "Buttons")
					.transform.parent;
				canvas.gameObject.AddComponent<SpeedSettingsCreator>();
				TimeScale = TimeScale;
			}
		}

		private void ScoreControllerOnNoteWasCutEvent(NoteData arg1, NoteCutInfo arg2, int arg3)
		{
			throw new NotImplementedException();
		}

		public void OnLevelWasLoaded(int level)
		{
			
		}

		public void OnLevelWasInitialized(int level)
		{
			
		}

		public void OnUpdate()
		{
			
		}

		public void OnFixedUpdate()
		{
			
		}
	}
}