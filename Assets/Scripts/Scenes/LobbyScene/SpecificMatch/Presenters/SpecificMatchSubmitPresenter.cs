using UniRx;


namespace PopcornMountain.LobbyScene.SpecificMatch {
    public class SpecificMatchSubmitPresenter : BaseLobbyComponent {

        private void Start() {
            var specificMatchView = GetComponent<SpecificMatchView>();

            specificMatchView.OnPushButtonObservable
                .Subscribe(async _ => {
                    if (specificMatchView.password.Length == 0) {
                        specificMatchView.Warning();
                        return;
                    }
                    Manager.SetTransitionEnabled(false);
                    await PhotonManager.CreateMatchSpecificRoomObservable(specificMatchView.password)
                        .DoOnSubscribe(() => Manager.SetConnecting(true))
                        .DoOnCompleted(() => Manager.SetConnecting(false));
                    SceneManager.ChangeSceneWithAnimation(GameScene.MainGameScene);
                })
                .AddTo(this);

            Manager.OnChangeIsTransitionEnabled
                .Subscribe(isEnable => specificMatchView.SetEnable(isEnable))
                .AddTo(this);
        }

    }
}
