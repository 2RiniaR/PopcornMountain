using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene.DangerBean {
    public class DangerBeanParticleView : BaseMainGameComponent {

        [SerializeField]
        private ParticleSystem energyDrainParticle;
        [SerializeField]
        private ParticleSystem energyFlashParticle;
        [SerializeField]
        private ParticleSystem blastParticle;


        // powerは0~1の範囲内
        public void SetEnergyDrainParticle(float power) {
            if (energyDrainParticle == null) return;
            
            // 量(10 -> 300)
            var e = energyDrainParticle.emission;
            e.rateOverTime = 10 + (290 * Mathf.Pow(power, 2));
            
            // 速度(-0.1 -> -0.5)
            var m = energyDrainParticle.main;
            m.startSpeed = -0.1f - (0.4f * power);

            // 半径(0.5 -> 0.8)
            var s = energyDrainParticle.shape;
            s.radius = 0.5f + (0.3f * power);

            if (power <= 0f) {
                energyDrainParticle.Stop();
                return;
            }
            if (!energyDrainParticle.isPlaying) {
                energyDrainParticle.Play();
            }
        }

        public void StopEnergyDrainParticle() {
            if (energyDrainParticle == null) return;
            energyDrainParticle.Stop();
        }

        // powerは0~1の範囲内
        public void SetEnergyFlashParticle(float power) {
            // 0.0 ~ 0.5 -> 点滅なし
            // 0.5 ~ 0.9 -> leftTime=0.1, emittion=2~5
            // 0.9 ~ 1.0 -> leftTime=0.05, emittion=10
            if (energyFlashParticle == null) return;
            
            if (power < 0.5f) {
                energyFlashParticle.Stop();
                return;
            }

            // 量
            var e = energyFlashParticle.emission;
            e.rateOverTimeMultiplier = (power < 0.9f) ? (2f + (power - 0.5f) * 6f) : 10f;

            // 残留時間
            var m = energyFlashParticle.main;
            m.startLifetime = (power < 0.9f) ? 0.1f : 0.05f;
            m.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0, 0, power));

            if (!energyFlashParticle.isPlaying) {
                energyFlashParticle.Play();
            }
        }

        public void StopEnergyFlashParticle() {
            if (energyFlashParticle == null) return;
            energyFlashParticle.Stop();
        }

        public void PlayBlastParticleOnParent() {
            var inst = Instantiate(blastParticle, transform.position, transform.rotation, transform.parent);
            
            Observable.EveryUpdate()
                .DoOnSubscribe(() => inst.Play())
                .TakeWhile(_ => inst.IsAlive())
                .Subscribe(_ => {}, () => Destroy(inst.gameObject))
                .AddTo(inst);
        }

    }
}
