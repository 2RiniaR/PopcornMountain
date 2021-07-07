using System;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class PlayerManager : BaseMainGameManager {

        #region 子コンポーネント

        private ScoreSender sender = null;

        /// <summary>
        ///   各プレイヤーのオブジェクト
        /// </summary>
        private Player[] players = null;

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   Subscribeしたパイプラインを一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   勝者が決定したときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<PlayerID> OnWinnerDecidedObservable { get; private set; } = null;

        #endregion


        #region 初期化用の関数

        public override void BeforeLoadScene() {
            sender = GameObject.FindWithTag("PhotonView")?.GetComponent<ScoreSender>();

            int playerCount = Enum.GetNames(typeof(PlayerID)).Length;
            players = new Player[playerCount];

            // 自分プレイヤーを初期化する
            players[(int)PlayerID.Self] = new Player(
                new PlayerParameters(PlayerID.Self, PhotonManager.GetSelfNickname(), 100f)
            );

            // 相手プレイヤーを初期化する
            players[(int)PlayerID.Other] = new Player(
                new PlayerParameters(PlayerID.Other, "Other Player", 100f)
            );

            // 相手プレイヤーが入室したら、相手プレイヤーのオブジェクトを初期化する
            PhotonManager.callbacks.OnPlayerEnteredRoomObservable
                .AsUnitObservable()
                .StartWith(Unit.Default)
                .Select(_ => PhotonManager.GetOtherNickname())
                .Where(nickname => nickname != null)
                .Subscribe(nickname => GetPlayer(PlayerID.Other).SetNickname(nickname))
                .AddTo(onDispose);

            // OnWinnerDecidedObservableを初期化する
            OnWinnerDecidedObservable = players.ToObservable()
                .SelectMany(x => x.OnFullHungerPoint, (x, _) => x.parameters.playerID)
                .Publish()
                .RefCount()
                .First()
                .PublishLast()
                .RefCount();

            // 相手側のクライアントからスコアを受信したら、相手プレイヤーのオブジェクトに反映する
            sender.OnReceiveScoreObservable
                .Subscribe(score => players[(int)PlayerID.Other].SetHungerPoint(score))
                .AddTo(onDispose);

            // 自分プレイヤーのスコアが変更されたとき、相手側のクライアントへ送信する
            players[(int)PlayerID.Self].OnChangeHungerPoint
                .Subscribe(score => sender.SendScore(score))
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
        ///   自分プレイヤーのスコアを加算する
        /// </summary>
        /// <param name="score">加算するスコア</param>
        public void AddSelfPlayerScore(float score) {
            players[(int)PlayerID.Self].AddHungerPoint(score);
        }


        /// <summary>
        ///   指定したプレイヤーのパラメータを返す
        /// </summary>
        /// <param name="id">対象のプレイヤーID</param>
        public Player GetPlayer(PlayerID id) {
            return players[(int)id];
        }

        #endregion

    }
}
