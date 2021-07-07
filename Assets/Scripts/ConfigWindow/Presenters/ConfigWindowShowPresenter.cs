using UnityEngine;
using UniRx;


namespace PopcornMountain.ConfigWindow {
    public class ConfigWindowShowPresenter : MonoBehaviour {

        private void Start() {
            var elementView = GetComponent<ConfigWindowElementView>();
            var animatorView = GetComponent<ConfigWindowAnimatorView>();
            var openButtonView = GetComponent<ConfigWindowOpenButtonView>();

            openButtonView.openButtonClickObservable
                .Subscribe(async _ => {
                    openButtonView.SetEnable(false);
                    await animatorView.CreateShowAnimationObservable();
                    elementView.SetEnable(true);
                })
                .AddTo(this);

            elementView.ReturnButtonClickObservable
                .Subscribe(async _ => {
                    elementView.SetEnable(false);
                    await animatorView.CreateHideAnimationObservable();
                    openButtonView.SetEnable(true);
                })
                .AddTo(this);
        }

    }
}
