using UnityEngine;
using UnityEngine.Audio;
using UniRx;
using Cysharp.Threading.Tasks;


namespace PopcornMountain {
    public sealed class AudioManager : SingletonMonoBehaviour<AudioManager> {

        #region 定数

        /// <summary>
        ///   Resourcesフォルダを基準とした、当ゲームオブジェクトのPrefabへのパス
        /// </summary>
        private const string mainAudioSourceResourcePath = "Prefabs/Managers/MainAudioSource";

        /// <summary>
        ///   ミュート時のボリューム(dB)
        /// </summary>
        private const float muteVolume = -80f;

        /// <summary>
        ///   最小ボリューム(dB)
        /// </summary>
        private const float minVolume = -60f;

        /// <summary>
        ///   最大ボリューム(dB)
        /// </summary>
        private const float maxVolume = 0f;

        #endregion


        #region コンポーネント参照

        [SerializeField]
        private AudioSource bgmAudioSource = null;
        [SerializeField]
        private AudioSource seAudioSource = null;
        [SerializeField]
        private AudioMixer mainAudioMixer = null;

        #endregion


        #region 初期化用の関数

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitInstantiateObject() {
            // ゲーム開始時にオブジェクトを生成する
            var instance = GameObject.Instantiate(Resources.Load(mainAudioSourceResourcePath) as GameObject);
        }


        private void Start() {
            // 設定のBGM音量が変更されたとき、AudioMixerのBGM音量を下げる
            ConfigManager.OnBGMVolumeChangedObservable
                .Select(GetVolumeFromConfigValue)
                .Subscribe(SetBGMVolume);

            // 設定のSE音量が変更されたとき、AudioMixerのSE音量を下げる
            ConfigManager.OnSEVolumeChangedObservable
                .Select(GetVolumeFromConfigValue)
                .Subscribe(SetSEVolume);
        }

        #endregion


        #region 非公開の関数

        /// <summary>
        ///   音量の設定値から音量スケール(dB)を返す
        /// </summary>
        /// <param name="value">音量の設定値</param>
        private float GetVolumeFromConfigValue(float value) {
            if (value == 0f) {
                return muteVolume;
            }
            float differenceMinToMax = maxVolume - minVolume;
            return minVolume + (differenceMinToMax * value / 100f);
        }


        /// <summary>
        ///   オーディオミキサーのBGM音量を設定する
        /// </summary>
        /// <param name="volume">音量(dB)</param>
        private void SetBGMVolume(float volume) {
            mainAudioMixer.SetFloat("BGMVolume", volume);
        }


        /// <summary>
        ///   オーディオミキサーのSE音量を設定する
        /// </summary>
        /// <param name="volume">音量(dB)</param>
        private void SetSEVolume(float volume) {
            mainAudioMixer.SetFloat("SEVolume", volume);
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   一度だけ音声を再生する(SE用)
        /// </summary>
        /// <param name="sound">再生する音声のAudioClip</param>
        /// <param name="volume">再生する音声の音量(0..1)</param>
        public void PlayOneShot(AudioClip sound, float volume = 1f) {
            seAudioSource.PlayOneShot(sound, volume);
        }

        /// <summary>
        ///   BGMを再生する
        /// </summary>
        public void PlayBGM() {
            bgmAudioSource.Play();
        }

        /// <summary>
        ///   BGMを停止する
        /// </summary>
        public void StopBGM() {
            bgmAudioSource.Stop();
        }

        /// <summary>
        ///   BGMを一時停止する
        /// </summary>
        public void PauseBGM() {
            bgmAudioSource.Pause();
        }

        /// <summary>
        ///   BGMの一時停止を解除する
        /// </summary>
        public void UnpauseBGM() {
            bgmAudioSource.UnPause();
        }

        /// <summary>
        ///   BGMを変更する
        /// </summary>
        public void ChangeBGM(AudioClip bgm, bool isRefreshSame = false) {
            if (!isRefreshSame && bgmAudioSource.clip == bgm) {
                return;
            }
            bgmAudioSource.clip = bgm;
            StopBGM();
            PlayBGM();
        }

        #endregion

    }
}
