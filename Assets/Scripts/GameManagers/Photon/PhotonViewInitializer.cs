using UnityEngine;
using Photon.Pun;


namespace PopcornMountain {
    public static class PhotonViewInitializer {

        private const string photonViewPrefabPath = "Prefabs/Photon/PhotonView";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitInstantiateObject() {
            // ゲーム開始時にオブジェクトを生成する
            var instance = GameObject.Instantiate(Resources.Load(photonViewPrefabPath) as GameObject);

            // PhotonView IDを指定する
            var photonView = instance.GetComponent<PhotonView>();
            photonView.ViewID = 1;
        }

    }
}
