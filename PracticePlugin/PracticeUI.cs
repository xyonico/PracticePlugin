using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Attributes;
namespace PracticePlugin
{
    public class PracticeUI : NotifiableSingleton<PracticeUI>
    {
        private float _speed = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.practiceSettings.songSpeedMul;
        [UIValue("speed")]
        public float speed
        {
            get => _speed;
            set
            {
                _speed = value;
            }
        }
        [UIAction("setSpeed")]
        void SetSpeed(float value)
        {
            speed = value;
            Plugin.TimeScale = value;
        }
        private float _njs = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.noteJumpMovementSpeed;
        [UIValue("njs")]
        public float njs
        {
            get => _njs;
            set
            {
                _njs = value;
            }
        }
        [UIAction("setnjs")]
        void SetNjs(float value)
        {
            njs = value;
            UIElementsCreator.NjsController_ValueChangedEvent(value);
        }
        private float _offset = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.noteJumpStartBeatOffset;
        [UIValue("offset")]
        public float offset
        {
            get => _offset;
            set
            {
                _offset = value;
            }
        }
        [UIAction("setoffset")]
        void SetSpawnOffset(float value)
        {
            offset = value;
            UIElementsCreator.SpawnOffsetController_ValueChangedEvent(value);
        }

        [UIAction("speedFormatter")]
        string speedForValue(float value)
        {
            return $"{(int)(value * 100)}%";
        }
        [UIAction("njsFormatter")]
        string njsForValue(float value)
        {
            return value == UIElementsCreator.defaultNJS ? $"<u>{value}</u>" : $"{value}";
        }
        [UIAction("spawnOffsetFormatter")]
        string offsetForValue(float value)
        {
            return value == UIElementsCreator.defaultOffset ? $"<u>{value.ToString("F2")}</u>" : $"{value.ToString("F2")}";
        }
    }
}
