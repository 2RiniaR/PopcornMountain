using System;
using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public class AnimatorPauseController : BasePauseController {

        #region 非公開のプロパティ

        /// <summary>
        ///   ポーズ中のコンポーネントをすべて格納する配列
        /// </summary>
        protected Animator[] pausingComponents = null;

        /// <summary>
        ///   ポーズ中のコンポーネントの、ポーズ前のアニメーションスピードを保存しておく配列
        /// </summary>
        private float[] pausingAnimatorSpeed = null;

        #endregion


        #region 公開する関数

        /// <summary>
        ///   コンポーネントをポーズ状態にする
        /// </summary>
        protected override void PauseComponents() {
            pausingComponents = Array.FindAll(
                pauseObjectParent.GetComponentsInChildren<Animator>(),
                obj => obj.speed != 0
            );
            pausingAnimatorSpeed = new float[pausingComponents.Length];
            
            for(int i = 0; i < pausingComponents.Length; i++) {
                pausingAnimatorSpeed[i] = pausingComponents[i].speed;
                pausingComponents[i].speed = 0;
            }
        }


        /// <summary>
        ///   コンポーネントのポーズ状態を解除する
        /// </summary>
        protected override void ResumeComponents() {
            for(int i = 0; i < pausingComponents.Length; i++) {
                if (pausingComponents[i] == null) continue;
                pausingComponents[i].speed = pausingAnimatorSpeed[i];
            }
        }

        #endregion

    }
}