using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public class HeatAnimationView : MonoBehaviour {

        [SerializeField]
        private Animator potAnimator = null;
        [SerializeField]
        private ParticleSystem fireParticle = null;


        // スライダーの表示を変更する
        private void SetPotHeatEffect(int power) {
            potAnimator.SetInteger("FirePower", power);
        }

        /// <summary>
        /// </summary>
        /// <param name="power">煙の強さ。(0 <= power <= 1)</param>
        private void SetFireParticle(float power) {
            var e = fireParticle.emission;
            e.rateOverTimeMultiplier = power * 5;
            if (power <= 0f) {
                fireParticle.Stop();
                return;
            }
            if (!fireParticle.isPlaying) {
                fireParticle.Play();
            }
        }


        public void SetPower(int power, int maxPower) {
            SetPotHeatEffect(power);
            SetFireParticle((float)power / maxPower);
        }

    }
}
