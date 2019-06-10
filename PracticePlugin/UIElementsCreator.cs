using System;
using TMPro;
using UnityEngine;

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
        private GameObject _speedSettings = null;
        private GameObject _njsSettings = null;
        private GameObject _offsetSettings = null;
        internal SpeedSettingsController speedController;
        internal NjsSettingsController njsController;
        internal SpawnOffsetController spawnOffsetController;
        private TMP_Text _leaderboardText;
        private float _newTimeScale = 1;

        public void Init()
        {
            Invoke(nameof(InitDelayed), 0.1f);
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
                defaultNJS = Plugin._spawnController.GetPrivateField<float>("_noteJumpMovementSpeed");
                currentNJS = defaultNJS;
                //        Console.WriteLine("NJS: " + UIElementsCreator.defaultNJS);
                defaultOffset = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.noteJumpStartBeatOffset;
                currentSpawnOffset = defaultOffset;
                //        Console.WriteLine("Offset: " + UIElementsCreator.defaultOffset);
            }

        }

        private void OnEnable()
        {

            if (_speedSettings == null && Plugin.PracticeMode)
            {
                _speedSettings = Instantiate(Plugin.SpeedSettingsObject, transform);
                _speedSettings.SetActive(true);

                var rectTransform = (RectTransform)_speedSettings.transform;
                rectTransform.anchorMin = Vector2.right * 0.5f;
                rectTransform.anchorMax = Vector2.right * 0.5f;
                rectTransform.anchoredPosition = new Vector2(5, 4);
                TextMeshProUGUI settingText = CustomUI.BeatSaber.BeatSaberUI.CreateText(rectTransform, "Speed", new Vector2(-30f, -2f));
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
                rectTransform.anchoredPosition = new Vector2(5, -4);
                TextMeshProUGUI settingText = CustomUI.BeatSaber.BeatSaberUI.CreateText(rectTransform, "NJS", new Vector2(-30f, -2f));
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
                rectTransform.anchoredPosition = new Vector2(5, -12);
                TextMeshProUGUI settingText = CustomUI.BeatSaber.BeatSaberUI.CreateText(rectTransform, "Spawn Offset", new Vector2(-30f, -2f));
                settingText.fontSize = 6f;
                spawnOffsetController = _offsetSettings.GetComponent<SpawnOffsetController>();
                spawnOffsetController.ValueChangedEvent += SpawnOffsetController_ValueChangedEvent;
            }





        }

        private void SpawnOffsetController_ValueChangedEvent(float offset)
        {
            currentSpawnOffset = offset;
            Plugin.AdjustNjsAndOffset();
            SongSeeker._startTimeSamples = SongSeeker._songAudioSource.timeSamples - 1;
        }

        private void NjsController_ValueChangedEvent(float njs)
        {
            currentNJS = njs;
            Plugin.AdjustNjsAndOffset();
            SongSeeker._startTimeSamples = SongSeeker._songAudioSource.timeSamples - 1;
        }

        private void OnDisable()
        {
            if (ValueChangedEvent != null)
            {
                ValueChangedEvent(_newTimeScale);
            }
            Destroy(_speedSettings);
        }

        private void SpeedControllerOnValueChangedEvent(float timeScale)
        {
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
        }
    }
}