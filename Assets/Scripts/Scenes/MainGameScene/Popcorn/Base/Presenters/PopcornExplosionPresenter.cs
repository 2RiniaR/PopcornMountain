using UniRx;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornExplosionPresenter : PhaseSwitchablePresenterMonobehavior {
        
        private void Start() {
            AddEnablePhases(GamePhase.Cooking);
            var cookingStatusModel = GetComponent<PopcornCookingStatusModel>();
            var parameterModel = GetComponent<PopcornParameterModel>();
            var explosionView = GetComponent<PopcornExplosionView>();

            var placingStatusModel = GetComponent<PopcornPlacingStatusModel>();
            var impactView = GetComponent<PopcornImpactView>();
            var impactReceiverView = transform.Find("Collider").GetComponent<PopcornImpactReceiverView>();

            // ポップコーンが破裂したとき、爆発を起こす
            cookingStatusModel.OnPoppedWithoutInitObservable
                .Subscribe(_ => explosionView.Explosion(parameterModel.parameters.explosionPower))
                .AddTo(this);

            impactReceiverView.OnReceiveObservable
                .Where(_ => !placingStatusModel.IsMoving)
                .Subscribe(impactView.Impact)
                .AddTo(this);
        }
        
    }
}
