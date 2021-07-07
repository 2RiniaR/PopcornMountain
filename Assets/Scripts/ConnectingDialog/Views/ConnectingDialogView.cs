using System;
using UnityEngine;
using UniRx;


namespace PopcornMountain.ConnectingDialog {
    public class ConnectingDialogView : MonoBehaviour {

        [SerializeField]
        private Animator animator = null;


        private void Show() {
            animator.SetBool("isShow", true);
        }

        private void Hide() {
            animator.SetBool("isShow", false);
        }


        public IObservable<Unit> CreateShowAnimationObservable() {
            return Observable.Empty<Unit>()
                .DoOnSubscribe(Show)
                .Publish()
                .RefCount();
        }

        public IObservable<Unit> CreateHideAnimationObservable() {
            return Observable.Empty<Unit>()
                .DoOnSubscribe(Hide)
                .Publish()
                .RefCount();
        }

    }
}
