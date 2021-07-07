using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace PopcornMountain {
    public static class ConfigManager {

        #region 定数

        /// <summary>
        ///   BGM音量の設定値を保存するKVSのキー
        /// </summary>
        private const string BGMVolumeKey = "BGMVolume";

        /// <summary>
        ///   SE音量の設定値を保存するKVSのキー
        /// </summary>
        private const string SEVolumeKey  = "SEVolume";

        /// <summary>
        ///   ニックネームの設定値を保存するKVSのキー
        /// </summary>
        private const string NicknameKey = "Nickname";

        /// <summary>
        ///   BGM音量の設定値の最大値
        /// </summary>
        public const float BGMMaxValue = 100f;

        /// <summary>
        ///   SE音量の設定値の最大値
        /// </summary>
        public const float SEMaxValue = 100f;

        #endregion


        #region プロパティ

        /// <summary>
        ///   BGM音量の設定値のReactiveProperty
        /// </summary>
        private static ReactiveProperty<float> bgmVolume;

        /// <summary>
        ///   SE音量の設定値のReactiveProperty
        /// </summary>
        private static ReactiveProperty<float> seVolume;

        /// <summary>
        ///   ニックネームの設定値のReactiveProperty
        /// </summary>
        private static ReactiveProperty<string> nickname;

        /// <summary>
        ///   現在のBGM音量の設定値
        /// </summary>
        public static float BGMVolume {
            get {
                return bgmVolume.Value;
            }
            set {
                if (value < 0 || BGMMaxValue < value) return;
                bgmVolume.Value = value;
            }
        }

        /// <summary>
        ///   現在のSE音量の設定値
        /// </summary>
        public static float SEVolume {
            get {
                return seVolume.Value;
            }
            set {
                if (value < 0 || SEMaxValue < value) return;
                seVolume.Value = value;
            }
        }

        /// <summary>
        ///   現在のニックネームの設定値
        /// </summary>
        public static string Nickname {
            get {
                return nickname.Value;
            }
            set {
                if (0 < value.Length && value.Length <= 12) {
                    PhotonManager.SetNickname(value);
                    nickname.SetValueAndForceNotify(value);
                }
                nickname.SetValueAndForceNotify(Nickname);
            }
        }

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   BGM音量の設定値が変更されたときにOnNext()が発行されるObservable
        /// </summary>
        public static IObservable<float> OnBGMVolumeChangedObservable { get { return bgmVolume; } }

        /// <summary>
        ///   SE音量の設定値が変更されたときにOnNext()が発行されるObservable
        /// </summary>
        public static IObservable<float> OnSEVolumeChangedObservable { get { return seVolume; } }

        /// <summary>
        ///   ニックネームの設定値が変更されたときにOnNext()が発行されるObservable
        /// </summary>
        public static IObservable<string> OnNicknameChangedObservable { get { return nickname; } }

        #endregion


        #region 初期化用の関数

        static ConfigManager() {
            bgmVolume = new ReactiveProperty<float>(PlayerPrefs.GetFloat(BGMVolumeKey, 70f));
            seVolume = new ReactiveProperty<float>(PlayerPrefs.GetFloat(SEVolumeKey, 70f));
            nickname = new ReactiveProperty<string>(PlayerPrefs.GetString(NicknameKey, "Player"));
            OnBGMVolumeChangedObservable.Subscribe(value => PlayerPrefs.SetFloat(BGMVolumeKey, value));
            OnSEVolumeChangedObservable.Subscribe(value => PlayerPrefs.SetFloat(SEVolumeKey, value));
            OnNicknameChangedObservable.Subscribe(value => PlayerPrefs.SetString(NicknameKey, value));
            PhotonManager.SetNickname(Nickname);
        }

        #endregion

    }
}
