using System;
using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public class RigidbodyPauseController : BasePauseController {

        /// <summary>
        ///   ポーズ解除後に復元するプロパティ
        /// </summary>
        private struct RigidbodyStoreProperty {
            public bool isSimulated;
            public Vector2 velocity;
            public float angularVeloccity;
            
            public RigidbodyStoreProperty(Rigidbody2D rigidbody) {
                isSimulated = rigidbody.simulated;
                velocity = rigidbody.velocity;
                angularVeloccity = rigidbody.angularVelocity;
            }
        }


        #region 非公開のプロパティ

        /// <summary>
        ///   ポーズ中のコンポーネントをすべて格納する配列
        /// </summary>
        protected Rigidbody2D[] pausingComponents = null;

        /// <summary>
        ///   ポーズ中のコンポーネントの、ポーズ前のプロパティを保存しておく配列
        /// </summary>
        private RigidbodyStoreProperty[] pausingRigidbodyVelocity = null;

        #endregion


        #region 公開する関数

        /// <summary>
        ///   コンポーネントをポーズ状態にする
        /// </summary>
        protected override void PauseComponents() {
            pausingComponents = Array.FindAll(
                pauseObjectParent.GetComponentsInChildren<Rigidbody2D>(),
                obj => !obj.IsSleeping()
            );
            pausingRigidbodyVelocity = new RigidbodyStoreProperty[pausingComponents.Length];
            
            for(int i = 0; i < pausingComponents.Length; i++) {
                pausingRigidbodyVelocity[i] = new RigidbodyStoreProperty(pausingComponents[i]);
                pausingComponents[i].simulated = false;
            }
        }


        /// <summary>
        ///   コンポーネントのポーズ状態を解除する
        /// </summary>
        protected override void ResumeComponents() {
            for(int i = 0; i < pausingComponents.Length; i++) {
                if (pausingComponents[i] == null) continue;
                pausingComponents[i].simulated = pausingRigidbodyVelocity[i].isSimulated;
                pausingComponents[i].velocity = pausingRigidbodyVelocity[i].velocity;
                pausingComponents[i].angularVelocity = pausingRigidbodyVelocity[i].angularVeloccity;
            }
        }

        #endregion

    }
}