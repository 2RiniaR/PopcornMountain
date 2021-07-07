using UniRx;


namespace PopcornMountain.MainGameScene.SendedBeans {
    public class PopcornSenderPresenter : BaseMainGameComponent {

        private void Start() {
            var sendedBeansAnimationView = GetComponent<SendedBeansAnimationView>();
            var beansManager = GetManager<BeansManager>();

            beansManager.OnSendBeans
                .Subscribe(x => sendedBeansAnimationView.CreatePlaySendAnimationObservable(x.Item1, x.Item2).Subscribe().AddTo(this))
                .AddTo(this);
        }

    }
}
