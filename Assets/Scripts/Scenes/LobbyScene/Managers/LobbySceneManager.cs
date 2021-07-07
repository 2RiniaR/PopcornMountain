using System;
using UnityEngine;
using UniRx;


namespace PopcornMountain.LobbyScene {
    public sealed class LobbySceneManager : BaseSceneManager {

        protected override GameScene targetGameScene { get { return GameScene.LobbyScene; } }
        private const string bgmResourcePath = "Sounds/BGM/StartMenuSceneBGM";

        private BoolReactiveProperty isTransitionEnabled = new BoolReactiveProperty(true);
        public bool IsTransitionEnabled { get { return isTransitionEnabled.Value; } }
        public IObservable<bool> OnChangeIsTransitionEnabled { get { return isTransitionEnabled; } }

        private BoolReactiveProperty isConnecting = new BoolReactiveProperty(false);
        public bool IsConnecting { get { return isConnecting.Value; } }
        public IObservable<bool> OnChangeIsConnectingObservable { get { return isConnecting; } }


        public LobbySceneManager() {
        }

        public override void BeforeLoadScene() {
            AudioManager.Instance.ChangeBGM(Resources.Load(bgmResourcePath) as AudioClip);
        }


        public override void Dispose() {
            isTransitionEnabled.Dispose();
            BaseLobbyComponent.DisposeComponentsReference();
        }


        public void SetTransitionEnabled(bool isEnable) {
            isTransitionEnabled.SetValueAndForceNotify(isEnable);
        }

        public void SetConnecting(bool isConnecting) {
            this.isConnecting.Value = isConnecting;
        }

    }
}
