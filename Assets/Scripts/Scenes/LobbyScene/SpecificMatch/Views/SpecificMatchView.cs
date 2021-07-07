using System;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using TMPro;


namespace PopcornMountain.LobbyScene.SpecificMatch {
    public class SpecificMatchView : MonoBehaviour {

        [SerializeField]
        private Button submitButton = null;
        [SerializeField]
        private Animator warningMessageAnimator = null;
        [SerializeField]
        private TMP_InputField passwordInput = null;
        public string password { get { return passwordInput.text; } }


        public IObservable<Unit> OnPushButtonObservable { get; private set; }

        private void Awake() {
            OnPushButtonObservable = submitButton
                .OnClickAsObservable()
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }

        public void SetEnable(bool isEnable) {
            passwordInput.interactable = isEnable;
            submitButton.interactable = isEnable;
        }

        public void Warning() {
            warningMessageAnimator.SetTrigger("Warn");
        }

    }
}
