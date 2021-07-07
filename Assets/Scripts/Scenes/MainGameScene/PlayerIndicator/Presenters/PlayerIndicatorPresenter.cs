using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene.PlayerIndicator {
    public class PlayerIndicatorPresenter : BaseMainGameComponent {

        [SerializeField]
        private PlayerID playerID = PlayerID.Self;

        private void Start() {
            var nameView = GetComponent<PlayerIndicatorNameView>();
            var gaugeView = GetComponent<PlayerIndicatorGaugeView>();
            var phaseManager = GetManager<PhaseManager>();
            var playerManager = GetManager<PlayerManager>();
            var targetPlayer = playerManager.GetPlayer(playerID);

            phaseManager.OnExitPhase(GamePhase.WaitingPlayers)
                .Subscribe(_ => nameView.SetNickName(targetPlayer.parameters.nickname))
                .AddTo(this);

            targetPlayer.OnChangeHungerPoint
                .Subscribe(point => gaugeView.SetValue(point, targetPlayer.parameters.maxHungerPoint))
                .AddTo(this);

            targetPlayer.OnChangeHungerPoint
                .Pairwise()
                .Select(p => p.Current - p.Previous)
                .Subscribe(pointDif => {
                    if (pointDif > 0) {
                        gaugeView.PlayGetPointAnimation();
                    } else if (pointDif < 0) {
                        gaugeView.PlayLostPointAnimation();
                    }
                })
                .AddTo(this);

            targetPlayer.OnFullHungerPoint
                .Subscribe(_ => gaugeView.PlayMaxPointAnimation())
                .AddTo(this);
        }
        
    }
}
