using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class HandGaugeValuePresenter : BaseMainGameComponent {

        private void Start() {
            var sliderView = GetComponent<HandGaugeSliderView>();
            var handManager = GetManager<HandManager>();

            handManager.OnDurabilityChangedObservable
                .Subscribe(durability => sliderView.SetValue(durability, HandManager.maxDurability))
                .AddTo(this);

            handManager.OnDurabilityEmptyObservable
                .Subscribe(_ => sliderView.PlayEmptyAnimation())
                .AddTo(this);
        }

    }
}
