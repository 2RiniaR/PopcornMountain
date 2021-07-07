using System;
using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain {
    public static class PhotonManager {

        public static readonly RoomOptions randomRoomOptions = new RoomOptions() {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 2
        };

        public static readonly RoomOptions specificRoomOptions = new RoomOptions() {
            IsVisible = false,
            IsOpen = true,
            MaxPlayers = 2
        };

        public static readonly TypedLobby lobbyType = TypedLobby.Default;
        public static PhotonCallbackReceiver callbacks { get; private set; } = new PhotonCallbackReceiver();


        /// <summary>
        ///   <para>Subscribe時にサーバーへ接続し、接続が完了したら OnCompleted() されるObservableを返す</para>
        ///   <para>既にサーバーへ接続済みの場合、即座にOnCompleted()を発行するObservableを返す</para>
        /// </summary>
        public static IObservable<Unit> CreateConnectObservable() {
            if (PhotonNetwork.IsConnected) {
                return Observable.Empty<Unit>();
            }
            return callbacks.OnConnectedToMasterObservable
                .DoOnSubscribe(() => PhotonNetwork.ConnectUsingSettings())
                .First()
                .PublishLast()
                .RefCount();
        }


        /// <summary>
        ///   <para>ランダムな部屋に参加し、GameSceneをMainGameSceneに遷移させる</para>
        ///   <para>部屋がない場合、新しく作成する</para>
        /// </summary>
        public static IObservable<Unit> CreateMatchRandomRoomObservable() {
            if (PhotonNetwork.InRoom) {
                return Observable.Empty<Unit>();
            }
            return callbacks.OnJoinedRoomObservable
                .DoOnSubscribe(() => PhotonNetwork.JoinRandomRoom())
                .First()
                .PublishLast()
                .RefCount();
        }


        /// <summary>
        ///   <para>指定した名前の部屋に参加し、GameSceneをMainGameSceneに遷移させる</para>
        ///   <para>指定した名前の部屋がない場合、新しく作成する</para>
        /// </summary>
        public static IObservable<Unit> CreateMatchSpecificRoomObservable(string roomName) {
            if (PhotonNetwork.InRoom) {
                return Observable.Empty<Unit>();
            }
            return callbacks.OnJoinedRoomObservable
                .DoOnSubscribe(() => PhotonNetwork.JoinOrCreateRoom(roomName, specificRoomOptions, lobbyType))
                .First()
                .PublishLast()
                .RefCount();
        }


        /// <summary>
        ///   <para>部屋から退出する</para>
        /// </summary>
        public static IObservable<Unit> CreateLeaveRoomObservable() {
            if (!PhotonNetwork.InRoom) {
                return Observable.Empty<Unit>();
            }
            return callbacks.OnLeftRoomObservable
                .DoOnSubscribe(() => PhotonNetwork.LeaveRoom())
                .First()
                .PublishLast()
                .RefCount();
        }


        /// <summary>
        ///   <para>サーバーから切断する</para>
        /// </summary>
        public static IObservable<Unit> CreateDisconnectObservable() {
            if (!PhotonNetwork.IsConnected) {
                return Observable.Empty<Unit>();
            }
            return callbacks.OnDisconnectedObservable
                .DoOnSubscribe(() => PhotonNetwork.Disconnect())
                .First()
                .PublishLast()
                .RefCount();
        }


        /// <summary>
        ///   <para>プレイヤーのニックネームを設定する</para>
        /// </summary>
        public static void SetNickname(string nickname) {
            PhotonNetwork.NickName = nickname;
        }

        /// <summary>
        ///   <para>自分プレイヤーのニックネームを設定する</para>
        /// </summary>
        public static string GetSelfNickname() {
            return PhotonNetwork.LocalPlayer.NickName;
        }

        /// <summary>
        ///   <para>相手プレイヤーのニックネームを設定する</para>
        /// </summary>
        public static string GetOtherNickname() {
            if (PhotonNetwork.PlayerListOthers.Length < 1) return null;
            return PhotonNetwork.PlayerListOthers[0]?.NickName;
        }

        public static int GetPlayerCountInCurrentRoom() {
            return PhotonNetwork.CurrentRoom.PlayerCount;
        }

        public static int GetPlayerCountPlayingGame() {
            return PhotonNetwork.CountOfPlayersInRooms;
        }

        public static int GetPlayerCountOnline() {
            return PhotonNetwork.CountOfPlayers;
        }

        public static string GetRoomId() {
            return PhotonNetwork.CurrentRoom.Name;
        }

        public static bool IsHostPlayer() {
            return PhotonNetwork.LocalPlayer.IsMasterClient;
        }

        public static int GetMaxPlayerInRoom() {
            return PhotonNetwork.CurrentRoom.MaxPlayers;
        }

        public static bool IsCurrentRoomVisible() {
            return PhotonNetwork.CurrentRoom.IsVisible;
        }

    }
}