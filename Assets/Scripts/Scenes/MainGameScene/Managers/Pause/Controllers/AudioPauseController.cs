using System;
using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public class AudioPauseController : BasePauseController {

        #region 非公開のプロパティ

        /// <summary>
        ///   ポーズ中のコンポーネントをすべて格納する配列
        /// </summary>
        protected AudioSource[] pausingComponents = null;

        /// <summary>
        ///   ポーズ中のコンポーネントの、ポーズ前に再生していたかを保存しておく配列
        /// </summary>
        private bool[] pausingAudioIsPlaying = null;

        #endregion


        #region 公開する関数

        /// <summary>
        ///   コンポーネントをポーズ状態にする
        /// </summary>
        protected override void PauseComponents() {
            pausingComponents = Array.FindAll(
                pauseObjectParent.GetComponentsInChildren<AudioSource>(),
                obj => obj.isPlaying
            );
            pausingAudioIsPlaying = new bool[pausingComponents.Length];

            for(int i = 0; i < pausingComponents.Length; i++) {
                pausingAudioIsPlaying[i] = pausingComponents[i].isPlaying;
                pausingComponents[i].Pause();
            }
        }


        /// <summary>
        ///   コンポーネントのポーズ状態を解除する
        /// </summary>
        protected override void ResumeComponents() {
            for(int i = 0; i < pausingComponents.Length; i++) {
                if (pausingComponents[i] == null) continue;
                if (pausingAudioIsPlaying[i]) pausingComponents[i].UnPause();
            }
        }

        #endregion

    }
}