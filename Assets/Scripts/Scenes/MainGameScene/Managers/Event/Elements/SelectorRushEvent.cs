using System;
using UniRx;
using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public class SelectorRushEvent : BaseEventElement {

        private SelectorManager selectorManager = GetManager<SelectorManager>();


        /// <summary>
        ///   イベントを発生させて、発生終了時にOnCompleted()が発行されるObservableを返す
        /// </summary>
        public override IObservable<Unit> CreateRunEventObservable() {
            var timerObservable = Observable.ReturnUnit()
                .Delay(TimeSpan.FromSeconds(15f))
                .Publish()
                .RefCount();

            selectorManager.OnItemEmitObservable
                .AsUnitObservable()
                .StartWith(Unit.Default)
                .TakeUntil(timerObservable)
                .Subscribe(_ => selectorManager.EmitSelector());

            return timerObservable;
        }

    }
}