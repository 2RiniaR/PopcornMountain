using UnityEngine;


namespace PopcornMountain.MainGameScene.BlazeBeans {
    public class BlazeBeanParticleView : BaseMainGameComponent {

        [SerializeField]
        private ParticleSystem blazeFragmentParticle = null;

        public void SetBlazeFragmentParticle(float power) {
            var e = blazeFragmentParticle.emission;
            e.rateOverTimeMultiplier = power;
            if (power <= 0f) {
                blazeFragmentParticle.Stop();
                return;
            }
            if (!blazeFragmentParticle.isPlaying) {
                blazeFragmentParticle.Play();
            }
        }

        public void StopBlazeFragmentParticle() {
            blazeFragmentParticle.Stop();
        }

    }
}
