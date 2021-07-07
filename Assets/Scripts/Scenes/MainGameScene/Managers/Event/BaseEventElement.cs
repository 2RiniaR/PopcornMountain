using System;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public abstract class BaseEventElement : BaseMainGameManager {

        public BaseEventElement() : base() {}

        /// <summary>
        ///   イベントを発生させて、発生終了時にOnCompleted()が発行されるObservableを返す
        /// </summary>
        public abstract IObservable<Unit> CreateRunEventObservable();

    }
}