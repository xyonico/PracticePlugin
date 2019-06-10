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
            numberOfElements = 50;
            idx = (int)(10 * UIElementsCreator.currentSpawnOffset); 
        }

        protected override void ApplyValue(int idx)
        {
        }

        protected override string TextForValue(int idx)
        {
            if (ValueChangedEvent != null)
            {
                ValueChangedEvent(idx / 10f);
            }
            if ((idx / 10f) == UIElementsCreator.defaultOffset)
                return $"<u>{idx / 10f}</u>";
            else
                return (idx /10f).ToString();
        }
    }
}