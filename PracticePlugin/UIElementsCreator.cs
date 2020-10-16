using System;
using TMPro;
using UnityEngine;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Attributes;
namespace PracticePlugin
{
    public class UIElementsCreator : MonoBehaviour
    {
        public event Action<float> ValueChangedEvent;
        public SongSeeker SongSeeker { get; private set; }
        internal static float currentNJS;
        internal static float currentSpawnOffset;
        internal static float defaultNJS;
        internal static float defaultOffset;

        internal static float _newTimeScale { get; private set; } = 1f;

        public void Init()
        {
            //  Invoke(nameof(InitDelayed), 0.1f);
            InitDelayed();
        }

        private void InitDelayed()
        {
            if (Plugin.PracticeMode)
            {
                var seekerObj = new GameObject("Song Seeker");
                seekerObj.transform.SetParent(transform, false);
                seekerObj.AddComponent<RectTransform>();
                SongSeeker = seekerObj.AddComponent<SongSeeker>();
                SongSeeker.Init();
                new GameObject("No Fail Game Energy").AddComponent<NoFailGameEnergy>();
                defaultNJS = Plugin._spawnController.GetPrivateField<BeatmapObjectSpawnController.InitData>("_initData").noteJumpMovementSpeed;
                currentNJS = defaultNJS;
                //        Console.WriteLine("NJS: " + UIElementsCreator.defaultNJS);
                defaultOffset = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.noteJumpStartBeatOffset;
                currentSpawnOffset = defaultOffset;
                //        Console.WriteLine("Offset: " + UIElementsCreator.defaultOffset);
            }

        }

        private void OnEnable()
        {
            /*
            if (_speedSettings == null && Plugin.PracticeMode)
            {
                _speedSettings = Instantiate(Plugin.SpeedSettingsObject, transform);
                _speedSettings.SetActive(true);

                var rectTransform = (RectTransform)_speedSettings.transform;
                rectTransform.anchorMin = Vector2.right * 0.5f;
                rectTransform.anchorMax = Vector2.right * 0.5f;
                rectTransform.anchoredPosition = new Vector2(5, 8);
                TextMeshProUGUI settingText = Plugin.CreateText(rectTransform, "Speed (Use at own risk!)", new Vector2(-30f, -2f));
                settingText.fontSize = 6f;
                speedController = _speedSettings.GetComponent<SpeedSettingsController>();
                speedController.ValueChangedEvent += SpeedControllerOnValueChangedEvent;
            }
            if (_njsSettings == null && Plugin.PracticeMode)
            {
                _njsSettings = Instantiate(Plugin.NjsSettingsObject, transform);
                _njsSettings.SetActive(true);

                var rectTransform = (RectTransform)_njsSettings.transform;
                rectTransform.anchorMin = Vector2.right * 0.5f;
                rectTransform.anchorMax = Vector2.right * 0.5f;
                rectTransform.anchoredPosition = new Vector2(5, 0);
                TextMeshProUGUI settingText = Plugin.CreateText(rectTransform, "NJS", new Vector2(-30f, -2f));
                settingText.fontSize = 6f;
                njsController = _njsSettings.GetComponent<NjsSettingsController>();
                njsController.ValueChangedEvent += NjsController_ValueChangedEvent;
            }
            if (_offsetSettings == null && Plugin.PracticeMode)
            {
                _offsetSettings = Instantiate(Plugin.SpawnOffsetSettingsObject, transform);
                _offsetSettings.SetActive(true);

                var rectTransform = (RectTransform)_offsetSettings.transform;
                rectTransform.anchorMin = Vector2.right * 0.5f;
                rectTransform.anchorMax = Vector2.right * 0.5f;
                rectTransform.anchoredPosition = new Vector2(5, -8);
                TextMeshProUGUI settingText = Plugin.CreateText(rectTransform, "Spawn Offset", new Vector2(-30f, -2f));
                settingText.fontSize = 6f;
                spawnOffsetController = _offsetSettings.GetComponent<SpawnOffsetController>();
                spawnOffsetController.ValueChangedEvent += SpawnOffsetController_ValueChangedEvent;
            }
            _newTimeScale = Plugin.TimeScale;
            njsController.Refresh(true);
            spawnOffsetController.Refresh(true);
            */



        }

        private void SpawnOffsetController_ValueChangedEvent(float offset)
        {
            currentSpawnOffset = offset;
            //  Plugin.AdjustNjsAndOffset();
            Plugin.UpdateSpawnMovementData(currentNJS, currentSpawnOffset);
            SongSeeker._startTimeSamples = SongSeeker._songAudioSource.timeSamples - 1;
        }

        private void NjsController_ValueChangedEvent(float njs)
        {
            currentNJS = njs;
            //  Plugin.AdjustNjsAndOffset();
            Plugin.UpdateSpawnMovementData(currentNJS, currentSpawnOffset);
            SongSeeker._startTimeSamples = SongSeeker._songAudioSource.timeSamples - 1;
        }

        private void OnDisable()
        {
            if (ValueChangedEvent != null)
            {
                ValueChangedEvent(_newTimeScale);
            }
            Plugin.UpdateSpawnMovementData(UIElementsCreator.currentNJS, UIElementsCreator.currentSpawnOffset);
      //      Destroy(_speedSettings);
        }

        private void SpeedControllerOnValueChangedEvent(float timeScale)
        {
            _newTimeScale = timeScale;
      //      njsController.Refresh(true);
       //     spawnOffsetController.Refresh(true);
            /*
            _newTimeScale = timeScale;
            if (Math.Abs(_newTimeScale - 1) > 0.0000000001f)
            {
                //      spawnOffsetController.enabled = false;
                //       njsController.enabled = false;

            }
            else
            {
                //       spawnOffsetController.enabled = true;
                //        njsController.enabled = true;

            }
            if (Plugin.PracticeMode) return;
            if (!Plugin.HasTimeScaleChanged && Math.Abs(_newTimeScale - 1) > 0.0000000001f)
            {
                _leaderboardText.text = "Leaderboard will be disabled!";
            }
            else
            {
                _leaderboardText.text = Plugin.HasTimeScaleChanged ? "Leaderboard has been disabled\nSet speed to 100% and restart to enable again" : string.Empty;
            }
            */
        }
    }
}