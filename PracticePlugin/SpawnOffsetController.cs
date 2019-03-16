using System;
using UnityEngine;

namespace PracticePlugin
{
    public class SpawnOffsetController : ListSettingsController
    {
        public event Action<float> ValueChangedEvent;

        private int _indexOffset;
        private int defaultOffset;
        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            _indexOffset = Plugin.PracticeMode ? 1 : 20;
            numberOfElements = 50;
            defaultOffset = (int)Plugin._levelData.GameplayCoreSceneSetupData.difficultyBeatmap.noteJumpStartBeatOffset;
            idx = defaultOffset;
            UIElementsCreator.spawnOffset = defaultOffset;
        }

        protected override void ApplyValue(int idx)
        {
        }

        protected override string TextForValue(int idx)
        {
            if (ValueChangedEvent != null)
            {
                ValueChangedEvent(idx);
            }
            if (idx == defaultOffset)
                return $"<u>{idx}</u>";
            else
                return idx.ToString();
        }
    }
}