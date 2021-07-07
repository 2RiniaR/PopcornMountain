using System;
using UniRx;


namespace PopcornMountain {
    public abstract class BaseSceneManager : IDisposable {
        protected abstract GameScene targetGameScene { get; }

        public BaseSceneManager() {}

        public virtual void BeforeLoadScene() {}
        public virtual void AfterLoadScene() {}
        public virtual void Dispose() {}
    }
}