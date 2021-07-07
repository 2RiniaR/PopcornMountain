using System;
using UniRx;
using Photon.Pun;


namespace PopcornMountain.MainGameScene {
    public sealed class EventSender : SingletonMonoBehaviour<EventSender> {

        #region 非公開のプロパティ

        /// <summary>
        ///   相手のクライアントからイベントを受信したときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<CookingEvent> onReceiveEvent = new Subject<CookingEvent>();

        /// <summary>
        ///   相手のクライアントからイベントを受信したときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<Unit> onReceiveCount = new Subject<Unit>();

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   相手のクライアントからイベントを受信したときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<CookingEvent> OnReceiveEventObservable { get { return onReceiveEvent; } }

        /// <summary>
        ///   相手のクライアントからイベントのカウントダウン開始通知を受信したときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnReceiveCountObservable { get { return onReceiveCount; } }

        #endregion


        #region 非公開の関数

        /// <summary>
        ///   相手クライアントから送られてきたイベントを受信する
        /// </summary>
        /// <remarks>
        ///   相手クライアントからRPCで呼び出される
        /// </remarks>
        /// <param name="eventType">イベントの種類</param>
        [PunRPC]
        private void ReceiveEvent(int eventType) {
            onReceiveEvent.OnNext((CookingEvent)Enum.ToObject(typeof(CookingEvent), eventType));
        }


        /// <summary>
        ///   相手クライアントから送られてきたイベントのカウントダウン開始通知を受信する
        /// </summary>
        /// <remarks>
        ///   相手クライアントからRPCで呼び出される
        /// </remarks>
        [PunRPC]
        private void ReceiveCount() {
            onReceiveCount.OnNext(Unit.Default);
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   イベントのカウントダウン開始通知をRPCで相手に送信する
        /// </summary>
        public void SendCount() {
            var view = PhotonView.Get(this);
            view.RPC(nameof(ReceiveCount), RpcTarget.All);
        }


        /// <summary>
        ///   イベントをRPCで相手に送信する
        /// </summary>
        /// <param name="eventType">送信するイベントの種類</param>
        public void SendEvent(CookingEvent eventType) {
            var view = PhotonView.Get(this);
            view.RPC(nameof(ReceiveEvent), RpcTarget.All, (int)eventType);
        }

        #endregion

    }
}
