using UnityEngine;
using UnityEngine.UI;


namespace PopcornMountain.MainGameScene {
    public class PlaceIndicatorView : MonoBehaviour {

        [SerializeField]
        private Animator animator = null;


        public void Show() {
            animator.SetBool("isShow", true);
        }

        public void Hide() {
            animator.SetBool("isShow", false);
        }

    }
}
