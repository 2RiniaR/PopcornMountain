using UniRx;


namespace PopcornMountain.MainGameScene {
    public class HeatConductorPresenter : BaseMainGameComponent {

        private void Start() {
            var heatSourceColliderView = GetComponent<HeatSourceColliderView>();
            var fireManager = GetManager<FireManager>();

            heatSourceColliderView.OnPopcornStayInHeatAreaObservable
                .Subscribe(x => x.Item1.Issue(fireManager.GetConductionHeat(x.Item2)))
                .AddTo(this);
        }

    }
}
