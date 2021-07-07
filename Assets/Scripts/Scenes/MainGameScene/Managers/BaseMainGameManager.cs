using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene {
    public abstract class BaseMainGameManager : IDisposable {

        public BaseMainGameManager() {
            BeforeLoadScene();
        }

        public virtual void BeforeLoadScene() {}
        public virtual void AfterLoadScene() {}
        public virtual void Dispose() {}


        private static Dictionary<Type, BaseMainGameManager> managerStore = new Dictionary<Type, BaseMainGameManager>();
        private static MainGameSceneManager mainGameSceneManager = null;
        private static MainGameSceneManager Manager {
            get { return mainGameSceneManager ?? (mainGameSceneManager = SceneManager.GetCurrentSceneManager<MainGameSceneManager>()); }
        }

        protected static T GetManager<T>() where T : BaseMainGameManager {
            if (managerStore.TryGetValue(typeof(T), out var manager)) {
                return manager as T;
            }
            manager = Manager.GetManagerComponent<T>();
            managerStore.Add(typeof(T), manager);
            return manager as T;
        }

        public static void DisposeComponentsReference() {
            managerStore.Clear();
            mainGameSceneManager = null;
        }

    }
}
