using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.ResultWindow {
    public class ResultWindowElementView : BaseMainGameComponent {

        [SerializeField]
        private Button againButton = null;
        [SerializeField]
        private Button exitButton = null;
        [SerializeField]
        private AudioClip againButtonSound = null;
        [SerializeField]
        private AudioClip exitButtonSound = null;

        public IObservable<Unit> againButtonClickObservable = null;
        public IObservable<Unit> exitButtonClickObservable = null;


        private void Awake() {
            againButtonClickObservable = againButton.OnClickAsObservable().AsUnitObservable().Publish().RefCount();
            againButtonClickObservable.Subscribe(_ => AudioManager.Instance.PlayOneShot(againButtonSound)).AddTo(this);
            exitButtonClickObservable = exitButton.OnClickAsObservable().AsUnitObservable().Publish().RefCount();
            exitButtonClickObservable.Subscribe(_ => AudioManager.Instance.PlayOneShot(exitButtonSound)).AddTo(this);
        }
        
    }
}
