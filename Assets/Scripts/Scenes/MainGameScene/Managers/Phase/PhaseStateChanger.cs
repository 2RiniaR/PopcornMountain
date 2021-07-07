using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PopcornMountain.MainGameScene {
    public class PhaseStateChanger : BaseMainGameManager {

        /// <summary>
        ///   ゲームフェーズ間の遷移を定義する構造体
        /// </summary>
        private struct PhaseTransition {
            /// <summary>
            ///   移動元のゲームフェーズ
            /// </summary>
            public GamePhase source;

            /// <summary>
            ///   移動先のゲームフェーズ
            /// </summary>
            public GamePhase destination;

            /// <summary>
            ///   OnCompleted()が発行されたタイミングで遷移が実行されるObservable
            /// </summary>
            public Func<IObservable<Unit>> observableGenerator;
        }


        #region 子コンポーネント

        private PhaseSender sender = null;

        #endregion


        #region コンポーネント参照

        private PlayerManager playerManager = GetManager<PlayerManager>();

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   Subscribeしたパイプラインを、Dispose()時に一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        /// <summary>
        ///   ゲームフェーズ遷移用にSubscribeしたパイプラインを、ゲームフェーズの切り替え時に一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onExitState = new CompositeDisposable();

        /// <summary>
        ///   各ゲームフェーズからの遷移用のObservable
        /// </summary>
        private List<PhaseTransition>[] transitionObservables = null;

        private ReactiveProperty<GamePhase> currentGamePhase = new ReactiveProperty<GamePhase>(GamePhase.Init);
        private IObservable<Unit> OnFinishSceneAnimationObservable = null;
        private IObservable<Unit> OnFinishStartAnimationObservable = null;

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   ゲームフェーズが変更されたときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<GamePhase> OnChangeGamePhaseObservable { get { return currentGamePhase; } }

        public GamePhase CurrentGamePhase { get { return currentGamePhase.Value; } }

        #endregion


        #region 初期化用の関数

        public override void BeforeLoadScene() {
            sender = GameObject.FindWithTag("PhotonView")?.GetComponent<PhaseSender>();

            var onFinishSceneAnimationObservable = sender.OnFinishSceneAnimationObservable.First().PublishLast();
            onFinishSceneAnimationObservable.Connect();
            OnFinishSceneAnimationObservable = onFinishSceneAnimationObservable;

            var onFinishStartAnimationObservable = sender.OnFinishStartAnimationObservable.First().PublishLast();
            onFinishStartAnimationObservable.Connect();
            OnFinishStartAnimationObservable = onFinishStartAnimationObservable;

            InitTransitions();

            // フェーズが変更されたとき、対象フェーズを起点とするObservableの購読を開始する
            OnChangeGamePhaseObservable.Subscribe(phase => {
                foreach (var transition in transitionObservables[(int)phase]) {
                    SubscribeTransition(transition);
                }
            })
            .AddTo(onDispose);
        }


        /// <summary>
        ///   各ゲームフェーズからの遷移用のObservableを初期化する
        /// </summary>
        private void InitTransitions() {
            var stateCount = Enum.GetNames(typeof(GamePhase)).Length;
            transitionObservables = new List<PhaseTransition>[stateCount];
            for (int i = 0; i < stateCount; i++) {
                transitionObservables[i] = new List<PhaseTransition>();
            }
        }


        public override void AfterLoadScene() {
            AddTransition(GamePhase.Init, GamePhase.WaitingPlayers, () => Observable.Empty<Unit>());
            AddTransition(GamePhase.WaitingPlayers, GamePhase.PlayingGameStartEffects, WaitPlayers);
            AddTransition(GamePhase.PlayingGameStartEffects, GamePhase.Cooking, PlayGameStartEffects);
            AddTransition(GamePhase.Cooking, GamePhase.ShowingResult, Game);

            AddTransition(GamePhase.WaitingPlayers, GamePhase.LostConnection, OnDisconnected);
            AddTransition(GamePhase.PlayingGameStartEffects, GamePhase.LostConnection, OnDisconnected);
            AddTransition(GamePhase.Cooking, GamePhase.LostConnection, OnDisconnected);
            AddTransition(GamePhase.LostConnection, GamePhase.ShowingResult, WaitErrorWindowSubmit);
        }

        #endregion


        #region 終了処理用の関数

        public override void Dispose() {
            onExitState.Dispose();
            onDispose.Dispose();
        }

        #endregion


        #region 非公開の関数

        private void SubscribeTransition(PhaseTransition transition) {
            transition.observableGenerator()
                .Subscribe(
                    _ => { },
                    () => ChangePhase(transition.destination)
                )
                .AddTo(onExitState);
        }


        /// <summary>
        ///   ゲームフェーズを変更する
        /// </summary>
        /// <param name="destination">変更先のゲームフェーズ</param>
        private void ChangePhase(GamePhase destination) {
            if (destination == GamePhase.Any || destination == GamePhase.Init) {
                throw new Exception("不正なGamePhaseへの遷移を試みました: " + destination);
            }
            onExitState.Dispose();
            onExitState = new CompositeDisposable();
            currentGamePhase.SetValueAndForceNotify(destination);
        }


        /// <summary>
        ///   ゲームフェーズの繊維を追加する
        /// </summary>
        /// <param name="source">遷移元のゲームフェーズ</param>
        /// <param name="destination">遷移先のゲームフェーズ</param>
        /// <param name="observableGenerator">OnCompleted()が発行されたタイミングで遷移が実行されるObservable</param>
        private void AddTransition(GamePhase source, GamePhase destination, Func<IObservable<Unit>> observableGenerator) {
            var transition = new PhaseTransition {
                source = source,
                destination = destination,
                observableGenerator = observableGenerator
            };
            transitionObservables[(int)source].Add(transition);
            if (CurrentGamePhase == source) {
                SubscribeTransition(transition);
            }
        }

        #endregion


        #region ゲームフェーズ遷移用の関数

        /// <summary>
        ///   相手プレイヤーの入室と、プレイヤー待機画面のアニメーション終了を待つ
        /// </summary>
        private IObservable<Unit> WaitPlayers() {
            var waitingPlayerLabelView = GameObject.FindWithTag("WaitingPlayerLabel")?.GetComponent<WaitingPlayerLabel.WaitingPlayerLabelView>();

            var showAnimationObservable = Observable.Defer(() => {
                return waitingPlayerLabelView.CreateShowAnimationObservable(playerManager.GetPlayer(PlayerID.Self).parameters.nickname);
            });

            // 部屋にいるプレイヤーの人数が2人になったときにOnCompleted()が発行されるObservable
            var allPlayersJoinObservable = PhotonManager.callbacks.OnPlayerEnteredRoomObservable
                .AsUnitObservable()
                .StartWith(Unit.Default)
                .Select(_ => PhotonManager.GetPlayerCountInCurrentRoom())
                .TakeWhile(playerCount => playerCount < PhotonManager.GetMaxPlayerInRoom())
                .AsUnitObservable()
                .Publish()
                .RefCount();

            // 相手へ通知を送り、帰ってきたらOnCompleted()が発行されるObservable
            var allPlayersFinishAnimationObservable = OnFinishSceneAnimationObservable
                .DoOnSubscribe(() => sender.SendFinishSceneAnimation())
                .DoOnCompleted(() => sender.SendFinishSceneAnimation())
                .Publish()
                .RefCount();

            var joinAnimationObservable = Observable.Defer(() => {
                return waitingPlayerLabelView.CreateJoinAnimationObservable(playerManager.GetPlayer(PlayerID.Other).parameters.nickname);
            });

            return Observable.Concat(
                showAnimationObservable,
                allPlayersJoinObservable,
                allPlayersFinishAnimationObservable,
                joinAnimationObservable
            );
        }


        private IObservable<Unit> PlayGameStartEffects() {
            var startEffectView = GameObject.FindWithTag("StartEffect")?.GetComponent<StartEffect.StartEffectView>();
            var labelView = GameObject.FindWithTag("Label")?.GetComponent<Label.LabelView>();
            var fireTurnAnimationView = GameObject.FindWithTag("Handle")?.GetComponent<HandleAnimationView>();

            var syncOtherObservable = OnFinishStartAnimationObservable
                .DoOnSubscribe(sender.SendFinishStartAnimation)
                .Publish()
                .RefCount();

            return Observable.Concat(
                    startEffectView.CreatePlayingObservable(),
                    labelView.CreatePlayingObservable("まんぷく を めざせ！"),
                    syncOtherObservable,
                    fireTurnAnimationView.CreateTurnOnObservable()
                );
        }


        private IObservable<Unit> Game() {
            return playerManager.OnWinnerDecidedObservable
                .First()
                .Delay(TimeSpan.FromSeconds(1f))
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }


        private IObservable<Unit> OnDisconnected() {
            return PhotonManager.callbacks.OnPlayerLeftRoomObservable
                .AsUnitObservable()
                .First()
                .Publish()
                .RefCount();
        }


        private IObservable<Unit> WaitErrorWindowSubmit() {
            var errorWindowView = GameObject.FindWithTag("ErrorWindow")?.GetComponent<ErrorWindow.ErrorWindowView>();
            return errorWindowView.CreateShowAnimationObservable("通信エラー", "相手プレイヤーの通信が切断されました。");
        }

        #endregion

    }
}
