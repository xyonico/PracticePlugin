using Zenject;
namespace PracticePlugin
{
    /*
    public class CustomEffectPoolsInstaller : EffectPoolsInstaller
    {
        public override void ManualInstallBindings(DiContainer container, bool shortBeatEffect)
        {
            try
            {
                container.BindMemoryPool<FlyingTextEffect, FlyingTextEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(this._flyingTextEffectPrefab);
            container.BindMemoryPool<FlyingScoreEffect, FlyingScoreEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(this._flyingScoreEffectPrefab);
            container.BindMemoryPool<FlyingSpriteEffect, FlyingSpriteEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(this._flyingSpriteEffectPrefab);
            container.BindMemoryPool<NoteDebris, NoteDebris.Pool>().WithInitialSize(40).FromComponentInNewPrefab(this._noteDebrisHDConditionVariable ? this._noteDebrisHDPrefab : this._noteDebrisLWPrefab);
            container.BindMemoryPool<BeatEffect, BeatEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(shortBeatEffect ? this._shortBeatEffectPrefab : this._beatEffectPrefab);
            container.BindMemoryPool<NoteCutSoundEffect, NoteCutSoundEffect.Pool>().WithInitialSize(16).FromComponentInNewPrefab(this._noteCutSoundEffectPrefab);
            container.BindMemoryPool<BombCutSoundEffect, BombCutSoundEffect.Pool>().WithInitialSize(20).FromComponentInNewPrefab(this._bombCutSoundEffectPrefab);
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
*/
}