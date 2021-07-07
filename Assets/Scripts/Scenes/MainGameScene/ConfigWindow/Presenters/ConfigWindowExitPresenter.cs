using UniRx;


namespace PopcornMountain.MainGameScene {
    public class ConfigWindowExitPresenter : BaseMainGameComponent {

        private void Start() {
            var exitButtonView = GetComponent<ConfigWindowExitButtonView>();

            exitButtonView.ExitButtonClickObservable
                .Subscribe(_ => {
                    SceneManager.ChangeSceneWithAnimation(
                        GameScene.StartScene,
                        Observable.Concat(
                            PhotonManager.CreateLeaveRoomObservable(),
                            PhotonManager.CreateDisconnectObservable()
                        )
                    );
                })
                .AddTo(this);
        }

    }
}
