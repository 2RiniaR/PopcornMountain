using UniRx;


namespace PopcornMountain.MainGameScene.ResultWindow {
    public class ResultWindowButtonPresenter : BaseMainGameComponent {

        private void Start() {
            var elementView = GetComponent<ResultWindowElementView>();

            // 「もう一度」ボタンが押されたら、再び同じシーンに遷移する
            elementView.againButtonClickObservable
                .Subscribe(_ => SceneManager.ChangeSceneWithAnimation(GameScene.MainGameScene))
                .AddTo(this);

            // 「終わる」ボタンが押されたら、ロビーシーンまで戻る
            elementView.exitButtonClickObservable
                .Subscribe(_ => SceneManager.ChangeSceneWithAnimation(
                    GameScene.LobbyScene,
                    PhotonManager.CreateLeaveRoomObservable()
                ))
                .AddTo(this);
        }

    }
}
