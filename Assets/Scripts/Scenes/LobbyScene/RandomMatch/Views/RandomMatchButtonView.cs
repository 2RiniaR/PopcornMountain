using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;


namespace PopcornMountain.LobbyScene.RandomMatch {
    public class RandomMatchButtonView : MonoBehaviour {

        [SerializeField]
        private Button submitButton = null;

        public IObservable<Unit> OnPushButtonObservable { get; private set; }

        private void Awake() {
            OnPushButtonObservable = submitButton
                .OnClickAsObservable()
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }

        public void SetEnable(bool isEnable) {
            submitButton.interactable = isEnable;
        }

    }
}
