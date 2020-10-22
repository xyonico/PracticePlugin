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
        public static SongSeeker SongSeeker { get; private set; }
        internal static float defaultNJS;
        internal static float defaultOffset;
        internal static PracticeUI practiceUI;
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
           //     PracticeUI.instance.njs = defaultNJS;
                //        Console.WriteLine("NJS: " + UIElementsCreator.defaultNJS);
                defaultOffset = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.noteJumpStartBeatOffset;
          //      PracticeUI.instance.offset = defaultOffset;
                //        Console.WriteLine("Offset: " + UIElementsCreator.defaultOffset);
            }

        }

        private void OnEnable()
        {


        }

        public static void SpawnOffsetController_ValueChangedEvent(float offset)
        {
            //  Plugin.AdjustNjsAndOffset();
            Plugin.UpdateSpawnMovementData(practiceUI.njs, practiceUI.offset);
        }

        public static void NjsController_ValueChangedEvent(float njs)
        {
            //  Plugin.AdjustNjsAndOffset();
            Plugin.UpdateSpawnMovementData(practiceUI.njs, practiceUI.offset);

        }

        private void OnDisable()
        {
            if (ValueChangedEvent != null)
            {
                ValueChangedEvent(_newTimeScale);
            }
            if(SongSeeker._songAudioSource.time > 0)
            {
                SongSeeker._startTimeSamples = SongSeeker._songAudioSource.timeSamples - 1;
                SongSeeker.ApplyPlaybackPosition();
                Plugin.TimeScale = practiceUI.speed;
            }

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