using UniRx;
using UnityEngine;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornHeatConductorPresenter : PhaseSwitchablePresenterMonobehavior {

        private void Start() {
            var cookingStatusModel = GetComponent<PopcornCookingStatusModel>();
            var heatReceiverView = transform.Find("Collider").GetComponent<PopcornHeatReceiverView>();

            heatReceiverView.OnReceiveObservable
                .Subscribe(heat => cookingStatusModel.AddHeat(heat))
                .AddTo(this);
        }

    }
}
