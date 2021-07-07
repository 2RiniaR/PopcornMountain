using UnityEngine;


namespace PopcornMountain.MainGameScene.Faint {
    public class FaintRedEffectView : MonoBehaviour {

        [SerializeField]
        private Animator _animator = null;


        public void SetRedEffect(bool isFainting) {
            _animator.SetBool("isFainting", isFainting);
        }

    }
}
