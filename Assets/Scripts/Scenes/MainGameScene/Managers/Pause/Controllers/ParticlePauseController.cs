using System;
using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public class ParticlePauseController : BasePauseController {

        #region 非公開のプロパティ

        /// <summary>
        ///   ポーズ中のコンポーネントをすべて格納する配列
        /// </summary>
        protected ParticleSystem[] pausingComponents = null;

        /// <summary>
        ///   ポーズ中のコンポーネントの、ポーズ前に再生していたかどうかを保存しておく配列
        /// </summary>
        private bool[] pausingParticleIsPlaying = null;

        #endregion


        #region 公開する関数

        /// <summary>
        ///   コンポーネントをポーズ状態にする
        /// </summary>
        protected override void PauseComponents() {
            pausingComponents = Array.FindAll(
                pauseObjectParent.GetComponentsInChildren<ParticleSystem>(),
                obj => obj.isPlaying
            );
            pausingParticleIsPlaying = new bool[pausingComponents.Length];
            
            for(int i = 0; i < pausingComponents.Length; i++) {
                pausingParticleIsPlaying[i] = pausingComponents[i].isPlaying;
                pausingComponents[i].Pause();
            }
        }


        /// <summary>
        ///   コンポーネントのポーズ状態を解除する
        /// </summary>
        protected override void ResumeComponents() {
            for(int i = 0; i < pausingComponents.Length; i++) {
                if (pausingComponents[i] == null || !pausingParticleIsPlaying[i]) continue;
                pausingComponents[i].Play();
            }
        }

        #endregion

    }
}