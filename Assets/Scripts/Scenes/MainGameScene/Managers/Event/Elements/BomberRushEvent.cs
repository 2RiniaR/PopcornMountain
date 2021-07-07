using System;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class BomberRushEvent : BaseEventElement {

        private BeansManager beansManager = GetManager<BeansManager>();


        /// <summary>
        ///   イベントを発生させて、発生終了時にOnCompleted()が発行されるObservableを返す
        /// </summary>
        public override IObservable<Unit> CreateRunEventObservable() {
            return Observable.Interval(TimeSpan.FromSeconds(2f))
                .Take(3)
                .Do(_ => beansManager.EmitBean(PopcornType.Bomber))
                .AsUnitObservable()
                .IgnoreElements()
                .Publish()
                .RefCount();
        }

    }
}