using UniRx;
using PopcornMountain.ConfigWindow;


namespace PopcornMountain.StartScene {
    public class ConfigWindowActivatorPresenter : BaseStartComponent {

        private void Start() {
            var elementView = GetComponent<ConfigWindowElementView>();
            var openButtonView = GetComponent<ConfigWindowOpenButtonView>();
            var animationView = GetComponent<ConfigWindowAnimatorView>();

            Manager.OnChangeIsTransitionEnabled
                .Subscribe(isEnable => {
                    elementView.SetEnable(isEnable);
                    openButtonView.SetEnable(isEnable);
                })
                .AddTo(this);

            animationView.OnShowAnimationStartObservable
                .Subscribe(_ => Manager.SetTransitionEnabled(false))
                .AddTo(this);

            animationView.OnHideAnimationFinishObservable
                .Subscribe(_ => Manager.SetTransitionEnabled(true))
                .AddTo(this);
        }

    }
}
