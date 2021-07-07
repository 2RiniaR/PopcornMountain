using System;
using System.Linq;
using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public sealed class MainGameSceneManager : BaseSceneManager {

        protected override GameScene targetGameScene { get { return GameScene.MainGameScene; } }
        private static readonly Type[] managersType = new Type[] {
            typeof(FireManager),      // 依存先:
            typeof(LidManager),       // 依存先:
            typeof(PlayerManager),    // 依存先:
            typeof(PhaseManager),     // 依存先: Player
            typeof(PauseManager),     // 依存先: Phase
            typeof(BeansManager),     // 依存先: Phase, Pause
            typeof(FaintManager),     // 依存先: Phase, Pause
            typeof(BGMManager),       // 依存先: Phase, Pause
            typeof(HandManager),      // 依存先: Beans
            typeof(SelectorManager),  // 依存先: Beans, Lid
            typeof(EventManager),     // 依存先: Beans, Lid
        };
        private BaseMainGameManager[] managers = null;

        public MainGameSceneManager() : base() {}


        /// <summary>
        ///   <para>シーンを初期化する</para>
        /// </summary>
        public override void BeforeLoadScene() {
            managers = new BaseMainGameManager[managersType.Length];
            for (int i = 0; i < managersType.Length; i++) {
                managers[i] = Activator.CreateInstance(managersType[i]) as BaseMainGameManager;
            }
        }


        public override void AfterLoadScene() {
            foreach (var manager in managers) {
                manager.AfterLoadScene();
            }
        }


        public override void Dispose() {
            foreach (var manager in managers.Reverse()) {
                manager.Dispose();
            }
            BaseMainGameComponent.DisposeComponentsReference();
            BaseMainGameManager.DisposeComponentsReference();
        }


        public T GetManagerComponent<T>() where T : BaseMainGameManager {
            var comp = Array.Find(managers, x => x is T) as T;
            return comp;
        }

    }
}
