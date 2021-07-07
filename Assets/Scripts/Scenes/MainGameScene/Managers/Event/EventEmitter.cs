using System;
using System.Linq;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class EventEmitter : BaseMainGameManager {

        #region 定数

        /// <summary>
        ///   イベントが発生するまでの最長時間(s)
        /// </summary>
        private const float maxIntervalTime = 60f;
        // private const float maxIntervalTime = 1000001f;

        /// <summary>
        ///   イベントが発生するまでの最短時間(s)
        /// </summary>
        private const float minIntervalTime = 40f;
        // private const float minIntervalTime = 1000000f;

        #endregion


        #region コンポーネント参照

        private PhaseManager phaseManager = GetManager<PhaseManager>();
        private PauseManager pauseManager = GetManager<PauseManager>();

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   Subscribeしたパイプラインを一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        /// <summary>
        ///   イベントが生成されたときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<CookingEvent> emitCookingEventSubject = new Subject<CookingEvent>();

        /// <summary>
        ///   イベントのカウントダウン開始時にOnNext()が発行されるSubject
        /// </summary>
        private Subject<Unit> cookingEventCountSubject = new Subject<Unit>();

        /// <summary>
        ///   各イベントの発生確率
        /// </summary>
        private float[] cookingEventProbablities = null;

        /// <summary>
        ///   イベント発生までの残り時間(s)
        /// </summary>
        private FloatReactiveProperty eventEmitLeftTime = new FloatReactiveProperty(minIntervalTime);

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   イベントが生成されたときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<CookingEvent> OnEmitCookingEventObservable { get { return emitCookingEventSubject; } }

        /// <summary>
        ///   イベントのカウントダウン開始時にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnCookingEventCountObservable { get; private set; } = null;

        #endregion


        #region 初期化用の関数

        public EventEmitter() : base() {
            InitProbablities();
            InitCookingEventObservable();
        }

        /// <summary>
        ///   各イベントの生成確率を初期化する
        /// </summary>
        private void InitProbablities() {
            cookingEventProbablities = new float[Enum.GetValues(typeof(CookingEvent)).Length];
            for (int i = 0; i < cookingEventProbablities.Length; i++) {
                cookingEventProbablities[i] = 1f;
            }
        }


        /// <summary>
        ///   一定時間毎にイベントを生成するObservableを初期化する
        /// </summary>
        private void InitCookingEventObservable() {
            // 毎フレーム、カウントを減らしていく
            Observable.EveryUpdate()
                .Where(_ => !pauseManager.IsPause)
                .Where(_ => phaseManager.CurrentGamePhase == GamePhase.Cooking)
                .Subscribe(_ => {
                    eventEmitLeftTime.Value = Mathf.Max(0f, eventEmitLeftTime.Value - Time.deltaTime);
                    if (eventEmitLeftTime.Value <= 0f) {
                        eventEmitLeftTime.Value = UnityEngine.Random.Range(minIntervalTime, maxIntervalTime);
                        EmitCookingEvent(SelectEvent(cookingEventProbablities));
                    }
                })
                .AddTo(onDispose);

            // 5秒前になったとき、OnCookingEventCountObservableにOnNext()を発行する
            OnCookingEventCountObservable = eventEmitLeftTime
                .Pairwise()
                .Where(x => x.Previous > 5f && x.Current <= 5f)
                .AsUnitObservable()
                .Publish()
                .RefCount();
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
        private CookingEvent SelectEvent(float[] plobabilities) {
            const int defaultReturnValue = 0;
            float randomValue = UnityEngine.Random.Range(0f, plobabilities.Sum());
            float lockValue = 0f;
            int result = 0;
            for (result = 0; result < plobabilities.Length; result++) {
                lockValue += plobabilities[result];
                if(randomValue < lockValue) {
                    return (CookingEvent)Enum.ToObject(typeof(CookingEvent), result);
                };
            }
            return (CookingEvent)Enum.ToObject(typeof(CookingEvent), defaultReturnValue);
        }


        /// <summary>
        ///   生成したイベントから発生確率を調整する
        /// </summary>
        /// <param name="type">生成したイベントの種類</param>
        private void UpdateProbablitiesWithEmit(CookingEvent type) {
            for (int i = 0; i < cookingEventProbablities.Length; i++) {
                cookingEventProbablities[i] *= ((int)type == i) ? 0.5f : 2f;
            }
        }


        /// <summary>
        ///   イベントを生成する
        /// </summary>
        /// <param name="type">生成するイベントの種類</param>
        private void EmitCookingEvent(CookingEvent type) {
            emitCookingEventSubject.OnNext(type);
            UpdateProbablitiesWithEmit(type);
        }

        #endregion

    }
}
