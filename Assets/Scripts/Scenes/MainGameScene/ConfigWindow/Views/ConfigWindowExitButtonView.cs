using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene {
    public class ConfigWindowExitButtonView : MonoBehaviour {

        [SerializeField]
        private Button exitButton = null;
        [SerializeField]
        private AudioClip exitButtonSound = null;

        public IObservable<Unit> ExitButtonClickObservable { get; private set; } = null;


        private void Awake() {
            ExitButtonClickObservable = exitButton.OnClickAsObservable().AsUnitObservable().Publish().RefCount();

            ExitButtonClickObservable.Subscribe(_ => PlayExitButtonSound()).AddTo(this);
        }

        private void PlayExitButtonSound() {
            AudioManager.Instance.PlayOneShot(exitButtonSound);
        }

        public void SetEnable(bool isEnable) {
            exitButton.interactable = isEnable;
        }

    }
}
