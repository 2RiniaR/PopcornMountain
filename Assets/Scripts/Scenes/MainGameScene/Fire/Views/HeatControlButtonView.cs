using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class HeatControlButtonView : MonoBehaviour {

        [SerializeField]
        private List<Button> firePowerButtons = null;
        [SerializeField]
        private Animator sliderAnimator = null;
        [SerializeField]
        private AudioClip changeFirePowerSound = null;

        public IObservable<int> OnPowerButtonClickedObservable { get; private set; } = null;


        private void Awake() {
            var buttonsSubject = new Subject<Tuple<Button, int>>();
            OnPowerButtonClickedObservable = buttonsSubject.SelectMany(
                x => x.Item1.OnClickAsObservable().Where(_ => x.Item1.enabled),
                (x, _) => x.Item2
            )
            .Publish()
            .RefCount();

            OnPowerButtonClickedObservable.Publish().Connect();

            for (int i = 0; i < firePowerButtons.Count; i++) {
                buttonsSubject.OnNext(new Tuple<Button, int>(firePowerButtons[i], i+1));
            }

            OnPowerButtonClickedObservable
                .Subscribe(_ => PlayChangeFirePowerSound())
                .AddTo(this);
        }


        private void PlayChangeFirePowerSound() {
            AudioManager.Instance.PlayOneShot(changeFirePowerSound);
        }


        // スライダーの表示を変更する
        public void SetSliderPowerDisplay(int power) {
            sliderAnimator.SetInteger("Power", power);
        }

        public void SetEnable(bool isEnable) {
            foreach (var button in firePowerButtons) {
                button.interactable = isEnable;
            }
        }

    }
}
