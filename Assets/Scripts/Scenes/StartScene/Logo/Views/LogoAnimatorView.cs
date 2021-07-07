using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.StartScene {
    public class LogoAnimatorView : MonoBehaviour {

        [SerializeField]
        private Animator logoAnimator = null;

        private static readonly int showStateHash = Animator.StringToHash("Base Layer.Show");
        private static readonly int hideStateHash = Animator.StringToHash("Base Layer.Hide");
        public IObservable<Unit> OnHideAnimationFinishObservable { get; private set; }
        public IObservable<Unit> OnShowAnimationStartObservable { get; private set; }

        private void Awake() {
            var animationTrigger = logoAnimator.GetBehaviour<ObservableStateMachineTrigger>();

            OnHideAnimationFinishObservable = animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == hideStateHash)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            OnShowAnimationStartObservable = animationTrigger.OnStateEnterAsObservable()
                .Where(state => state.StateInfo.fullPathHash == showStateHash)
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }

        public void Show() {
            logoAnimator.SetBool("isShow", true);
        }

        public void Hide() {
            logoAnimator.SetBool("isShow", false);
        }

    }
}
