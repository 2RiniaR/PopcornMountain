using UniRx;


namespace PopcornMountain.LobbyScene.User {
    public class UserSetNicknamePresenter : BaseLobbyComponent {

        private void Start() {
            var nicknameInputView = GetComponent<UserNicknameInputView>();

            nicknameInputView.OnChangeNicknameObservable
                .Subscribe(x => ConfigManager.Nickname = x)
                .AddTo(this);

            ConfigManager.OnNicknameChangedObservable
                .Subscribe(x => nicknameInputView.SetText(x))
                .AddTo(this);

            Manager.OnChangeIsTransitionEnabled
                .Subscribe(nicknameInputView.SetEnable)
                .AddTo(this);

        }
        
    }
}
