using UnityEngine;


namespace PopcornMountain.StartScene {
    public abstract class BaseStartComponent : MonoBehaviour {

        private static StartSceneManager startSceneManager = null;
        protected static StartSceneManager Manager {
            get { return startSceneManager ?? (startSceneManager = SceneManager.GetCurrentSceneManager<StartSceneManager>()); }
        }

        public static void DisposeComponentsReference() {
            startSceneManager = null;
        }

    }
}
