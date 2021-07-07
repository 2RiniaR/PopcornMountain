using System;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {

    /// <summary>
    ///   不定期イベント
    /// </summary>
    public enum CookingEvent {
        BomberRush,
        BlazeRush,
        GoldenRush,
        SelectorRush,
        DangerSpawn
    }


    public class EventManager : BaseMainGameManager {

        #region 定数

        /// <summary>
        ///   各イベント実行クラスのインスタンス
        /// </summary>
        private readonly BaseEventElement[] eventElements = new BaseEventElement[] {
            new BomberRushEvent(),
            new BlazeRushEvent(),
            new GoldenRushEvent(),
            new SelectorRushEvent(),
            new DangerSpawnEvent(),
        };


        #endregion


        #region 子コンポーネント

        private EventEmitter eventEmitter = null;

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   Subscribeしたパイプラインを一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   イベントが生成されたときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<CookingEvent> OnCookingEventObservable { get; private set; } = null;

        /// <summary>
        ///   イベントのカウントダウン開始時にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnEventCountObservable { get; private set; } = null;

        #endregion


        #region 初期化用の関数

        public EventManager() {
            var eventSender = GameObject.FindWithTag("PhotonView")?.GetComponent<EventSender>();
            OnCookingEventObservable = eventSender.OnReceiveEventObservable;
            OnEventCountObservable = eventSender.OnReceiveCountObservable;

            if (PhotonManager.IsHostPlayer()) {
                eventEmitter = new EventEmitter();

                eventEmitter.OnEmitCookingEventObservable
                    .Subscribe(type => eventSender.SendEvent(type))
                    .AddTo(onDispose);

                eventEmitter.OnCookingEventCountObservable
                    .Subscribe(_ => eventSender.SendCount())
                    .AddTo(onDispose);
            }
        }

        #endregion


        #region 終了処理用の関数

        public override void Dispose() {
            eventEmitter?.Dispose();
            onDispose.Dispose();
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   指定したイベントを実行する
        /// </summary>
        /// <param name="type">イベントの種類</param>
        public void RunEvent(CookingEvent type) {
            eventElements[(int)type].CreateRunEventObservable().Subscribe().AddTo(onDispose);
        }

        #endregion

    }
}
