using System;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class FaintManager : BaseMainGameManager {

        #region コンポーネント参照

        private PauseManager pauseManager = GetManager<PauseManager>();
        private PhaseManager phaseManager = GetManager<PhaseManager>();

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   気絶の残り時間(s)のReactiveProperty
        /// </summary>
        private FloatReactiveProperty faintingTimeLeft = new FloatReactiveProperty(0);

        /// <summary>
        ///   Subscribeしたパイプラインを一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        #endregion


        #region 公開するプロパティ

        /// <summary>
        ///   気絶の残り時間(s)
        /// </summary>
        public float FaintingTimeLeft { get { return faintingTimeLeft.Value; } }

        /// <summary>
        ///   現在気絶しているかどうか
        /// </summary>
        /// <value></value>
        public bool IsFainting { get { return FaintingTimeLeft > 0; } }

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   気絶状態の残り時間が変化したときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<float> OnChangeFaintingTimeLeftObservable { get { return faintingTimeLeft; } }

        /// <summary>
        ///   気絶状態のON/OFFが切り替わったときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<bool> OnChangeIsFaintingObservable { get; private set; }

        #endregion


        #region 初期化用の関数

        public override void BeforeLoadScene() {
            // OnChangeIsFaintingObservableを初期化する
            OnChangeIsFaintingObservable = OnChangeFaintingTimeLeftObservable
                .Pairwise()
                .Where(x => (x.Previous == 0) ^ (x.Current == 0))
                .Select(x => x.Current > 0)
                .Publish()
                .RefCount();

            // 毎フレーム、気絶の残り時間を減らす
            Observable.EveryUpdate()
                .Where(_ => !pauseManager.IsPause)
                .Subscribe(x => faintingTimeLeft.Value = Mathf.Max(0f, FaintingTimeLeft - Time.deltaTime))
                .AddTo(onDispose);

            // 料理フェーズから出たとき、気絶状態を解除する
            phaseManager.OnExitPhase(GamePhase.Cooking)
                .Subscribe(_ => Cure())
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
        ///   気絶状態を回復する
        /// </summary>
        public void Cure() {
            faintingTimeLeft.Value = 0;
        }


        /// <summary>
        ///   一定時間、気絶状態にする
        /// </summary>
        /// <param name="sec">気絶状態にする秒数</param>
        public void Faint(float sec) {
            faintingTimeLeft.Value = Mathf.Max(FaintingTimeLeft, sec);
        }

        #endregion

    }
}
