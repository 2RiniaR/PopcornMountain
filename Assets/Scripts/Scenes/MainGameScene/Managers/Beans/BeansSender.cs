using System;
using System.Linq;
using UniRx;
using Photon.Pun;


namespace PopcornMountain.MainGameScene {
    public sealed class BeansSender : SingletonMonoBehaviour<BeansSender> {

        #region 非公開のプロパティ

        /// <summary>
        ///   相手のクライアントから豆を受信したときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<Tuple<PopcornParameters, float>> onReceiveBeans = new Subject<Tuple<PopcornParameters, float>>();

        /// <summary>
        ///   相手のクライアントからポップコーンの総数の変更を受信したときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<int> onReceivePopcornCountDifference = new Subject<int>();

        /// <summary>
        ///   相手のクライアントからポップコーンの総数を受信したときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<bool> onReceiveIsOverEmission = new Subject<bool>();

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   相手のクライアントから豆を受信したときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Tuple<PopcornParameters, float>> OnReceiveBeansObservable { get { return onReceiveBeans; } }

        /// <summary>
        ///   相手のクライアントから豆を受信したときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<int> OnReceivePopcornCountObservableDifference { get { return onReceivePopcornCountDifference; } }

        /// <summary>
        ///   相手のクライアントからポップコーンの総数を受信したときにOnNext()が発行されるSubject
        /// </summary>
        public IObservable<bool> OnReceiveIsOverEmissionObsrvable { get { return onReceiveIsOverEmission; } }

        #endregion


        #region 非公開の関数

        /// <summary>
        ///   相手クライアントから送られてきた豆を受信する
        /// </summary>
        /// <remarks>
        ///   相手クライアントからRPCで呼び出される
        /// </remarks>
        /// <param name="serializedBeansData">シリアライズされた豆のデータ</param>
        [PunRPC]
        private void ReceivePopcornCountDifference(int countDifference) {
            onReceivePopcornCountDifference.OnNext(countDifference);
        }


        /// <summary>
        ///   相手クライアントから送られてきた豆を受信する
        /// </summary>
        /// <remarks>
        ///   相手クライアントからRPCで呼び出される
        /// </remarks>
        /// <param name="serializedBeansData">シリアライズされた豆のデータ</param>
        [PunRPC]
        private void ReceiveIsOverEmission(bool isOverEmission) {
            onReceiveIsOverEmission.OnNext(isOverEmission);
        }


        /// <summary>
        ///   相手クライアントから送られてきた豆を受信する
        /// </summary>
        /// <remarks>
        ///   相手クライアントからRPCで呼び出される
        /// </remarks>
        /// <param name="serializedBeansData">シリアライズされた豆のデータ</param>
        [PunRPC]
        private void ReceiveBean(byte[] serializedBeansData) {
            DeserializeBeansParameter(serializedBeansData, out var parameters, out float currentHeat);
            onReceiveBeans.OnNext(new Tuple<PopcornParameters, float>(parameters, currentHeat));
        }


        /// <summary>
        ///   豆のパラメータと熱量のデータをシリアライズして返す
        /// </summary>
        /// <param name="parameters">シリアライズする豆のパラメータ</param>
        /// <param name="currentHeat">シリアライズする豆の熱量</param>
        private byte[] SerializeBeansParameter(PopcornParameters parameters, float currentHeat) {
            byte[] bytes = new byte[128];
            
            BitConverter.GetBytes((int)parameters.type)     .CopyTo(bytes, 0);
            BitConverter.GetBytes(parameters.popHeat)       .CopyTo(bytes, 4 + 8 * 0);
            BitConverter.GetBytes(parameters.blackHeat)     .CopyTo(bytes, 4 + 8 * 1);
            BitConverter.GetBytes(parameters.diffence)      .CopyTo(bytes, 4 + 8 * 2);
            BitConverter.GetBytes(parameters.explosionPower).CopyTo(bytes, 4 + 8 * 3);
            BitConverter.GetBytes(parameters.point)         .CopyTo(bytes, 4 + 8 * 4);
            BitConverter.GetBytes(parameters.steam)         .CopyTo(bytes, 4 + 8 * 5);
            BitConverter.GetBytes(parameters.scale)         .CopyTo(bytes, 4 + 8 * 6);
            BitConverter.GetBytes(parameters.unpoppedMass)  .CopyTo(bytes, 4 + 8 * 7);
            BitConverter.GetBytes(parameters.poppedMass)    .CopyTo(bytes, 4 + 8 * 8);
            BitConverter.GetBytes(currentHeat)              .CopyTo(bytes, 4 + 8 * 9);
            return bytes;
        }


