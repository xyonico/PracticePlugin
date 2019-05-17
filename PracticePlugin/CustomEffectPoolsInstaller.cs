namespace PracticePlugin
{
    public class CustomEffectPoolsInstaller : EffectPoolsInstaller
    {
        public override void InstallBindings()
        {
            try
            {
                Container.BindMemoryPool<FlyingTextEffect, FlyingTextEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(_flyingTextEffectPrefab);
                Container.BindMemoryPool<FlyingScoreEffect, FlyingScoreEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(_flyingScoreEffectPrefab);
                Container.BindMemoryPool<FlyingSpriteEffect, FlyingSpriteEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(_flyingSpriteEffectPrefab);
                Container.BindMemoryPool<NoteDebris, NoteDebris.Pool>().WithInitialSize(40).FromComponentInNewPrefab(_noteDebrisHDConditionVariable ? _noteDebrisHDPrefab : _noteDebrisLWPrefab);
                Container.BindMemoryPool<BeatEffect, BeatEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(_beatEffectPrefab);
                Container.BindMemoryPool<NoteCutSoundEffect, NoteCutSoundEffect.Pool>().WithInitialSize(16).FromComponentInNewPrefab(_noteCutSoundEffectPrefab);
                Container.BindMemoryPool<BombCutSoundEffect, BombCutSoundEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(_bombCutSoundEffectPrefab);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }

        }

        private CustomNoteCutSoundEffect ReplacePrefab()
        {
            return CustomNoteCutSoundEffect.CopyOriginal(_noteCutSoundEffectPrefab);
        }
    }
}