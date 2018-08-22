using System;
using UnityEngine;

namespace PracticePlugin
{
	public class CustomNoteCutSoundEffect : NoteCutSoundEffect
	{
		public static CustomNoteCutSoundEffect CopyOriginal(NoteCutSoundEffect original)
		{
			var gameObj = Instantiate(original.gameObject);
			gameObj.name = "This is a copy!";
			//gameObj.SetActive(false);
			original = gameObj.GetComponent<NoteCutSoundEffect>();
			var noteCutSoundEffect = (CustomNoteCutSoundEffect) ReflectionUtil.CopyComponent(original, typeof(NoteCutSoundEffect),
				typeof(CustomNoteCutSoundEffect), gameObj);
			DestroyImmediate(original);
			noteCutSoundEffect.Awake();
			return noteCutSoundEffect;
		}

		public override void Awake()
		{
			if (_badCutSoundEffectAudioClips == null) return;
			base.Awake();
		}

		public override void LateUpdate()
		{
			if (_audioSource.clip == null) return;
			base.LateUpdate();
		}

		public override void Init(AudioClip audioClip, double noteDSPTime, float aheadTime, float missedTimeOffset,
			Saber saber, NoteData noteData, bool handleWrongSaberTypeAsGood)
		{
			Console.WriteLine("Custom Init!");
			base.Init(audioClip, noteDSPTime, aheadTime, missedTimeOffset, saber, noteData, handleWrongSaberTypeAsGood);
			_audioSource.Stop();
			var dspTime = AudioSettings.dspTime;
			var timeDiff = noteDSPTime - dspTime;
			timeDiff /= Plugin.TimeScale;
			var newTime = dspTime + (timeDiff - aheadTime);
			_audioSource.PlayScheduled(newTime);
		}
	}
}