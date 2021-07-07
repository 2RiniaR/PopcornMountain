using UnityEngine;
using UniRx;


namespace PopcornMountain.LobbyScene.RandomMatch {
    public class RandomMatchSubmitPresenter : BaseLobbyComponent {

        private void Start() {
            var buttonView = GetComponent<RandomMatchButtonView>();

            buttonView.OnPushButtonObservable
                .Subscribe(async _ => {
                    Manager.SetTransitionEnabled(false);
                    await PhotonManager.CreateMatchRandomRoomObservable()
                        .DoOnSubscribe(() => Manager.SetConnecting(true))
                        .DoOnCompleted(() => Manager.SetConnecting(false));
                    SceneManager.ChangeSceneWithAnimation(GameScene.MainGameScene);
                })
                .AddTo(this);

            Manager.OnChangeIsTransitionEnabled
                .Subscribe(buttonView.SetEnable)
                .AddTo(this);
        }
        
    }
}
