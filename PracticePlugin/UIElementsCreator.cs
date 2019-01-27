using System;
using TMPro;
using UnityEngine;

namespace PracticePlugin
{
    public class UIElementsCreator : MonoBehaviour
    {
        public event Action<float> ValueChangedEvent;
        public SongSeeker SongSeeker { get; private set; }

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
            }

        }

        private void OnEnable()
        {
            if (Plugin.multiActive == false)
            {
                if (_speedSettings == null && Plugin.PracticeMode)
                {
                    _speedSettings = Instantiate(Plugin.SpeedSettingsObject, transform);
                    _speedSettings.SetActive(true);

                    var rectTransform = (RectTransform)_speedSettings.transform;
                    rectTransform.anchorMin = Vector2.right * 0.5f;
                    rectTransform.anchorMax = Vector2.right * 0.5f;
                    rectTransform.anchoredPosition = new Vector2(0, 10);

                    speedController = _speedSettings.GetComponent<SpeedSettingsController>();
                    speedController.ValueChangedEvent += SpeedControllerOnValueChangedEvent;
                    speedController.Init();
                }
                if (_njsSettings == null && Plugin.PracticeMode)
                {
                    _njsSettings = Instantiate(Plugin.NjsSettingsObject, transform);
                    _njsSettings.SetActive(true);

                    var rectTransform = (RectTransform)_njsSettings.transform;
                    rectTransform.anchorMin = Vector2.right * 0.5f;
                    rectTransform.anchorMax = Vector2.right * 0.5f;
                    rectTransform.anchoredPosition = new Vector2(0, 2);

                    njsController = _njsSettings.GetComponent<NjsSettingsController>();
                    njsController.ValueChangedEvent += NjsController_ValueChangedEvent;
                    njsController.Init();
                }
                if (_offsetSettings == null && Plugin.PracticeMode)
                {
                    _offsetSettings = Instantiate(Plugin.SpawnOffsetSettingsObject, transform);
                    _offsetSettings.SetActive(true);

                    var rectTransform = (RectTransform)_offsetSettings.transform;
                    rectTransform.anchorMin = Vector2.right * 0.5f;
                    rectTransform.anchorMax = Vector2.right * 0.5f;
                    rectTransform.anchoredPosition = new Vector2(0, -6);

                    spawnOffsetController = _offsetSettings.GetComponent<SpawnOffsetController>();
                    spawnOffsetController.ValueChangedEvent += SpawnOffsetController_ValueChangedEvent;
                    spawnOffsetController.Init();
                }




            }
        }

        private void SpawnOffsetController_ValueChangedEvent(float offset)
        {
            Plugin.AdjustSpawnOffset(offset);
            SongSeeker.ApplyPlaybackPosition();
        }

        private void NjsController_ValueChangedEvent(float njs)
        {
            Plugin.AdjustNJS(njs);
            SongSeeker.ApplyPlaybackPosition();
        }

        private void OnDisable()
        {
            if (ValueChangedEvent != null)
            {
                ValueChangedEvent(_newTimeScale);
            }
            DestroyImmediate(_speedSettings);
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