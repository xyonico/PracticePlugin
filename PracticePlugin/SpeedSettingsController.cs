using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PracticePlugin
{
	public class SpeedSettingsController : ListSettingsController
	{
		private const float MaxSize = 5f;
		private const float StepSize = 0.05f;
		
		protected override void GetInitValues(out int idx, out int numberOfElements)
		{
			numberOfElements = Mathf.RoundToInt(MaxSize / StepSize);
			idx = Mathf.RoundToInt(Plugin.TimeScale / StepSize) - 1;
		}

		protected override void ApplyValue(int idx)
		{
		}

		protected override string TextForValue(int idx)
		{
			Plugin.TimeScale = StepSize * (idx + 1);
			return StepSize * 100f * (idx + 1) + "%";
		}
	}
}