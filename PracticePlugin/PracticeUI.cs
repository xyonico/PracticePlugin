﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using UnityEngine;

namespace PracticePlugin
{
    public class PracticeUI : MonoBehaviour
    {
        
        private float _speed = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.practiceSettings.songSpeedMul;
        [UIValue("speed")]
        public float speed
        {
            get => _speed;
            set
            {
                _speed = value;
           //     Plugin.TimeScale = PracticeUI.instance.speed;
            }
        }
        [UIAction("setSpeed")]
        void SetSpeed(float value)
        {
            speed = value;
        }
        private float _njs = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.noteJumpMovementSpeed != 0?
            BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.noteJumpMovementSpeed : 
            BeatmapDifficultyMethods.NoteJumpMovementSpeed(BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.difficulty);

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

        [UIAction("#post-parse")]
        void PostParse()
        {
          if(gameObject.GetComponent<Touchable>() == null) 
                gameObject.AddComponent<Touchable>();
        }
    }
}
