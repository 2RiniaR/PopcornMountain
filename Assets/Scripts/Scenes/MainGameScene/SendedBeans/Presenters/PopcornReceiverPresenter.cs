using UniRx;


namespace PopcornMountain.MainGameScene.SendedBeans {
    public class PopcornReceiverPresenter : BaseMainGameComponent {

        private void Start() {
            var sendedBeansAnimationView = GetComponent<SendedBeansAnimationView>();
            var beansManager = GetManager<BeansManager>();

            beansManager.OnReceiveBeans
                .Subscribe(x => {
                    sendedBeansAnimationView.CreatePlayReceiveAnimationObservable(x.Item1, x.Item2)
                        .Subscribe(
                            _ => {},
                            () => beansManager.InstantiatePopcorn(x.Item1, x.Item2)
                        )
                        .AddTo(this);
                })
                .AddTo(this);
        }

    }
}
