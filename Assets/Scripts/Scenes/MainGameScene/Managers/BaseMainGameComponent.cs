using UnityEngine;
using System;
using System.Collections.Generic;


namespace PopcornMountain.MainGameScene {
    public abstract class BaseMainGameComponent : MonoBehaviour {

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
