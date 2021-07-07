using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class PlaceIndicatorDisplayerPresenter : BaseMainGameComponent {

        private void Start() {
            var beansManager = GetManager<BeansManager>();
            var placeIndicatorView = GetComponent<PlaceIndicatorView>();

            beansManager.OnHeldBeans
                .Subscribe(_ => placeIndicatorView.Show())
                .AddTo(this);

            beansManager.OnReleasedBeans
                .Subscribe(_ => placeIndicatorView.Hide())
                .AddTo(this);
        }

    }
}
