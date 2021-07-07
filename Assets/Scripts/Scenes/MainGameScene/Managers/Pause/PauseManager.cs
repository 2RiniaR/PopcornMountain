using System;
using UniRx;
using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public class PauseManager : BaseMainGameManager {

        #region 定数

        /// <summary>
        ///   ポーズ画面を開いたときに、ゲームが停止するようにするかどうか
        /// </summary>
        public const bool IsPausableOnShowPauseWindow = false;

        #endregion


        #region 子コンポーネント

        /// <summary>
        ///   対象GameObjectの各コンポーネントをポーズ状態にするコンポーネント
        /// </summary>
        private readonly BasePauseController[] pauseControllers = {
            new AnimatorPauseController(),
            new ParticlePauseController(),
            new RigidbodyPauseController(),
            new AudioPauseController()
        };

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   現在ポーズ状態であるかどうかのReactiveProperty
        /// </summary>
        private BoolReactiveProperty isPauseObservable = new BoolReactiveProperty(false);

        /// <summary>
        ///   Subscribeしたパイプラインを一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        #endregion


        #region 公開するプロパティ

        /// <summary>
        ///   現在ポーズ状態であるかどうか
        /// </summary>
        public bool IsPause { get { return isPauseObservable.Value; } }

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   ポーズ状態になったときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnPauseObservable { get; private set; }

        /// <summary>
        ///   ポーズ状態が解除されたときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnResumeObservable { get; private set; }

        #endregion


        #region 初期化用の関数

        public PauseManager() {
            // OnPauseObservableを初期化する
            OnPauseObservable = isPauseObservable
                .Where(isPause => isPause)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            // OnResumeObservableを初期化する
            OnResumeObservable = isPauseObservable
                .Where(isPause => !isPause)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            // ポーズ状態になったとき、対象GameObjectのコンポーネントをポーズ状態にする
            OnPauseObservable.Subscribe(_ => {
                foreach(var controller in pauseControllers) {
                    controller.Pause();
                }
            }).AddTo(onDispose);

            // ポーズ状態が解除されたとき、対象GameObjectのコンポーネントをポーズ状態を解除する
            OnResumeObservable.Subscribe(_ => {
                foreach(var controller in pauseControllers) {
                    controller.Resume();
                }
            }).AddTo(onDispose);

            // ゲーム終了時、ゲームをポーズする
            var phaseManager = GetManager<PhaseManager>();
            phaseManager.OnExitPhase(GamePhase.Cooking)
                .Subscribe(_ => PauseGame())
                .AddTo(onDispose);
        }

        #endregion


        #region 終了処理用の関数

        public override void Dispose() {
            onDispose.Dispose();
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   ゲームをポーズ状態にする
        /// </summary>
        public void PauseGame() {
            isPauseObservable.Value = true;
        }

        /// <summary>
        ///   ゲームのポーズ状態を解除する
        /// </summary>
        public void ResumeGame() {
            isPauseObservable.Value = false;
        }

        #endregion

    }
}
