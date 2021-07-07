using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public abstract class BasePauseController : BaseMainGameManager {

        #region 非公開のプロパティ

        /// <summary>
        ///   ポーズ対象のオブジェクトの親オブジェクトのタグ
        /// </summary>
        private static string pauseObjectParentTagName = "PausableObjectParent";

        /// <summary>
        ///   現在コンポーネントがポーズ中かどうか
        /// </summary>
        private bool isPause = false;

        /// <summary>
        ///   ポーズ対象のオブジェクトの親オブジェクト
        /// </summary>
        protected GameObject pauseObjectParent {
            get {
                return GameObject.FindGameObjectWithTag(pauseObjectParentTagName);
            }
        }

        #endregion


        #region 非公開の関数

        /// <summary>
        ///   コンポーネントのポーズ処理を行う
        /// </summary>
        protected abstract void PauseComponents();


        /// <summary>
        ///   コンポーネントのポーズ解除処理を行う
        /// </summary>
        protected abstract void ResumeComponents();

        #endregion


        #region 公開する関数

        /// <summary>
        ///   コンポーネントをポーズする
        /// </summary>
        public void Pause() {
            if (isPause) return;
            isPause = true;
            PauseComponents();
        }


        /// <summary>
        ///   コンポーネントのポーズを解除する
        /// </summary>
        public void Resume() {
            if (!isPause) return;
            isPause = false;
            ResumeComponents();
        }

        #endregion

    }
}