using UniRx;


namespace PopcornMountain.MainGameScene.Faint {
    public class FaintEffectPresenter : BaseMainGameComponent {

        private void Start() {
            var effectView = GetComponent<FaintRedEffectView>();
            var faintManager = GetManager<FaintManager>();

            // 気絶状態の変化に合わせて画面エフェクトのON/OFFを切り替える
            faintManager.OnChangeIsFaintingObservable
                .Subscribe(x => effectView.SetRedEffect(x))
                .AddTo(this);
        }

    }
}
