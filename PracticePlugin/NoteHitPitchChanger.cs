using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PracticePlugin
{
	public static class NoteHitPitchChanger
	{
		private static CustomNoteCutSoundEffect _noteCutSoundEffect;
		
		public static void ReplacePrefab()
		{
			var noteCutSoundEffectManager = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>().FirstOrDefault();
			if (noteCutSoundEffectManager == null) return;
			var noteCutSoundEffect =
				noteCutSoundEffectManager.GetPrivateField<NoteCutSoundEffect>("_noteCutSoundEffectPrefab");
			var oldNotes = noteCutSoundEffect.GetSpawned();
			foreach (var oldNote in oldNotes)
			{
				Object.Destroy(oldNote);
			}
			noteCutSoundEffect.RecycleAll();
			_noteCutSoundEffect = CustomNoteCutSoundEffect.CopyOriginal(noteCutSoundEffect);
			_noteCutSoundEffect.CreatePool(20,
				new Action<NoteCutSoundEffect>(noteCutSoundEffectManager.SetCutSoundEffectEventCallbacks));
			noteCutSoundEffectManager.SetPrivateField("_noteCutSoundEffectPrefab", _noteCutSoundEffect);
		}
	}
}