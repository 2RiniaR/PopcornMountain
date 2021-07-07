using UniRx;
using PopcornMountain.ConnectingDialog;


namespace PopcornMountain.LobbyScene {
    public class ConnectingDialogActivatorPresenter : BaseLobbyComponent {

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
