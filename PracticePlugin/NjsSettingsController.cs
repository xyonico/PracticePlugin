using System;
using UnityEngine;

namespace PracticePlugin
{
    public class NjsSettingsController : ListSettingsController
    {
        public event Action<float> ValueChangedEvent;

        private int _indexOffset;
        private int defaultNJS;
        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            _indexOffset = Plugin.PracticeMode ? 1 : 20;
            numberOfElements = 50;
            defaultNJS = (int)Plugin._spawnController.GetPrivateField<float>("_noteJumpMovementSpeed");
            idx = defaultNJS;
            UIElementsCreator.njsSpeed = defaultNJS;
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
            if (idx == defaultNJS)
                return $"<u>{idx}</u>";
            else
            return idx.ToString();
        }
    }
}