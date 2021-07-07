using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.ErrorWindow {
    public class ErrorWindowView : BaseMainGameComponent {

        [SerializeField]
        private Animator windowAnimator = null;
        [SerializeField]
        private AudioClip errorSound = null;
        [SerializeField]
        private TextMeshProUGUI titleText = null;
        [SerializeField]
        private TextMeshProUGUI detailText = null;
        [SerializeField]
        private Button submitButton = null;


        private readonly int showStateHash = Animator.StringToHash("Base Layer.Show");
        private readonly int hideStateHash = Animator.StringToHash("Base Layer.Hide");
        private ObservableStateMachineTrigger animationTrigger = null;


        private void OnEnable() {
            animationTrigger = windowAnimator.GetBehaviour<ObservableStateMachineTrigger>();
        }


        private void PlayErrorSound() {
            AudioManager.Instance.PlayOneShot(errorSound);
        }


        private void Show() {
            windowAnimator.SetBool("isShow", true);
        }


        private void Hide() {
            windowAnimator.SetBool("isShow", false);
        }


        private void SetMessage(string title, string detail) {
            titleText.text = title;
            detailText.text = detail;
        }


        public IObservable<Unit> CreateShowAnimationObservable(string title, string detail) {
            var animatorObservable = animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == showStateHash)
                .DoOnSubscribe(() => {
                    Show();
                    SetMessage(title, detail);
                })
                .AsUnitObservable()
                .First()
                .Publish()
                .RefCount();

            var audioObservable = Observable.Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.3f))
                .Do(_ => PlayErrorSound())
                .First()
                .Publish()
                .RefCount();

            var showObservable = Observable.Merge(animatorObservable, audioObservable)
                .IgnoreElements()
                .Publish()
                .RefCount();

            var buttonClickObservable = submitButton
                .OnPointerUpAsObservable()
                .AsUnitObservable()
                .First()
                .Publish()
                .RefCount();

            var hideObservable = animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == hideStateHash)
                .DoOnSubscribe(Hide)
                .AsUnitObservable()
                .First()
                .Publish()
                .RefCount();

            return Observable.Concat(
                showObservable,
                buttonClickObservable,
                hideObservable
            );

        }

    }
}
