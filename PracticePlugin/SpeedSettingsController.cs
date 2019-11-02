using System;
using UnityEngine;

namespace PracticePlugin
{
    public class SpeedSettingsController : ListSettingsController
    {
        public event Action<float> ValueChangedEvent;

        private int _indexOffset;

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            _indexOffset = Plugin.PracticeMode ? 1 : 20;
            //  numberOfElements = Mathf.RoundToInt(Plugin.SpeedMaxSize / Plugin.SpeedStepSize) - _indexOffset;
            numberOfElements = 0;
            idx = 0;
       //     idx = Mathf.RoundToInt(Plugin.TimeScale / Plugin.SpeedStepSize) - _indexOffset;
        }

        protected override void ApplyValue(int idx)
        {
        }

        protected override string TextForValue(int idx)
        {
            if (ValueChangedEvent != null)
            {
                ValueChangedEvent(Plugin.SpeedStepSize * (idx + _indexOffset));
            }
            return "";
      //      return Plugin.SpeedStepSize * 100f * (idx + _indexOffset) + "%";
        }
    }
}