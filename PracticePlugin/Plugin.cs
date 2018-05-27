using System;
using System.IO;
using System.Linq;
using IllusionPlugin;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using Object = UnityEngine.Object;

namespace PracticePlugin
{
	public class Plugin : IPlugin
	{
		public static float TimeScale = 1;
		public static GameObject SettingsObject;
		
		private bool _enabled;
		private bool _init;
		private MainGameSceneSetupData _mainGameSceneSetupData;
		private AudioTimeSyncController _audioTimeSync;
		private AudioSource _songAudio;
		private AudioSource _noteCutAudioSource;
		private string _lastLevelId;
		
		public string Name
		{
			get { return "Practice Plugin"; }
		}

		public string Version
		{
			get { return "v1.0"; }
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
				_enabled = _mainGameSceneSetupData.gameplayOptions.noEnergy;
				var noteCutSoundEffectManager = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>().FirstOrDefault();
				var noteCutSoundEffect =
					ReflectionUtil.GetPrivateField<NoteCutSoundEffect>(noteCutSoundEffectManager, "_noteCutSoundEffectPrefab");
				_noteCutAudioSource =
					ReflectionUtil.GetPrivateField<AudioSource>(noteCutSoundEffect, "_audioSource");

				var canvas = Resources.FindObjectsOfTypeAll<HorizontalLayoutGroup>().FirstOrDefault(x => x.name == "Buttons")
					.transform.parent;
				canvas.gameObject.AddComponent<SpeedSettingsCreator>();
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
			if (_songAudio == null) return;
			if (!_enabled)
			{
				TimeScale = 1;
			}

			_noteCutAudioSource.pitch = TimeScale;
			_songAudio.pitch = TimeScale;
		}

		public void OnFixedUpdate()
		{
			
		}
	}
}