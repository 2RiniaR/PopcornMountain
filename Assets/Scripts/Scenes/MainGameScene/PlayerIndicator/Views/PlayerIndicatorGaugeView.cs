using UnityEngine;
using UnityEngine.UI;


namespace PopcornMountain.MainGameScene.PlayerIndicator {
    public class PlayerIndicatorGaugeView : BaseMainGameComponent {

        [SerializeField]
        private Slider gaugeSlider = null;
        [SerializeField]
        private AudioClip getPointSound = null;
        [SerializeField]
        private AudioClip lostPointSound = null;
        [SerializeField]
        private AudioClip maxPointSound = null;
        [SerializeField]
        private Animator gaugeAnimator = null;


        public void SetValue(float value, float maxValue) {
            gaugeSlider.maxValue = maxValue;
            gaugeSlider.value = value;
        }

        public void PlayGetPointAnimation() {
            gaugeAnimator.SetTrigger("Get");
            AudioManager.Instance.PlayOneShot(getPointSound);
        }

        public void PlayLostPointAnimation() {
            gaugeAnimator.SetTrigger("Lost");
            AudioManager.Instance.PlayOneShot(lostPointSound);
        }

        public void PlayMaxPointAnimation() {
            gaugeAnimator.SetTrigger("Max");
            AudioManager.Instance.PlayOneShot(maxPointSound);
        }

    }
}
