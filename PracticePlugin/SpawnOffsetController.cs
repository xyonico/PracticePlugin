using System;
using UnityEngine;

namespace PracticePlugin
{
    public class SpawnOffsetController : ListSettingsController
    {
        public event Action<int> ValueChangedEvent;

        private int _indexOffset;
        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            _indexOffset = Plugin.PracticeMode ? 1 : 20;
            numberOfElements = 50;
            idx = UIElementsCreator.currentSpawnOffset; 
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
            if (idx == UIElementsCreator.defaultOffset)
                return $"<u>{idx}</u>";
            else
                return idx.ToString();
        }
    }
}