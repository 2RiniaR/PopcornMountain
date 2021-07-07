using System;
using UnityEngine;
using UniRx;


namespace PopcornMountain.LobbyScene.RandomMatch {
    public class RandomMatchIndicatePresenter : MonoBehaviour {

        private void Start() {
            var textView = GetComponent<RandomMatchTextView>();

            Observable.Interval(TimeSpan.FromSeconds(2f))
                .Subscribe(_ => textView.SetUserCount(PhotonManager.GetPlayerCountPlayingGame()))
                .AddTo(this);
        }

    }
}
