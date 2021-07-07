using System;
using UnityEngine;
using UniRx;
using PopcornMountain.MainGameScene.Popcorn;


namespace PopcornMountain.MainGameScene {
    public class BeansManager : BaseMainGameManager {

        #region 子コンポーネント

        private BeansCounter counter = null;
        private BeansEmitter emitter = new BeansEmitter();
        private BeansGenerator generator = new BeansGenerator();
        private BeansSender sender = null;
        private BeansCollector collector = new BeansCollector();
        private PauseManager pauseManager = GetManager<PauseManager>();

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   Subscribeしたパイプラインを一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        private bool isOverEmission = false;

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   豆が相手のクライアントへ送られたときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Tuple<PopcornParameters, float>> OnSendBeans { get; private set; } = null;

        /// <summary>
        ///   豆を相手のクライアントから受け取ったときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Tuple<PopcornParameters, float>> OnReceiveBeans {
            get {
                return sender.OnReceiveBeansObservable;
            }
        }

        /// <summary>
        ///   豆が取得されたときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<GameObject> OnCatchedBeans {
            get {
                return collector.OnCatched;
            }
        }


        /// <summary>
        ///   豆が取得されたときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<GameObject> OnHeldBeans {
            get {
                return collector.OnHeldBeans;
            }
        }


        /// <summary>
        ///   豆が取得されたときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<GameObject> OnReleasedBeans {
            get {
                return collector.OnReleaseBeans;
            }
        }

        #endregion


        #region 初期化用の関数

        public BeansManager() : base() {
            // senderを初期化する
            sender = GameObject.FindWithTag("PhotonView").GetComponent<BeansSender>();

            // OnSendBeansを初期化する
            OnSendBeans = collector.OnFlown
                .Select(x => new Tuple<PopcornParameters, float>(
                    x.GetComponent<PopcornParameterModel>().parameters,
                    x.GetComponent<PopcornCookingStatusModel>().CurrentHeat
                ))
                .Publish()
                .RefCount();

            // 生成された豆をインスタンス化する
            emitter.OnEmitPopcornObservable
                .Where(_ => !pauseManager.IsPause)
                .Where(_ => !isOverEmission)
                .Do(_ => sender.SendPopcornCountDifference(1))
                .Subscribe(x => generator.AddGenerationBuffer(x.Item1, x.Item2))
                .AddTo(onDispose);

            // 豆が場外に出たとき、相手プレイヤーへ送信する
            OnSendBeans
                .Where(_ => !pauseManager.IsPause)
                .Subscribe(x => sender.SendBean(x.Item1, x.Item2))
                .AddTo(onDispose);

            // 豆がインスタンス化されたとき、Observableに登録する
            generator.OnInstantiateBeansObservable
                .Subscribe(x => collector.RegistBeans(x))
                .AddTo(onDispose);

            // 豆が取得された、もしくは落下判定になったとき、カウントを減らす
            Observable.Merge(collector.OnFallen.Do(_ => Debug.Log("Fallen")), collector.OnCatched.Do(_ => Debug.Log("Caught")), collector.OnBurned.Do(_ => Debug.Log("Burned")))
                .Subscribe(_ => sender.SendPopcornCountDifference(-1))
                .AddTo(onDispose);

            sender.OnReceiveIsOverEmissionObsrvable
                .Subscribe(x => isOverEmission = x)
                .AddTo(onDispose);

            if (PhotonManager.IsHostPlayer()) {
                counter = new BeansCounter();

                sender.OnReceivePopcornCountObservableDifference
                    .Subscribe(x => counter.AddCount(x))
                    .AddTo(onDispose);

                counter.IsOverEmissionChangedObservable
                    .Subscribe(x => sender.SendIsOverEmission(x))
                    .AddTo(onDispose);
            }

            // Debug
            // Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha1)).AsUnitObservable().Subscribe(_ => EmitBean(PopcornType.Normal)).AddTo(onDispose);
            // Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha2)).AsUnitObservable().Subscribe(_ => EmitBean(PopcornType.Hard)).AddTo(onDispose);
            // Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha3)).AsUnitObservable().Subscribe(_ => EmitBean(PopcornType.Bomber)).AddTo(onDispose);
            // Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha4)).AsUnitObservable().Subscribe(_ => EmitBean(PopcornType.Golden)).AddTo(onDispose);
            // Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha5)).AsUnitObservable().Subscribe(_ => EmitBean(PopcornType.Danger)).AddTo(onDispose);
            // Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha6)).AsUnitObservable().Subscribe(_ => EmitBean(PopcornType.Blaze)).AddTo(onDispose);
            // Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha7)).AsUnitObservable().Subscribe(_ => EmitBean(PopcornType.Soggy)).AddTo(onDispose);
            // Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Alpha8)).AsUnitObservable().Subscribe(_ => EmitBean(PopcornType.Troll)).AddTo(onDispose);
        }


        public override void AfterLoadScene() {
            generator.AfterLoadScene();
        }

        #endregion


        #region 終了処理用の関数

        public override void Dispose() {
            emitter.Dispose();
            generator.Dispose();
            collector.Dispose();
            counter?.Dispose();
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   指定した種類の豆を生成する
        /// </summary>
        /// <param name="type">生成する豆の種類</param>
        public void EmitBean(PopcornType type) {
            emitter.EmitBean(type);
        }


        public void InstantiatePopcorn(
            PopcornParameters parameters,
            float initialHeat = 0f,
            Vector2? initialVelocity = null
        ) {
            if (pauseManager.IsPause) return;
            generator.AddGenerationBuffer(parameters, initialHeat: initialHeat, initialVelocity: initialVelocity);
        }

        #endregion

    }
}
