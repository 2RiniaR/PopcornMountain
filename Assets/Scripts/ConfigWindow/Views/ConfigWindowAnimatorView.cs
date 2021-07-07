using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.ConfigWindow {
    public class ConfigWindowAnimatorView : MonoBehaviour {

        [SerializeField]
        private Animator windowAnimator = null;

        private static readonly int showStateHash = Animator.StringToHash("Base Layer.Show");
        private static readonly int hideStateHash = Animator.StringToHash("Base Layer.Hide");
        private ObservableStateMachineTrigger animationTrigger = null;

        public IObservable<Unit> OnHideAnimationStartObservable { get; private set; }
        public IObservable<Unit> OnShowAnimationStartObservable { get; private set; }
        public IObservable<Unit> OnHideAnimationFinishObservable { get; private set; }
        public IObservable<Unit> OnShowAnimationFinishObservable { get; private set; }


        private void OnEnable() {
            animationTrigger = windowAnimator.GetBehaviour<ObservableStateMachineTrigger>();

            OnHideAnimationStartObservable = animationTrigger.OnStateEnterAsObservable()
                .Where(state => state.StateInfo.fullPathHash == hideStateHash)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            OnShowAnimationStartObservable = animationTrigger.OnStateEnterAsObservable()
                .Where(state => state.StateInfo.fullPathHash == showStateHash)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            OnHideAnimationFinishObservable = animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == hideStateHash)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            OnShowAnimationFinishObservable = animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == showStateHash)
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }


        public IObservable<Unit> CreateShowAnimationObservable() {
            return animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == showStateHash)
                .AsUnitObservable()
                .First()
                .DoOnSubscribe(() => windowAnimator.SetBool("isShow", true))
                .Publish()
                .RefCount();
        }

        public IObservable<Unit> CreateHideAnimationObservable() {
            return animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == hideStateHash)
                .AsUnitObservable()
                .First()
                .DoOnSubscribe(() => windowAnimator.SetBool("isShow", false))
                .Publish()
                .RefCount();
        }

    }
}
