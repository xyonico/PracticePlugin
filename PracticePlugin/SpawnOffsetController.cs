using System;
using UnityEngine;

namespace PracticePlugin
{
    public class SpawnOffsetController : ListSettingsController
    {
        public event Action<float> ValueChangedEvent;

        private int _indexOffset;
        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            _indexOffset = Plugin.PracticeMode ? 1 : 20;
            numberOfElements = 100;
            idx = 50 + (int)(10 * UIElementsCreator.currentSpawnOffset); 
        }

        protected override void ApplyValue(int idx)
        {
        }

        protected override string TextForValue(int idx)
        {
            if (ValueChangedEvent != null)
            {
                ValueChangedEvent((idx - 50) / 10f);
            }
            if (((idx - 50) / 10f) == UIElementsCreator.defaultOffset)
                return $"<u>{(idx - 50) / 10f}</u>";
            else
                return ((idx - 50) /10f).ToString();
        }
    }
}