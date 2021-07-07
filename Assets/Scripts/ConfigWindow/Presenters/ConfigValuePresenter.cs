using UnityEngine;
using UniRx;


namespace PopcornMountain.ConfigWindow {
    public class ConfigValuePresenter : MonoBehaviour {

        private void Start() {
            var configWindowElementView = GetComponent<ConfigWindowElementView>();

            configWindowElementView.SetBGMVolumeSliderValue(
                ConfigManager.BGMVolume,
                ConfigManager.BGMMaxValue
            );

            configWindowElementView.SetSEVolumeSliderValue(
                ConfigManager.SEVolume,
                ConfigManager.SEMaxValue
            );

            configWindowElementView.BGMVolumeChangeObservable
                .Subscribe(value => ConfigManager.BGMVolume = value)
                .AddTo(this);

            configWindowElementView.SEVolumeChangeObservable
                .Subscribe(value => ConfigManager.SEVolume = value)
                .AddTo(this);

            ConfigManager.OnBGMVolumeChangedObservable
                .Subscribe(value => configWindowElementView.SetBGMVolumeSliderValue(value, ConfigManager.BGMMaxValue))
                .AddTo(this);

            ConfigManager.OnSEVolumeChangedObservable
                .Subscribe(value => configWindowElementView.SetSEVolumeSliderValue(value, ConfigManager.SEMaxValue))
                .AddTo(this);
        }

    }
}
