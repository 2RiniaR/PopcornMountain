using UnityEngine;
using UniRx;


namespace PopcornMountain.StartScene {
    public class SceneChangerPresenter : BaseStartComponent {

        private void Start() {
            var startMenuView = GetComponent<StartMenuView>();

            startMenuView.startMultiPlayButtonClickObservable
                .Subscribe(async _ => {
                    Manager.SetTransitionEnabled(false);
                    await PhotonManager.CreateConnectObservable()
                        .DoOnSubscribe(() => Manager.SetConnecting(true))
                        .DoOnCompleted(() => Manager.SetConnecting(false));
                    SceneManager.ChangeSceneWithAnimation(GameScene.LobbyScene);
                })
                .AddTo(this);

            // 終了ボタンが押された時、ゲームを終了する
            startMenuView.exitButtonClickObservable
                .Subscribe(_ => {
                    Manager.SetTransitionEnabled(false);
                    SceneManager.ExitGame();
                })
                .AddTo(this);

            Manager.OnChangeIsTransitionEnabled
                .Subscribe(isEnable => startMenuView.SetEnable(isEnable))
                .AddTo(this);
        }

    }
}
