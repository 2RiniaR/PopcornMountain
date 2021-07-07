using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.StartEffect {
    public class StartEffectView : BaseMainGameComponent {

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private AudioClip crapSound = null;
        [SerializeField]
        private AudioClip shineSound = null;
        private int startStateHash = Animator.StringToHash("Base Layer.Start");
        private ObservableStateMachineTrigger animationTrigger;


        private void Awake() {
            animationTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
        }

        private void PlayCrapSound() {
            AudioManager.Instance.PlayOneShot(crapSound);
        }

        private void PlayShineSound() {
            AudioManager.Instance.PlayOneShot(shineSound);
        }

        private void PlayOpeningAnimation() {
            _animator.SetTrigger("Show");
        }


        public IObservable<Unit> CreatePlayingObservable() {
            var animatorObservable = animationTrigger
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == startStateHash)
                .AsUnitObservable()
                .DoOnSubscribe(PlayOpeningAnimation)
                .First()
                .Publish()
                .RefCount();

            var audioObservable = Observable.Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.4f))
                .Do(_ => PlayCrapSound())
                .Delay(TimeSpan.FromSeconds(0.3f))
                .Do(_ => PlayCrapSound())
                .Delay(TimeSpan.FromSeconds(0.3f))
                .Do(_ => PlayShineSound())
                .First()
                .Publish()
                .RefCount();

            return Observable.Merge(animatorObservable, audioObservable)
                .IgnoreElements()
                .Publish()
                .RefCount();
        }
        
    }
}
