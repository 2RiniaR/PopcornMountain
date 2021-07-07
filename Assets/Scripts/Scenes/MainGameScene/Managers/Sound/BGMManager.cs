using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class BGMManager : BaseMainGameManager {

        #region 定数

        /// <summary>
        ///   Resourcesフォルダを起点とした、プレイヤーを待っている間のBGMへのパス
        /// </summary>
        private const string waitingPlayerBgmPath = "Sounds/BGM/StartMenuSceneBGM";

        /// <summary>
        ///   Resourcesフォルダを起点とした、ゲーム中BGMへのパス
        /// </summary>
        private const string playingGameBgmPath = "Sounds/BGM/MainGameSceneBGM";

        #endregion


        #region コンポーネント参照

        private PhaseManager phaseManager = GetManager<PhaseManager>();
        private PlayerManager playerManager = GetManager<PlayerManager>();

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   Subscribeしたパイプラインを一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        #endregion


        #region 初期化用の関数

        public override void BeforeLoadScene() {
            var waitingPlayerBGM = Resources.Load(waitingPlayerBgmPath) as AudioClip;
            var playingGameBgm = Resources.Load(playingGameBgmPath) as AudioClip;

            // シーンが開始されたとき、BGMを再生する
            phaseManager.OnEnterPhase(GamePhase.WaitingPlayers)
                .Subscribe(_ => AudioManager.Instance.ChangeBGM(waitingPlayerBGM))
                .AddTo(onDispose);

            // ゲーム開始演出が再生され始めたとき、BGMを停止する
            phaseManager.OnExitPhase(GamePhase.WaitingPlayers)
                .Subscribe(_ => AudioManager.Instance.StopBGM())
                .AddTo(onDispose);

            // 料理フェーズが開始したら、BGMを再生する
            phaseManager.OnEnterPhase(GamePhase.Cooking)
                .Subscribe(_ => AudioManager.Instance.ChangeBGM(playingGameBgm))
                .AddTo(onDispose);

            // 勝者が決定したとき、BGMを停止する
            phaseManager.OnExitPhase(GamePhase.Cooking)
                .Subscribe(_ => AudioManager.Instance.StopBGM())
                .AddTo(onDispose);
        }

        #endregion


        #region 終了処理用の関数

        public override void Dispose() {
            onDispose.Dispose();
        }

        #endregion

    }
}
