using UniRx;
using PopcornMountain.ConfigWindow;


namespace PopcornMountain.LobbyScene {
    public class ConfigWindowActivatorPresenter : BaseLobbyComponent {

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
