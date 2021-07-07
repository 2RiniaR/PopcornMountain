using UniRx;


namespace PopcornMountain.MainGameScene {
    public class HeaterPowerChangerPresenter : BaseMainGameComponent {

        private void Start() {
            var heatAnimationView = GetComponent<HeatAnimationView>();
            var heatControlButtonView = GetComponent<HeatControlButtonView>();
            var fireManager = GetManager<FireManager>();
            var faintManager = GetManager<FaintManager>();
            var phaseManager = GetManager<PhaseManager>();

            // 初期状態では、スライダーのボタンを無効化する
            heatControlButtonView.SetEnable(false);

            // スライダーがクリックされた時、火力を変更する
            heatControlButtonView.OnPowerButtonClickedObservable
                .Subscribe(power => fireManager.ChangeFirePower(power))
                .AddTo(this);

            // スライダーの表示を火力に合わせて変更する
            fireManager.OnFirePowerChangedObservable
                .Subscribe(power => {
                    heatAnimationView.SetPower(power, FireManager.maxPower);
                    heatControlButtonView.SetSliderPowerDisplay(power);
                })
                .AddTo(this);

            // 気絶状態になったとき、スライダーのボタンを無効化する
            faintManager.OnChangeIsFaintingObservable
                .Subscribe(isFainting => heatControlButtonView.SetEnable(!isFainting))
                .AddTo(this);

            // 調理フェーズ以外では、スライダーのボタンを無効化する
            phaseManager.OnChangeGamePhaseObservable
                .Select(phase => phase == GamePhase.Cooking)
                .Subscribe(x => heatControlButtonView.SetEnable(x))
                .AddTo(this);

            // 調理フェーズ開始時、火力を1にする
            phaseManager.OnEnterPhase(GamePhase.Cooking)
                .Subscribe(_ => fireManager.ChangeFirePower(1))
                .AddTo(this);
        }

    }
}
