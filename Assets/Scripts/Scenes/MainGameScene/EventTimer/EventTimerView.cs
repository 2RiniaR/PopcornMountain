using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;


namespace PopcornMountain.MainGameScene.EventTimer {
    public class EventTimerView : MonoBehaviour {

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Image countCircle;
        [SerializeField]
        private TMPro.TextMeshProUGUI countText;


        private void SetCount(float count, float maxCount) {
            countText.text = ((int)Mathf.Ceil(count)).ToString();
            countCircle.fillAmount = count / maxCount;
        }

        private void Show() {
            _animator.SetBool("isShow", true);
        }

        private void Hide() {
            _animator.SetBool("isShow", false);
        }


        public IObservable<Unit> CreateCountAnimationObservable() {
            var timerObservable = Observable.EveryUpdate()
                .Select(_ => Time.deltaTime)
                .Scan((acc, current) => acc + current)
                .Select(x => 5f - x)
                .TakeWhile(x => x > 0f)
                .Concat(Observable.Return<float>(0f).First())
                .Do(x => SetCount(x, 5f))
                .AsUnitObservable()
                .Publish()
                .RefCount();

            return Observable.Concat(
                timerObservable,
                Observable.Timer(TimeSpan.FromSeconds(1f)).AsUnitObservable().Publish().RefCount()
            )
                .DoOnSubscribe(Show)
                .DoOnCompleted(Hide)
                .Publish()
                .RefCount();
        }
        
    }
}
