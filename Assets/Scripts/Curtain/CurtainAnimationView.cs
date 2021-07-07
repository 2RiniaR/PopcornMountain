using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.Curtain {
    public class CurtainAnimationView : SingletonMonoBehaviour<CurtainAnimationView> {

        #region 定数

        /// <summary>
        ///   Resourcesフォルダを基準とした、当ゲームオブジェクトのPrefabへのパス
        /// </summary>
        private const string prefabResourcePath = "Prefabs/UI/CurtainCanvas";

        /// <summary>
        ///   カーテンが開くAnimationStateのHash値
        /// </summary>
        private static readonly int openCurtainStateHash = Animator.StringToHash("Base Layer.Open");

        /// <summary>
        ///   カーテンが閉じるAnimationStateのHash値
        /// </summary>
        private static readonly int closeCurtainStateHash = Animator.StringToHash("Base Layer.Close");

        #endregion


        #region コンポーネント参照
        
        [SerializeField] private Animator sceneAnimator;
        [SerializeField] private Slider progressBar;
        private ObservableStateMachineTrigger animationTrigger;
        
        #endregion


        #region 初期化用の関数

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitInstantiateObject() {
            // ゲーム開始時にオブジェクトを生成する
            UnityEngine.Object.Instantiate(Resources.Load(prefabResourcePath) as GameObject);
        }

        private void OnEnable() {
            animationTrigger = sceneAnimator.GetBehaviour<ObservableStateMachineTrigger>();
        }

        #endregion


        #region 非公開の関数

        /// <summary>
        ///   進捗度バーの値を設定する
        /// </summary>
        /// <param name="value">現在の値</param>
        /// <param name="maxValue">最大値</param>
        private void SetProgressBarValue(float value, float maxValue = 1.0f) {
            progressBar.maxValue = maxValue;
            progressBar.value = value;
        }

        /// <summary>
        ///   進捗度バーを表示する
        /// </summary>
        /// <param name="isInitZero">trueの場合、同時に値を0で初期化する</param>
        private void ShowProgressBar(bool isInitZero = false) {
            progressBar.gameObject.SetActive(true);
            if (isInitZero) SetProgressBarValue(0f);
        }

        /// <summary>
        ///   進捗度バーを非表示にする
        /// </summary>
        private void HideProgressBar() {
            progressBar.gameObject.SetActive(false);
        }

        /// <summary>
        ///   カーテンを閉じるアニメーションを再生する
        /// </summary>
        private void PlayCloseCurtainAnimation() {
            sceneAnimator.SetBool("isOpen", false);
        }

        /// <summary>
        ///   カーテンを開くアニメーションを再生する
        /// </summary>
        private void PlayOpenCurtainAnimation() {
            sceneAnimator.SetBool("isOpen", true);
        }

        /// <summary>
        ///   カーテンを閉じるアニメーションを再生するObservableを返す
        /// </summary>
        /// <remarks>
        ///   <para>Subscribe時、カーテンを閉じるアニメーションが再生される</para>
        ///   <para>カーテンを閉じるアニメーションが終了すると、即座にOnCompleted()が発行される</para>
        /// </remarks>
        private IObservable<Unit> CreateClosingAnimationObservable() {
            return animationTrigger.OnStateExitAsObservable()
                .DoOnSubscribe(PlayCloseCurtainAnimation)
                .Where(state => state.StateInfo.fullPathHash == closeCurtainStateHash)
                .First()
                .IgnoreElements()
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }

        /// <summary>
        ///   カーテンを開くアニメーションを再生するObservableを返す
        /// </summary>
        /// <remarks>
        ///   <para>Subscribe時、カーテンを開くアニメーションが再生される</para>
        ///   <para>カーテンを開くアニメーションが終了すると、即座にOnCompleted()が発行される</para>
        /// </remarks>
        private IObservable<Unit> CreateOpeningAnimationObservable() {
            return animationTrigger.OnStateExitAsObservable()
                .DoOnSubscribe(PlayOpenCurtainAnimation)
                .Where(state => state.StateInfo.fullPathHash == openCurtainStateHash)
                .First()
                .IgnoreElements()
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   カーテンを閉じるアニメーションを再生し、任意の処理の完了後にカーテンを開くアニメーションを再生するObservableを返す
        /// </summary>
        /// <remarks>
        ///   <para>Subscribe時、カーテンを閉じるアニメーションが再生される</para>
        ///   <para>カーテンを開くアニメーションが終了すると、即座にOnCompleted()が発行される</para>
        /// </remarks>
        /// <param name="processObservable">
        ///   <para>カーテンが閉じている間に処理を行い、処理の終了時にOnCompleted()が発行されるObservable</para>
        ///   <para>OnNext()で0..1の値を発行すると、進捗バーの値が変化する</para>
        ///   <para>nullを指定すると、カーテンが閉じた後に何も実行されずカーテンが開く</para>
        /// </param>
        public IObservable<Unit> CreateCurtainAnimationObservable(IObservable<float> processObservable = null) {
            if (processObservable == null) {
                processObservable = Observable.Empty<float>();
            }

            // 読み込み開始時にプログレスバーを表示し、終了時に非表示にする
            var progressBarObservable = processObservable
                .DoOnSubscribe(() => ShowProgressBar(isInitZero: true))
                .Do(progress => SetProgressBarValue(progress))
                .DoOnCompleted(HideProgressBar)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            return Observable.Concat(
                CreateClosingAnimationObservable(),
                progressBarObservable,
                CreateOpeningAnimationObservable()
            )
            .IgnoreElements()
            .Publish()
            .RefCount();
        }

        #endregion

    }
}
