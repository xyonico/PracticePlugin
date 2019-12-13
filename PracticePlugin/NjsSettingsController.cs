using System;
using UnityEngine;

namespace PracticePlugin
{
    public class NjsSettingsController : ListSettingsController
    {
        public event Action<float> ValueChangedEvent;

        private int _indexOffset;
        protected override bool GetInitValues(out int idx, out int numberOfElements)
        {
            _indexOffset = Plugin.PracticeMode ? 1 : 20;
            numberOfElements = 50;
            idx = (int)UIElementsCreator.currentNJS;
            return true;
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
            if (idx == UIElementsCreator.defaultNJS)
                return $"<u>{idx}</u>";
            else
            return idx.ToString();
        }
    }
}