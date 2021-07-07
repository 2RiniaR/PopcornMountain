using UnityEngine;
using UniRx;
using System;


namespace PopcornMountain.MainGameScene {
    public class HandGaugeActivatorPresenter : BaseMainGameComponent {

        private void Start() {
            var sliderView = GetComponent<HandGaugeSliderView>();
            var handManager = GetManager<HandManager>();

            handManager.OnGrabbedObservable
                .Subscribe(_ => sliderView.CreateShowAnimationObsrvable().Subscribe().AddTo(this))
                .AddTo(this);

            Observable.Merge(
                handManager.OnDurabilityFullObservable,
                handManager.OnReleasedObservable
            )
                .Delay(TimeSpan.FromSeconds(1.5f))
                .Where(_ => handManager.Durability >= HandManager.maxDurability && !handManager.IsGrabbing)
                .Subscribe(_ => sliderView.CreateHideAnimationObsrvable().Subscribe().AddTo(this))
                .AddTo(this);
        }

    }
}
