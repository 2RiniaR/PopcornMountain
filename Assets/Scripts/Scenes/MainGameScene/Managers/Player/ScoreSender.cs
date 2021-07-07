using System;
using UniRx;
using Photon.Pun;


namespace PopcornMountain.MainGameScene {
    public sealed class ScoreSender : SingletonMonoBehaviour<ScoreSender> {

        #region 非公開のプロパティ

        /// <summary>
        ///   相手側のクライアントからスコアを受け取ったときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<float> onReceiveScore = new Subject<float>();

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   相手側のクライアントからスコアを受け取ったときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<float> OnReceiveScoreObservable { get { return onReceiveScore; } }

        #endregion


        #region 非公開の関数

        /// <summary>
        ///   相手クライアントから送られてきたスコアを受信する
        /// </summary>
        /// <remarks>
        ///   相手クライアントからRPCで呼び出される
        /// </remarks>
        /// <param name="score">受け取ったスコア</param>
        [PunRPC]
        private void ReceiveScore(float score) {
            onReceiveScore.OnNext(score);
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   相手側のクライアントに現在のスコアを送信する
        /// </summary>
        /// <param name="score">送信するスコア</param>
        public void SendScore(float score) {
            var view = PhotonView.Get(this);
            view.RPC(nameof(ReceiveScore), RpcTarget.Others, score);
        }

        #endregion

    }
}
