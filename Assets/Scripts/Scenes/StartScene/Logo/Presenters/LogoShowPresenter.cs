using UnityEngine;


namespace PopcornMountain.StartScene {
    public class LogoShowPresenter : MonoBehaviour {

        private void Start() {
            var logoAnimatorView = GetComponent<LogoAnimatorView>();
            logoAnimatorView.Show();
        }

    }
}
