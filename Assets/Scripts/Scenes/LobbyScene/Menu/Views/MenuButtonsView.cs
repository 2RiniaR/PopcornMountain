using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;


namespace PopcornMountain.LobbyScene.Menu {
    public class MenuButtonsView : MonoBehaviour {

        [SerializeField]
        private Button returnButton = null;
        [SerializeField]
        private Button howtoButton = null;

        public IObservable<Unit> OnPushReturnButtonObservable { get; private set; }
        public IObservable<Unit> OnPushHowToButtonObservable { get; private set; }

        private void Awake() {
            OnPushReturnButtonObservable = returnButton
                .OnClickAsObservable()
                .AsUnitObservable()
                .Publish()
                .RefCount();

            if (howtoButton != null)
            {
                OnPushHowToButtonObservable = howtoButton
                .OnClickAsObservable()
                .AsUnitObservable()
                .Publish()
                .RefCount();
            }
        }

        public void SetEnable(bool isEnable) {
            returnButton.interactable = isEnable;

            if (howtoButton != null)
            {
                howtoButton.interactable = isEnable;
            }
        }

    }
}
