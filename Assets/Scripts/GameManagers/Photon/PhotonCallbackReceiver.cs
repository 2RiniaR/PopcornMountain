using UnityEngine;
using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace PopcornMountain {
    public class PhotonCallbackReceiver : IInRoomCallbacks, IConnectionCallbacks, IMatchmakingCallbacks, IDisposable {

        private Subject<Unit> onConnectedSubject = new Subject<Unit>();
        private Subject<Unit> onDisconnectedSubject = new Subject<Unit>();
        private Subject<Unit> onConnectedToMasterSubject = new Subject<Unit>();
        private Subject<Unit> onCreatedRoomSubject = new Subject<Unit>();
        private Subject<Tuple<short, string>> onCreatedRoomFailedSubject = new Subject<Tuple<short, string>>();
        private Subject<Unit> onJoinedRoomSubject = new Subject<Unit>();
        private Subject<Tuple<short, string>> onJoinedRoomFailedSubject = new Subject<Tuple<short, string>>();
        private Subject<Tuple<short, string>> onJoinedRandomFailedSubject = new Subject<Tuple<short, string>>();
        private Subject<Player> onPlayerEnteredRoomSubject = new Subject<Player>();
        private Subject<Player> onPlayerLeftRoomSubject = new Subject<Player>();
        private Subject<Unit> onLeftRoomSubject = new Subject<Unit>();

        public IObservable<Unit> OnConnectedObservable { get { return onConnectedSubject; } }
        public IObservable<Unit> OnDisconnectedObservable { get { return onDisconnectedSubject; } }
        public IObservable<Unit> OnConnectedToMasterObservable { get { return onConnectedToMasterSubject; } }
        public IObservable<Unit> OnCreatedRoomObservable { get { return onCreatedRoomSubject; } }
        public IObservable<Tuple<short, string>> OnCreatedRoomFailedObservable { get { return onCreatedRoomFailedSubject; } }
        public IObservable<Unit> OnJoinedRoomObservable { get { return onJoinedRoomSubject; } }
        public IObservable<Tuple<short, string>> OnJoinedRoomFailedObservable { get { return onJoinedRoomFailedSubject; } }
        public IObservable<Tuple<short, string>> OnJoinedRandomFailedObservable { get { return onJoinedRandomFailedSubject; } }
        public IObservable<Player> OnPlayerEnteredRoomObservable { get { return onPlayerEnteredRoomSubject; } }
        public IObservable<Player> OnPlayerLeftRoomObservable { get { return onPlayerLeftRoomSubject; } }
        public IObservable<Unit> OnLeftRoomObservable { get { return onLeftRoomSubject; } }


        public PhotonCallbackReceiver() {
            PhotonNetwork.AddCallbackTarget(this);
        }
        
        public void Dispose() {
            PhotonNetwork.RemoveCallbackTarget(this);
        }


        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friends) {}
        void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage) {}
        void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data) {}
        void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler) {}
        void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {}
        void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {}
        void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient) {}


        void IMatchmakingCallbacks.OnLeftRoom() {
            Debug.Log("部屋から退出しました。");
            onLeftRoomSubject.OnNext(Unit.Default);
        }


        void IConnectionCallbacks.OnConnected() {
            Debug.Log("接続しました。");
            onConnectedSubject.OnNext(Unit.Default);
        }


        void IConnectionCallbacks.OnDisconnected(DisconnectCause cause) {
            Debug.Log("切断しました。");
            onDisconnectedSubject.OnNext(Unit.Default);
        }


        /// <summary>
        ///   <para>ルームの作成が成功した時に呼ばれるコールバック</para>
        /// </summary>
        void IConnectionCallbacks.OnConnectedToMaster() {
            Debug.Log("マスターサーバーに接続しました");
            onConnectedToMasterSubject.OnNext(Unit.Default);
        }


        /// <summary>
        ///   <para>ルームの作成が成功した時に呼ばれるコールバック</para>
        /// </summary>
        void IMatchmakingCallbacks.OnCreatedRoom() {
            Debug.Log("ルーム作成に成功しました");
            onCreatedRoomSubject.OnNext(Unit.Default);
        }


        /// <summary>
        ///   <para>ルームの作成が失敗した時に呼ばれるコールバック</para>
        /// </summary>
        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message) {
            Debug.Log($"ルーム作成に失敗しました: {message}");
            onCreatedRoomFailedSubject.OnNext(new Tuple<short, string>(returnCode, message));
        }


        /// <summary>
        ///   <para>マッチングが成功した時に呼ばれるコールバック</para>
        /// </summary>
        void IMatchmakingCallbacks.OnJoinedRoom() {
            Debug.Log("ルームに参加しました");
            onJoinedRoomSubject.OnNext(Unit.Default);
        }


        /// <summary>
        ///   <para>通常のマッチングが失敗した時に呼ばれるコールバック</para>
        /// </summary>
        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message) {
            Debug.Log($"ルーム参加に失敗しました: {message}");
            onJoinedRoomFailedSubject.OnNext(new Tuple<short, string>(returnCode, message));
        }


        /// <summary>
        ///   <para>ランダムマッチングが失敗した時に呼ばれるコールバック</para>
        /// </summary>
        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message) {
            // ランダムに参加できるルームが存在しないなら、新しいルームを作成する
            Debug.Log($"ランダムルーム参加に失敗しました: {message}");
            PhotonNetwork.CreateRoom(null, PhotonManager.randomRoomOptions, PhotonManager.lobbyType);
            onJoinedRandomFailedSubject.OnNext(new Tuple<short, string>(returnCode, message));
        }


        /// <summary>
        ///   <para>他プレイヤーが参加した時に呼ばれるコールバック</para>
        /// </summary>
        void IInRoomCallbacks.OnPlayerEnteredRoom(Player player) {
            Debug.Log(player.NickName + "が参加しました");
            onPlayerEnteredRoomSubject.OnNext(player);
        }


        /// <summary>
        ///   <para>他プレイヤーが退出した時に呼ばれるコールバック</para>
        /// </summary>
        void IInRoomCallbacks.OnPlayerLeftRoom(Player player) {
            Debug.Log(player.NickName + "が退出しました");
            onPlayerLeftRoomSubject.OnNext(player);
        }

    }
}