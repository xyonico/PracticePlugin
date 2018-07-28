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
		public string Name
		{
			get { return "Practice Plugin"; }
		}

		public string Version
		{
			get { return "v2.2"; }
		}

		public const float MaxSize = 5f;
		public const float StepSize = 0.05f;

		public static GameObject SettingsObject;

		public static float TimeScale
		{
			get { return _timeScale; }
			set
			{
				_timeScale = value;
				if (!IsEqualToOne(_timeScale))
				{
					HasTimeScaleChanged = true;

					if (AudioTimeSync != null)
					{
						AudioTimeSync.forcedAudioSync = true;
					}
				}
				else
				{
					if (AudioTimeSync != null)
					{
						AudioTimeSync.forcedAudioSync = false;
					}
				}

				if (_songAudio != null)
				{
					_songAudio.pitch = _timeScale;
				}
			}
		}

		private static float _timeScale = 1;

		public static bool NoFail { get; private set; }

		public static bool HasTimeScaleChanged { get; private set; }

		private static bool _init;
		private static MainGameSceneSetupData _mainGameSceneSetupData;
		public static AudioTimeSyncController AudioTimeSync { get; private set; }
		private static AudioSource _songAudio;
		private static string _lastLevelId;
		private static UIElementsCreator _uiElementsCreator;
		private static bool _resetNoFail;

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
				if (_resetNoFail)
				{
					var resultsViewController =
						Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();
					resultsViewController.continueButtonPressedEvent += ResultsViewControllerOnContinueButtonPressedEvent;
				}
				
				if (SettingsObject == null)
				{
					var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
					volumeSettings.gameObject.SetActive(false);
					SettingsObject = Object.Instantiate(volumeSettings.gameObject);
					SettingsObject.SetActive(false);
					volumeSettings.gameObject.SetActive(true);
					var volume = SettingsObject.GetComponent<VolumeSettingsController>();
					ReflectionUtil.CopyComponent(volume, typeof(IncDecSettingsController),
						typeof(SpeedSettingsController), SettingsObject);
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
					if (_mainGameSceneSetupData == null) return;
					_mainGameSceneSetupData.didFinishEvent += MainGameSceneSetupDataOnDidFinishEvent;
				}

				if (scene.buildIndex != 5)
				{
					return;
				}

				if (_lastLevelId != _mainGameSceneSetupData.difficultyLevel.level.levelID &&
				    !string.IsNullOrEmpty(_lastLevelId))
				{
					HasTimeScaleChanged = false;
					TimeScale = 1;
					_lastLevelId = _mainGameSceneSetupData.difficultyLevel.level.levelID;
				}

				if (IsEqualToOne(TimeScale))
				{
					HasTimeScaleChanged = false;
				}

				_lastLevelId = _mainGameSceneSetupData.difficultyLevel.level.levelID;

				AudioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
				_songAudio = AudioTimeSync.GetPrivateField<AudioSource>("_audioSource");
				NoFail = !_mainGameSceneSetupData.gameplayOptions.validForScoreUse;

				if (!NoFail)
				{
					TimeScale = Mathf.Clamp(TimeScale, 1, MaxSize);
				}

				NoteHitPitchChanger.ReplacePrefab();

				var canvas = Resources.FindObjectsOfTypeAll<HorizontalLayoutGroup>()
					.FirstOrDefault(x => x.name == "Buttons")
					.transform.parent;
				_uiElementsCreator = canvas.gameObject.AddComponent<UIElementsCreator>();
				_uiElementsCreator.ValueChangedEvent += UIElementsCreatorOnValueChangedEvent;
				TimeScale = TimeScale;
			}
		}

		private void ResultsViewControllerOnContinueButtonPressedEvent(ResultsViewController obj)
		{
			PersistentSingleton<GameDataModel>.instance.gameDynamicData.GetCurrentPlayerDynamicData()
				.gameplayOptions.noEnergy = false;
		}

		private void MainGameSceneSetupDataOnDidFinishEvent(MainGameSceneSetupData arg1, LevelCompletionResults results)
		{
			if (!NoFail && HasTimeScaleChanged && results != null &&
			    results.levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared)
			{
				arg1.gameplayOptions.noEnergy = true;
				_resetNoFail = true;
			}
		}

		private void UIElementsCreatorOnValueChangedEvent(float timeScale)
		{
			if (!IsEqualToOne(timeScale))
			{
				HasTimeScaleChanged = true;
			}

			TimeScale = timeScale;
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

		public void OnUpdate()
		{

		}

		public void OnFixedUpdate()
		{

		}
	}
}