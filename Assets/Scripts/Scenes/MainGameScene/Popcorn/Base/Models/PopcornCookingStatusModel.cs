using System;
using UniRx;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornCookingStatusModel : BaseMainGameComponent {

        #region 非公開のプロパティ

        /// <summary>
        ///   初期化時にのみOnNext()が発行されるSubject
        /// </summary>
        private AsyncSubject<float> initHeatSubject = new AsyncSubject<float>();

        /// <summary>
        ///   初期化時以外にOnNext()が発行されるSubject
        /// </summary>
        private Subject<float> changeHeatSubject = new Subject<float>();

        /// <summary>
        ///   ParameterModelへの参照
        /// </summary>
        private PopcornParameterModel parameterModel = null;

        #endregion


        #region 公開するプロパティ

        /// <summary>
        ///   現在の熱量
        /// </summary>
        public float CurrentHeat { get; private set; } = 0f;

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   現在の調理状態
        /// </summary>
        public IObservable<PopcornCookingStates> InitCookingStateObservable { get; private set; } = null;

        /// <summary>
        ///   現在の調理状態
        /// </summary>
        public PopcornCookingStates CurrentCookingState { get; private set; } = PopcornCookingStates.Bean;

        /// <summary>
        ///   「状態が初期化された瞬間」にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<float> OnHeatChangedObservable { get; private set; } = null;

        /// <summary>
        ///   Init()による初期化以外のタイミングで「熱量が変化した瞬間」にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<float> OnHeatChangedWithoutInitObservable { get; private set; } = null;

        /// <summary>
        ///   「ポップコーンが破裂した瞬間」にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnPoppedObservable { get; private set; } = null;

        /// <summary>
        ///   Init()による初期化以外のタイミングで「ポップコーンが破裂した瞬間」にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnPoppedWithoutInitObservable { get; private set; } = null;

        /// <summary>
        ///   「ポップコーンが破裂した瞬間」にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnBurnedObservable { get; private set; } = null;

        /// <summary>
        ///   Init()による初期化以外のタイミングで「ポップコーンが破裂した瞬間」にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnBurnedWithoutInitObservable { get; private set; } = null;

        #endregion


        #region 初期化用の関数

        // 実行順は
        //   Awake() --> Init() --> Start()
        //   ※ Init() は BeansEmitter クラスにより Instantiate() 直後に呼ばれる

        private void Awake() {
            // 熱量の変更が通知されたとき、現在の熱量を変更する
            Observable.Merge(initHeatSubject, changeHeatSubject)
                .Subscribe(x => CurrentHeat = x)
                .AddTo(this);

            // OnHeatChangedObservableを初期化する
            OnHeatChangedObservable = Observable
                .Merge(initHeatSubject, changeHeatSubject)
                .Publish()
                .RefCount();

            // OnHeatChangedWithoutInitObservableを初期化する
            OnHeatChangedWithoutInitObservable = changeHeatSubject;
        }


        public void Init(float initialHeat) {
            parameterModel = GetComponent<PopcornParameterModel>();
            var parameter = parameterModel.parameters;

            // InitCookingStateObservableを初期化する
            InitCookingStateObservable = initHeatSubject
                .Select(x => {
                    if (x >= parameter.blackHeat) return PopcornCookingStates.Burned;
                    else if (x >= parameter.popHeat) return PopcornCookingStates.Popped;
                    return PopcornCookingStates.Bean;
                })
                .PublishLast()
                .RefCount();

            // OnBurnedObservableを初期化する
            OnBurnedObservable = Observable
                .Merge(initHeatSubject, changeHeatSubject)
                .Where(x => x >= parameter.blackHeat)
                .First()
                .AsUnitObservable()
                .PublishLast()
                .RefCount();

            // OnPoppedObservableを初期化する
            OnPoppedObservable = Observable
                .Merge(initHeatSubject, changeHeatSubject)
                .Where(x => x >= parameter.popHeat)
                .First()
                .AsUnitObservable()
                .PublishLast()
                .RefCount();

            // OnBurnedWithoutInitObservableを初期化する
            OnBurnedWithoutInitObservable = Observable.Defer(() => {
                if (CurrentCookingState == PopcornCookingStates.Burned) {
                    return Observable.Empty<Unit>();
                } else {
                    return changeHeatSubject
                        .Where(x => x >= parameter.blackHeat)
                        .First()
                        .AsUnitObservable()
                        .PublishLast()
                        .RefCount();
                }
            });

            // OnPoppedWithoutInitObservableを初期化する
            OnPoppedWithoutInitObservable = Observable.Defer(() => {
                if (CurrentCookingState == PopcornCookingStates.Burned || CurrentCookingState == PopcornCookingStates.Popped) {
                    return Observable.Empty<Unit>();
                } else {
                    return changeHeatSubject
                        .Where(x => x >= parameter.popHeat)
                        .First()
                        .AsUnitObservable()
                        .PublishLast()
                        .RefCount();
                }
            });

            // 破裂したことが通知されたとき、現在の状態を破裂した状態に変更する
            OnPoppedObservable
                .Subscribe(_ => CurrentCookingState = PopcornCookingStates.Popped)
                .AddTo(this);

            // 焦げたことが通知されたとき、現在の状態を焦げた状態に変更する
            OnBurnedObservable
                .Subscribe(_ => CurrentCookingState = PopcornCookingStates.Burned)
                .AddTo(this);

            // 初期値を通知する
            initHeatSubject.OnNext(initialHeat);
            initHeatSubject.OnCompleted();
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   <para>熱量を加算する</para>
        /// </summary>
        /// <param name="heat">追加する熱量</param>
        public void AddHeat(float heat) {
            changeHeatSubject.OnNext(CurrentHeat + heat);
        }

        /// <summary>
        ///   <para>ポップコーンが破裂するまでの現在の熱量の割合(0~1)を返す</para>
        /// </summary>
        public float GetHeatPercentToPop() {
            return CurrentHeat / parameterModel.parameters.popHeat;
        }

        #endregion

    }
}
