using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.StartScene {
    public class StartMenuView : MonoBehaviour {

        [SerializeField]
        private Button startMultiPlayButton = null;
        [SerializeField]
        private Button exitButton = null;
        [SerializeField]
        private AudioClip pushStartGameSound = null;

        public IObservable<Unit> startMultiPlayButtonClickObservable = null;
        public IObservable<Unit> exitButtonClickObservable = null;


        private void Awake() {
            startMultiPlayButtonClickObservable = startMultiPlayButton
                .OnClickAsObservable()
                .AsUnitObservable()
                .Publish()
                .RefCount();

            exitButtonClickObservable = exitButton
                .OnClickAsObservable()
                .AsUnitObservable()
                .Publish()
                .RefCount();

            startMultiPlayButtonClickObservable.Subscribe(_ => PlayStartGameSound()).AddTo(this);
        }

        private void PlayStartGameSound() {
            AudioManager.Instance.PlayOneShot(pushStartGameSound);
        }

        public void SetEnable(bool isEnable) {
            startMultiPlayButton.interactable = isEnable;
            exitButton.interactable = isEnable;
        }

    }
}
