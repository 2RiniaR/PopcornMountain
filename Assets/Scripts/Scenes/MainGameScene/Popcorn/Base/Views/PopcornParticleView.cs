using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornParticleView : BaseMainGameComponent {

        [SerializeField]
        private ParticleSystem popFragmentParticle = null;
        [SerializeField]
        private ParticleSystem popImpactParticle = null;
        [SerializeField]
        private ParticleSystem steamParticle = null;
        [SerializeField]
        private ParticleSystem impactSmoke = null;
        [SerializeField]
        private ParticleSystem eatenParticle = null;


        public void PlayPopParticle() {
            if (popFragmentParticle != null) {
                var inst = Instantiate(popFragmentParticle, transform.position, transform.rotation, transform.parent);
                
                Observable.Timer(TimeSpan.FromSeconds(inst.main.duration))
                    .DoOnSubscribe(() => inst.Play())
                    .First()
                    .Subscribe(_ => Destroy(inst.gameObject))
                    .AddTo(inst);
            }

            if (popImpactParticle != null) {
                var inst = Instantiate(popImpactParticle, transform.position, transform.rotation, transform.parent);
                
                Observable.Timer(TimeSpan.FromSeconds(inst.main.duration))
                    .DoOnSubscribe(() => inst.Play())
                    .First()
                    .Subscribe(_ => Destroy(inst.gameObject))
                    .AddTo(inst);
            }
        }


        /// <summary>
        ///   <para></para>
        ///   <param name="power">煙の強さ。(0 <= power <= 1)</param>
        /// </summary>
        public void SetSteamParticle(float power) {
            if (steamParticle == null) return;
            var e = steamParticle.emission;
            e.rateOverTimeMultiplier = power;
            if (power <= 0f) {
                steamParticle.Stop();
                return;
            }
            if (!steamParticle.isPlaying) {
                steamParticle.Play();
            }
        }

        public void StopSteamParticle() {
            if (steamParticle == null) return;
            SetSteamParticle(0f);
        }


        public void PlayEatenParticle() {
            if (eatenParticle == null) return;
            var inst = Instantiate(eatenParticle, transform.position, transform.rotation, transform.parent);
            
            Observable.Timer(TimeSpan.FromSeconds(inst.main.duration))
                .DoOnSubscribe(() => inst.Play())
                .First()
                .Subscribe(_ => Destroy(inst.gameObject))
                .AddTo(inst);
        }


        public void PlayImpactSmokeParticle() {
            if (impactSmoke == null) return;
            var inst = Instantiate(impactSmoke, transform.position, transform.rotation, transform.parent);
            
            Observable.Timer(TimeSpan.FromSeconds(inst.main.duration))
                .DoOnSubscribe(() => inst.Play())
                .First()
                .Subscribe(_ => Destroy(inst.gameObject))
                .AddTo(inst);
        }

    }
}
