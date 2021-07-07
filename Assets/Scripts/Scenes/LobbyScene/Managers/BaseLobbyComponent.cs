using UnityEngine;


namespace PopcornMountain.LobbyScene {
    public abstract class BaseLobbyComponent : MonoBehaviour {

        private static LobbySceneManager lobbySceneManager = null;
        protected static LobbySceneManager Manager {
            get { return lobbySceneManager ?? (lobbySceneManager = SceneManager.GetCurrentSceneManager<LobbySceneManager>()); }
        }

        public static void DisposeComponentsReference() {
            lobbySceneManager = null;
        }

    }
}
