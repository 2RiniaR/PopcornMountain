using UniRx;


namespace PopcornMountain.MainGameScene.EventTimer {
    public class EventTimerPresenter : BaseMainGameComponent {

        private void Start() {
            var timerView = GetComponent<EventTimerView>();
            var eventManager = GetManager<EventManager>();

            // イベントのカウントダウン開始通知が来たとき、カウントダウンを表示する
            eventManager.OnEventCountObservable
                .Subscribe(_ => timerView.CreateCountAnimationObservable().Subscribe().AddTo(this))
                .AddTo(this);
        }

    }
}
