using UnityEngine;
using UniRx;


namespace PopcornMountain.LobbyScene.Menu {
    public class MenuButtonsPresenter : BaseLobbyComponent {

        private void Start() {
            var buttonView = GetComponent<MenuButtonsView>();

            buttonView.OnPushReturnButtonObservable
                .Subscribe(async _ => {
                    Manager.SetTransitionEnabled(false);
                    await PhotonManager.CreateDisconnectObservable()
                        .DoOnSubscribe(() => Manager.SetConnecting(true))
                        .DoOnCompleted(() => Manager.SetConnecting(false));
                    SceneManager.ChangeSceneWithAnimation(GameScene.StartScene);
                })
                .AddTo(this);

            Manager.OnChangeIsTransitionEnabled
                .Subscribe(buttonView.SetEnable)
                .AddTo(this);
        }

    }
}