        /// <summary>
        ///   豆のパラメータと熱量のデータをデシリアライズして返す
        /// </summary>
        /// <param name="bytes">デシリアライズする対象のbyte配列</param>
        /// <param name="parameters">デシリアライズされた豆のパラメータが格納されるオブジェクト</param>
        /// <param name="currentHeat">デシリアライズされた豆の熱量が格納されるオブジェクト</param>
        private void DeserializeBeansParameter(byte[] bytes, out PopcornParameters parameters, out float currentHeat) {
            parameters = new PopcornParameters {
                type = (PopcornType)Enum.ToObject(typeof(PopcornType), BitConverter.ToInt32(bytes.Take(4).ToArray(), 0)),
                popHeat =        BitConverter.ToSingle(bytes.Skip(4 + 8 * 0).Take(8).ToArray(), 0),
                blackHeat =      BitConverter.ToSingle(bytes.Skip(4 + 8 * 1).Take(8).ToArray(), 0),
                diffence =       BitConverter.ToSingle(bytes.Skip(4 + 8 * 2).Take(8).ToArray(), 0),
                explosionPower = BitConverter.ToSingle(bytes.Skip(4 + 8 * 3).Take(8).ToArray(), 0),
                point =          BitConverter.ToSingle(bytes.Skip(4 + 8 * 4).Take(8).ToArray(), 0),
                steam =          BitConverter.ToSingle(bytes.Skip(4 + 8 * 5).Take(8).ToArray(), 0),
                scale =          BitConverter.ToSingle(bytes.Skip(4 + 8 * 6).Take(8).ToArray(), 0),
                unpoppedMass =   BitConverter.ToSingle(bytes.Skip(4 + 8 * 7).Take(8).ToArray(), 0),
                poppedMass =     BitConverter.ToSingle(bytes.Skip(4 + 8 * 8).Take(8).ToArray(), 0),
            };
            currentHeat = BitConverter.ToSingle(bytes.Skip(4 + 8 * 9).Take(8).ToArray(), 0);
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   豆をRPCで相手に送信する
        /// </summary>
        /// <param name="parameters">送信する豆のパラメータ</param>
        /// <param name="currentHeat">送信する豆の熱量</param>
        public void SendIsOverEmission(bool isOverEmission) {
            var view = PhotonView.Get(this);
            view.RPC(nameof(ReceiveIsOverEmission), RpcTarget.All, isOverEmission);
        }


        /// <summary>
        ///   豆をRPCで相手に送信する
        /// </summary>
        /// <param name="parameters">送信する豆のパラメータ</param>
        /// <param name="currentHeat">送信する豆の熱量</param>
        public void SendPopcornCountDifference(int countDifference) {
            var view = PhotonView.Get(this);
            view.RPC(nameof(ReceivePopcornCountDifference), RpcTarget.MasterClient, countDifference);
        }


        /// <summary>
        ///   豆をRPCで相手に送信する
        /// </summary>
        /// <param name="parameters">送信する豆のパラメータ</param>
        /// <param name="currentHeat">送信する豆の熱量</param>
        public void SendBean(PopcornParameters parameters, float currentHeat) {
            var view = PhotonView.Get(this);
            var data = SerializeBeansParameter(parameters, currentHeat);
            view.RPC(nameof(ReceiveBean), RpcTarget.Others, data);
        }

        #endregion

    }
}
