using System;
using UnityEngine;
using UniRx;
using TMPro;


namespace PopcornMountain.LobbyScene.User {
    public class UserNicknameInputView : MonoBehaviour {

        [SerializeField]
        private TMP_InputField nicknameInput = null;
        public IObservable<string> OnChangeNicknameObservable { get; private set; }

        private void Awake() {
            OnChangeNicknameObservable = nicknameInput.onEndEdit.AsObservable()
                .Publish()
                .RefCount();
        }

        public void SetText(string text) {
            nicknameInput.text = text;
        }

        public void SetEnable(bool isEnable) {
            nicknameInput.interactable = isEnable;
        }

    }
}
