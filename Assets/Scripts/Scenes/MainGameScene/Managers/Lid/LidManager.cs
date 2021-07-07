using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public class LidManager : BaseMainGameManager {

        #region 定数

        /// <summary>
        ///   蓋を生成するワールド座標
        /// </summary>
        private static readonly Vector3 defaultGeneratePosition = new Vector3(0, 7, -2);

        /// <summary>
        ///   蓋を生成する回転
        /// </summary>
        private static readonly Quaternion defaultGenerateRotation = Quaternion.identity;

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   蓋のGameObjectのPrefab
        /// </summary>
        private GameObject lidPrefab = null;

        #endregion


        #region 初期化用の関数

        public LidManager() : base() {
            // 蓋のPrefabを読み込む
            lidPrefab = Resources.Load("Prefabs/MainGameScene/Lid") as GameObject;
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   <para>鍋蓋のインスタンスを生成する</para>
        /// </summary>
        public void GenerateLid() {
            var lidParentObject = GameObject.FindWithTag("LidParent");
            UnityEngine.Object.Instantiate(lidPrefab, defaultGeneratePosition, defaultGenerateRotation, lidParentObject.transform);
        }

        #endregion

    }
}
