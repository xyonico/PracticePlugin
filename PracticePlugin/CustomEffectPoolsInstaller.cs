namespace PracticePlugin
{
	public class CustomEffectPoolsInstaller : EffectPoolsInstaller
	{
		public override void InstallBindings()
		{
			Container.BindMemoryPool<FlyingTextEffect, FlyingTextEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(_flyingTextEffectPrefab);
			Container.BindMemoryPool<FlyingScoreTextEffect, FlyingScoreTextEffect.Pool>().WithInitialSize(20)
				.FromComponentInNewPrefab(_flyingScoreTextEffectPrefab);
			Container.BindMemoryPool<FlyingSpriteEffect, FlyingSpriteEffect.Pool>().WithInitialSize(20)
				.FromComponentInNewPrefab(_flyingSpriteEffectPrefab);
			Container.BindMemoryPool<NoteDebris, NoteDebris.Pool>().WithInitialSize(30).FromComponentInNewPrefab(_noteDebrisPrefab);
			Container.BindMemoryPool<BeatEffect, BeatEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(_beatEffectPrefab);
			Container.BindMemoryPool<BombCutSoundEffect, BombCutSoundEffect.Pool>().WithInitialSize(20)
				.FromComponentInNewPrefab(_bombCutSoundEffectPrefab);;
			
			Container.BindMemoryPool<NoteCutSoundEffect, NoteCutSoundEffect.Pool>().WithInitialSize(10)
				.FromComponentInNewPrefab(ReplacePrefab());
		}

		private CustomNoteCutSoundEffect ReplacePrefab()
		{
			return CustomNoteCutSoundEffect.CopyOriginal(_noteCutSoundEffectPrefab);
		}
	}
}