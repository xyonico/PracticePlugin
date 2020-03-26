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
            string result;

            if (idx == UIElementsCreator.defaultNJS)
                result = $"<u>{idx}</u>";
            else
                result = idx.ToString();
            if(Plugin.adjustNJSWithSpeed && UIElementsCreator._newTimeScale != 1f)
                result += $"({(idx * (1f / UIElementsCreator._newTimeScale)).ToString("F2")})";
            return result;
        }
    }
}