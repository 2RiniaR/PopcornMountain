using UniRx;
using System;


namespace PopcornMountain.MainGameScene.ResultWindow {
    public class ResultWindowShowPresenter : BaseMainGameComponent {

        private void Start() {
            var animationView = GetComponent<ResultWindowAnimationView>();
            var phaseManager = GetManager<PhaseManager>();
            var playerManager = GetManager<PlayerManager>();

            phaseManager.OnEnterPhase(GamePhase.ShowingResult)
                .WithLatestFrom(
                    Observable.Merge(
                        playerManager.OnWinnerDecidedObservable,
                        PhotonManager.callbacks.OnPlayerLeftRoomObservable.Select(_ => PlayerID.Self).Publish().RefCount()
                    ),
                    (_, winner) => winner
                )
                .Subscribe(playerID => animationView.CreateShowAnimationObservable(playerID).Subscribe().AddTo(this))
                .AddTo(this);
        }

    }
}
