using UnityEngine;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornAudioView : BaseMainGameComponent {

        [SerializeField]
        private AudioClip pickupSound;
        [SerializeField]
        private AudioClip popSound;
        [SerializeField]
        private AudioClip burnSound;
        [SerializeField]
        private AudioClip[] eatSound;
        [SerializeField]
        private AudioClip[] burnedEatSound;
        [SerializeField]
        private AudioSource fringSoundSource;


        public void PlayPickupSound() {
            if (pickupSound == null) return;
            AudioManager.Instance.PlayOneShot(pickupSound);
        }

        public void PlaySoundWithState(PopcornCookingStates state) {
            switch(state) {
                case PopcornCookingStates.Popped:
                    PlayPopSound();
                    break;
                case PopcornCookingStates.Burned:
                    PlayBurnSound();
                    break;
            }
        }

        public void PlayPopSound() {
            if (popSound == null) return;
            AudioManager.Instance.PlayOneShot(popSound);
        }

        public void PlayBurnSound() {
            if (burnSound == null) return;
            AudioManager.Instance.PlayOneShot(burnSound);
        }

        public void SetFringSound(float power) {
            if (fringSoundSource == null) return;
            fringSoundSource.volume = power * 0.3f;
            if (!fringSoundSource.isPlaying) {
                fringSoundSource.Play();
            }
        }

        public void StopFringSound() {
            if (fringSoundSource == null) return;
            fringSoundSource.Stop();
        }

        public void PlayEatSound(EatingEffectType type) {
            if (type == EatingEffectType.GettingBurnedBeans) {
                if (burnedEatSound == null || burnedEatSound.Length == 0) return;
                AudioManager.Instance.PlayOneShot(burnedEatSound[UnityEngine.Random.Range(0, burnedEatSound.Length-1)]);
            } else {
                if (eatSound == null || eatSound.Length == 0) return;
                AudioManager.Instance.PlayOneShot(eatSound[UnityEngine.Random.Range(0, eatSound.Length-1)]);
            }
        }

    }
}
