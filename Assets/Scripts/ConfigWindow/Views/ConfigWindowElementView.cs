using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

namespace PopcornMountain.ConfigWindow {
    public class ConfigWindowElementView : MonoBehaviour {

        [SerializeField]
        private Slider bgmVolumeSlider = null;
        [SerializeField]
        private Slider seVolumeSlider = null;
        [SerializeField]
        private Button returnButton = null;
        [SerializeField]
        private AudioClip returnButtonSound = null;
        [SerializeField]
        private AudioClip checkSEVolumeSound = null;

        public IObservable<float> BGMVolumeChangeObservable = null;
        public IObservable<float> SEVolumeChangeObservable = null;
        public IObservable<Unit> SEVolumeSliderUpObservable = null;
        public IObservable<Unit> ReturnButtonClickObservable = null;


        private void Awake() {
            BGMVolumeChangeObservable = bgmVolumeSlider.OnValueChangedAsObservable();
            SEVolumeChangeObservable = seVolumeSlider.OnValueChangedAsObservable();

            SEVolumeSliderUpObservable = seVolumeSlider.OnPointerUpAsObservable()
                .Where(_ => seVolumeSlider.interactable)
                .AsUnitObservable()
                .Publish()
                .RefCount();

            ReturnButtonClickObservable = returnButton.OnClickAsObservable()
                .AsUnitObservable()
                .Publish()
                .RefCount();

            SEVolumeSliderUpObservable.Subscribe(_ => PlayCheckSESound()).AddTo(this);
            ReturnButtonClickObservable.Subscribe(_ => PlayReturnButtonSound()).AddTo(this);
        }


        private void PlayReturnButtonSound() {
            AudioManager.Instance.PlayOneShot(returnButtonSound);
        }

        private void PlayCheckSESound() {
            AudioManager.Instance.PlayOneShot(checkSEVolumeSound);
        }


        public void SetBGMVolumeSliderValue(float value, float maxValue) {
            bgmVolumeSlider.maxValue = maxValue;
            bgmVolumeSlider.value = value;
        }

        public void SetSEVolumeSliderValue(float value, float maxValue) {
            seVolumeSlider.maxValue = maxValue;
            seVolumeSlider.value = value;
        }

        public void SetEnable(bool isEnable) {
            bgmVolumeSlider.interactable = isEnable;
            seVolumeSlider.interactable = isEnable;
            returnButton.interactable = isEnable;
        }

    }
}
