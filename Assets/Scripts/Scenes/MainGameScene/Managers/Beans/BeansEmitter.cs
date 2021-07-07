using System;
using System.Linq;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class BeansEmitter : BaseMainGameManager {

        #region 定数

        /// <summary>
        ///   ポップコーンが自然生成されるまでの最長時間(s)
        /// </summary>
        private const float maxIntervalTime = 12f;
        // private const float maxIntervalTime = 1000001f;


        /// <summary>
        ///   ポップコーンが自然生成されるまでの最短時間(s)
        /// </summary>
        private const float minIntervalTime = 8f;
        // private const float minIntervalTime = 1000000f;

        #endregion


        #region コンポーネント参照

        private PhaseManager phaseManager = GetManager<PhaseManager>();
        private PauseManager pauseManager = GetManager<PauseManager>();

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   ポップコーンが生成されたときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<Tuple<PopcornType, float>> onEmitPopcornSubject = new Subject<Tuple<PopcornType, float>>();

        /// <summary>
        ///   Subscribeしたパイプラインを一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        /// <summary>
        ///   現在の各豆の生成確率
        /// </summary>
        private float[] beansGeneratePlobabilities = null;

        /// <summary>
        ///   イベント発生までの残り時間(s)
        /// </summary>
        private FloatReactiveProperty popcornEmitLeftTime = new FloatReactiveProperty(minIntervalTime);

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   ポップコーンが生成されたときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Tuple<PopcornType, float>> OnEmitPopcornObservable { get { return onEmitPopcornSubject; }}

        #endregion


        #region 初期化用の関数

        public BeansEmitter() {
            InitPlobabilities();
            InitGenerateBeansTimer();
        }


        /// <summary>
        ///   各豆の生成確率を初期化する
        /// </summary>
        private void InitPlobabilities() {
            beansGeneratePlobabilities = new float[] {49, 15, 10, 10, 1, 5, 5, 5};
        }


        /// <summary>
        ///   一定時間毎にポップコーンを生成するObservableを初期化する
        /// </summary>
        private void InitGenerateBeansTimer() {
            // 毎フレーム、カウントを減らしていく
            // 残り時間が0になったとき、残り時間をランダムな値にセットしてポップコーンを生成する
            Observable.EveryUpdate()
                .Where(_ => !pauseManager.IsPause)
                .Where(_ => phaseManager.CurrentGamePhase == GamePhase.Cooking)
                .Subscribe(_ => {
                    popcornEmitLeftTime.Value = Mathf.Max(0f, popcornEmitLeftTime.Value - Time.deltaTime);
                    if (popcornEmitLeftTime.Value <= 0f) {
                        popcornEmitLeftTime.Value = UnityEngine.Random.Range(minIntervalTime, maxIntervalTime);
                        EmitBean(SelectBean(beansGeneratePlobabilities));
                    }
                })
                .AddTo(onDispose);
        }

        #endregion


        #region 終了処理用の関数

        public override void Dispose() {
            onDispose.Dispose();
        }

        #endregion


        #region 非公開の関数

        /// <summary>
        ///   確率分布に従った抽選結果を返す
        /// </summary>
        /// <param name="plobabilities">確率分布の配列</param>
        private PopcornType SelectBean(float[] plobabilities) {
            const int defaultReturnValue = 0;
            float randomValue = UnityEngine.Random.Range(0f, plobabilities.Sum());
            float lockValue = 0f;
            int result = 0;
            for (result = 0; result < plobabilities.Length; result++) {
                lockValue += plobabilities[result];
                if(randomValue < lockValue) {
                    return (PopcornType)Enum.ToObject(typeof(PopcornType), result);
                };
            }
            return (PopcornType)Enum.ToObject(typeof(PopcornType), defaultReturnValue);
        }


        /// <summary>
        ///   ランダムなシード値を返す
        /// </summary>
        private float GenerateSeedValue() {
            return 0.75f + UnityEngine.Random.Range(0f, 0.5f);
        }


        /// <summary>
        ///   指定した種類・シード値の豆を生成する
        /// </summary>
        private void EmitBean(PopcornType type, float seed) {
            onEmitPopcornSubject.OnNext(new Tuple<PopcornType, float>(type, seed));
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   ランダムな豆を生成する
        /// </summary>
        public void EmitBean() {
            EmitBean(SelectBean(beansGeneratePlobabilities), GenerateSeedValue());
        }

        /// <summary>
        ///   指定した種類の豆を生成する
        /// </summary>
        public void EmitBean(PopcornType type) {
            EmitBean(type, GenerateSeedValue());
        }

        #endregion

    }
}
