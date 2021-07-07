using System;
using UniRx;
using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public class HandManager : BaseMainGameManager {

        public const float minDurability = 0f;
        public const float maxDurability = 1f;


        private CompositeDisposable onDispose = new CompositeDisposable();
        private FloatReactiveProperty durability = new FloatReactiveProperty(maxDurability);
        private BoolReactiveProperty isGrabbing = new BoolReactiveProperty(false);
        private float autoRecoverSpeed = 0.002f;
        private float decrement = 0f;


        public IObservable<float> OnDurabilityChangedObservable { get { return durability; } }
        public IObservable<Unit> OnDurabilityFullObservable { get; private set; } = null;
        public IObservable<Unit> OnDurabilityEmptyObservable { get; private set; } = null;
        public IObservable<Unit> OnGrabbedObservable { get; private set; } = null;
        public IObservable<Unit> OnReleasedObservable { get; private set; } = null;
        public float Durability { get { return durability.Value; } }
        public bool IsGrabbing { get { return isGrabbing.Value; } }


        public HandManager() {
            var beansManager = GetManager<BeansManager>();

            OnDurabilityFullObservable = OnDurabilityChangedObservable
                .Pairwise()
                .Where(x => x.Previous < maxDurability && x.Current >= maxDurability)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            OnDurabilityEmptyObservable = OnDurabilityChangedObservable
                .Pairwise()
                .Where(x => x.Previous > minDurability && x.Current <= minDurability)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            OnGrabbedObservable = isGrabbing.Where(x => x).AsUnitObservable().Publish().RefCount();
            OnReleasedObservable = isGrabbing.Where(x => !x).AsUnitObservable().Publish().RefCount();

            Observable.EveryUpdate()
                .Subscribe(_ => AddDurability(IsGrabbing ? -decrement : autoRecoverSpeed))
                .AddTo(onDispose);

            beansManager.OnHeldBeans
                .Subscribe(_ => Grab(0.05f, 0.0015f))
                .AddTo(onDispose);

            beansManager.OnReleasedBeans
                .Subscribe(_ => Release())
                .AddTo(onDispose);
        }

        public override void Dispose() {
            onDispose.Dispose();
        }


        private void AddDurability(float value) {
            durability.Value = Mathf.Clamp(durability.Value + value, minDurability, maxDurability);
        }

        public void Grab(float startDecrement, float continuousDecrement) {
            isGrabbing.Value = true;
            AddDurability(-startDecrement);
            this.decrement = continuousDecrement;
        }

        public void Release() {
            isGrabbing.Value = false;
        }

    }
}
