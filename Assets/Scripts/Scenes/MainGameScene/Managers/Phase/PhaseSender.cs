using System;
using UniRx;
using Photon.Pun;


namespace PopcornMountain.MainGameScene {
    public sealed class PhaseSender : SingletonMonoBehaviour<PhaseSender> {

        #region 定数

        /// <summary>
        ///   「相手プレイヤー側で、カーテンが開き終わったとき」にRPCで送信するコード
        /// </summary>
        private const int finishSceneAnimationKey = 0;

        /// <summary>
        ///   「相手プレイヤー側で、着火アニメーションが終わったとき」にRPCで送信するコード
        /// </summary>
        private const int finishStartAnimationKey = 1;

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   <para>「相手プレイヤー側で、カーテンが開き終わったとき」に、OnNext()とOnCompleted()が発行されるSubject</para>
        /// </summary>
        private Subject<Unit> onFinishStartAnimationSubject = new Subject<Unit>();

        /// <summary>
        ///   <para>「相手プレイヤー側で、着火アニメーションが終わったとき」に、OnNext()とOnCompleted()が発行されるSubject</para>
        /// </summary>
        private Subject<Unit> onFinishSceneAnimationSubject = new Subject<Unit>();

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   <para>「相手プレイヤー側で、カーテンが開き終わったとき」に、OnNext()とOnCompleted()が発行されるObservable</para>
        /// </summary>
        public IObservable<Unit> OnFinishSceneAnimationObservable { get { return onFinishSceneAnimationSubject; } }

        /// <summary>
        ///   <para>「相手プレイヤー側で、着火アニメーションが終わったとき」に、OnNext()とOnCompleted()が発行されるObservable</para>
        /// </summary>
        public IObservable<Unit> OnFinishStartAnimationObservable { get { return onFinishStartAnimationSubject; } }

        #endregion


        #region 非公開の関数

        /// <summary>
        ///   相手クライアントから送られてきたタイミング通知を受信する
        /// </summary>
        /// <remarks>
        ///   相手クライアントからRPCで呼び出される
        /// </remarks>
        /// <param name="phase">通知の種類</param>
        [PunRPC]
        private void ReceivePhase(int phase) {
            switch(phase) {
                case finishSceneAnimationKey:
                    onFinishSceneAnimationSubject.OnNext(Unit.Default);
                    break;
                case finishStartAnimationKey:
                    onFinishStartAnimationSubject.OnNext(Unit.Default);
                    break;
            }
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   「カーテンが開き終わった」タイミングをRPCで相手クライアントへ通知する
        /// </summary>
        public void SendFinishSceneAnimation() {
            var view = PhotonView.Get(this);
            view.RPC(nameof(ReceivePhase), RpcTarget.Others, finishSceneAnimationKey);
        }


        /// <summary>
        ///   「着火アニメーションが終了した」タイミングをRPCで相手クライアントへ通知する
        /// </summary>
        public void SendFinishStartAnimation() {
            var view = PhotonView.Get(this);
            view.RPC(nameof(ReceivePhase), RpcTarget.Others, finishStartAnimationKey);
        }

        #endregion

    }
}
