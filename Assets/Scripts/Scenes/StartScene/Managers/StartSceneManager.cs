using UnityEngine;
using UniRx;
using System;


namespace PopcornMountain.StartScene {
    public sealed class StartSceneManager : BaseSceneManager {

        protected override GameScene targetGameScene { get { return GameScene.MainGameScene; } }
        private const string bgmResourcePath = "Sounds/BGM/StartMenuSceneBGM";

        private BoolReactiveProperty isTransitionEnabled = new BoolReactiveProperty(true);
        public bool IsTransitionEnabled { get { return isTransitionEnabled.Value; } }
        public IObservable<bool> OnChangeIsTransitionEnabled { get { return isTransitionEnabled; } }

        private BoolReactiveProperty isConnecting = new BoolReactiveProperty(false);
        public bool IsConnecting { get { return isConnecting.Value; } }
        public IObservable<bool> OnChangeIsConnectingObservable { get { return isConnecting; } }


        public StartSceneManager() {
        }

        public override void BeforeLoadScene() {
            AudioManager.Instance.ChangeBGM(Resources.Load(bgmResourcePath) as AudioClip);
        }

        public override void Dispose() {
            isTransitionEnabled.Dispose();
            BaseStartComponent.DisposeComponentsReference();
        }


        public void SetTransitionEnabled(bool isEnable) {
            isTransitionEnabled.SetValueAndForceNotify(isEnable);
        }

        public void SetConnecting(bool isConnecting) {
            this.isConnecting.Value = isConnecting;
        }

    }
}
