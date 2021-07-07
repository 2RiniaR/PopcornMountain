using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.ResultWindow {
    public class ResultWindowAnimationView : BaseMainGameComponent {

        [SerializeField]
        private Animator windowAnimator = null;
        [SerializeField]
        private AudioClip loseSound = null;
        [SerializeField]
        private AudioClip winSound = null;

        private int showStateHash = Animator.StringToHash("Base Layer.Show");
        private ObservableStateMachineTrigger animationTrigger = null;


        private void OnEnable() {
            animationTrigger = windowAnimator.GetBehaviour<ObservableStateMachineTrigger>();
        }


        private void PlayResultSound(PlayerID winner) {
            switch(winner) {
                case PlayerID.Self:
                    AudioManager.Instance.PlayOneShot(winSound);
                    break;
                case PlayerID.Other:
                    AudioManager.Instance.PlayOneShot(loseSound);
                    break;
            }
        }


        public IObservable<Unit> CreateShowAnimationObservable(PlayerID winner) {
            var animatorObservable = animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == showStateHash)
                .DoOnSubscribe(() => {
                    windowAnimator.SetInteger("MessageType", (int)winner);
                    windowAnimator.SetTrigger("Show");
                })
                .AsUnitObservable()
                .First()
                .Publish()
                .RefCount();

            var audioObservable = Observable.Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(1f))
                .Do(_ => PlayResultSound(winner))
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
