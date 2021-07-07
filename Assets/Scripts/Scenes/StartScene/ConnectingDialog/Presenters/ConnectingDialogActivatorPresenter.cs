using UniRx;
using PopcornMountain.ConnectingDialog;


namespace PopcornMountain.StartScene {
    public class ConnectingDialogActivatorPresenter : BaseStartComponent {

        private void Start() {
            var animationView = GetComponent<ConnectingDialogView>();

            Manager.OnChangeIsConnectingObservable
                .Where(isConnecting => isConnecting)
                .Subscribe(isConnecting => animationView.CreateShowAnimationObservable().Subscribe().AddTo(this))
                .AddTo(this);

            Manager.OnChangeIsConnectingObservable
                .Where(isConnecting => !isConnecting)
                .Subscribe(isConnecting => animationView.CreateHideAnimationObservable().Subscribe().AddTo(this))
                .AddTo(this);
        }

    }
}
