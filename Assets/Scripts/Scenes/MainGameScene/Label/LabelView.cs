using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;


namespace PopcornMountain.MainGameScene.Label {
    public class LabelView : MonoBehaviour {
        
        [SerializeField]
        private Animator labelAnimator = null;
        [SerializeField]
        private TextMeshProUGUI labelText = null;
        [SerializeField]
        private AudioSource whistleAudio = null;
        private ObservableStateMachineTrigger animationTrigger;
        private int displayStateHash = Animator.StringToHash("Base Layer.Display");
        
        
        private void Awake() {
            animationTrigger = labelAnimator.GetBehaviour<ObservableStateMachineTrigger>();
        }
        
        private void DisplayLabel(string displayText) {
            labelText.text = displayText;
            labelAnimator.SetTrigger("Display");
        }
        
        private void PlayWhistleSound() {
            whistleAudio.Play();
        }
        
        
        public IObservable<Unit> CreatePlayingObservable(string displayText) {
            var animatorObservable = animationTrigger
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == displayStateHash)
                .AsUnitObservable()
                .DoOnSubscribe(() => DisplayLabel(displayText))
                .First()
                .Publish()
                .RefCount();
            
            var audioObservable = Observable.Return(Unit.Default)
                .Delay(TimeSpan.FromSeconds(0.5f))
                .Do(_ => PlayWhistleSound())
                .First()
                .Publish()
                .RefCount();
            
            return Observable.Merge(animatorObservable, audioObservable)
                .IgnoreElements()
                .Publish()
                .RefCount();
        }
        
    }
}
