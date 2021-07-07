using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene {
    public class HandleAnimationView : MonoBehaviour {

        [SerializeField]
        private AudioClip turnOnSound;
        [SerializeField]
        private AudioClip turnOffSound;
        [SerializeField]
        private AudioSource burningAudioSource;
        [SerializeField]
        private Animator handleAnimator = null;
        private int turnOnHash = Animator.StringToHash("Base Layer.TurnOn");
        private int turnOffHash = Animator.StringToHash("Base Layer.TurnOff");
        private ObservableStateMachineTrigger animationTrigger;


        private void Awake() {
            animationTrigger = handleAnimator.GetBehaviour<ObservableStateMachineTrigger>();
        }

        private void PlayTurnOnSound() {
            AudioManager.Instance.PlayOneShot(turnOnSound);
            burningAudioSource.PlayDelayed(0.5f);
        }

        private void PlayTurnOffSound() {
            AudioManager.Instance.PlayOneShot(turnOffSound);
            burningAudioSource.Stop();
        }

        private void PlayTurnOnAnimation() {
            handleAnimator.SetBool("isTurnOn", true);
        }

        private void PlayTurnOffAnimation() {
            handleAnimator.SetBool("isTurnOn", false);
        }


        public IObservable<Unit> CreateTurnOnObservable() {
            var animatorObservable = animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == turnOnHash)
                .AsUnitObservable()
                .DoOnSubscribe(PlayTurnOnAnimation)
                .First()
                .Publish()
                .RefCount();

            var audioObservable = Observable.Return(Unit.Default)
                .DoOnSubscribe(PlayTurnOnSound)
                .First()
                .Publish()
                .RefCount();

            return Observable.Merge(animatorObservable, audioObservable)
                .IgnoreElements()
                .Publish()
                .RefCount();
        }


        public IObservable<Unit> CreateTurnOffObservable() {
            var animatorObservable = animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == turnOffHash)
                .AsUnitObservable()
                .DoOnSubscribe(PlayTurnOffAnimation)
                .First()
                .Publish()
                .RefCount();

            var audioObservable = Observable.Return(Unit.Default)
                .DoOnSubscribe(PlayTurnOffSound)
                .First()
                .Publish()
                .RefCount();

            return Observable.Zip(animatorObservable, audioObservable)
                .AsUnitObservable()
                .First()
                .Publish()
                .RefCount();
        }

    }
}
