using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;


namespace PopcornMountain.ConfigWindow {
    public class ConfigWindowOpenButtonView : MonoBehaviour {

        [SerializeField]
        private Button openButton = null;
        [SerializeField]
        private AudioClip openButtonSound = null;

        public IObservable<Unit> openButtonClickObservable = null;


        private void Awake() {
            openButtonClickObservable = openButton.OnClickAsObservable()
                .Where(_ => openButton.interactable)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            openButtonClickObservable.Subscribe(_ => PlayOpenButtonSound()).AddTo(this);
        }


        private void PlayOpenButtonSound() {
            AudioManager.Instance.PlayOneShot(openButtonSound);
        }


        public void SetEnable(bool isEnable) {
            openButton.interactable = isEnable;
        }

    }
}
