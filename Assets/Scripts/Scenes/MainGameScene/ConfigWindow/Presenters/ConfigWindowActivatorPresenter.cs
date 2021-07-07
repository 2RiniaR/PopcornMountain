using UniRx;
using PopcornMountain.ConfigWindow;


namespace PopcornMountain.MainGameScene {
    public class ConfigWindowActivatorPresenter : BaseMainGameComponent {

        private void Start() {
            var elementView = GetComponent<ConfigWindowElementView>();
            var animatorView = GetComponent<ConfigWindowAnimatorView>();
            var openButtonView = GetComponent<ConfigWindowOpenButtonView>();
            var exitButtonView = GetComponent<ConfigWindowExitButtonView>();
            var phaseManager = GetManager<PhaseManager>();


            animatorView.OnShowAnimationFinishObservable
                .Subscribe(_ => exitButtonView.SetEnable(true))
                .AddTo(this);

            animatorView.OnHideAnimationStartObservable
                .Subscribe(_ => exitButtonView.SetEnable(false))
                .AddTo(this);

            phaseManager.OnChangeGamePhaseObservable
                .Select(phase => {
                    return phase == GamePhase.WaitingPlayers ||
                        phase == GamePhase.PlayingGameStartEffects ||
                        phase == GamePhase.Cooking;
                })
                .Subscribe(isEnable => {
                    openButtonView.SetEnable(isEnable);
                    if (!isEnable) {
                        animatorView.CreateHideAnimationObservable().Subscribe().AddTo(this);
                    }
                })
                .AddTo(this);
        }

    }
}
