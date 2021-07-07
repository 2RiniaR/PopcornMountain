using System;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {

    /// <summary>
    ///   ゲームフェーズの列挙
    /// </summary>
    public enum GamePhase {
        Any,
        Init,
        
        /// <summary>
        ///   1. 相手プレイヤーの入室を待機している状態
        /// </summary>
        WaitingPlayers,

        /// <summary>
        ///   2. ゲーム開始前の演出が再生されている状態
        /// </summary>
        PlayingGameStartEffects,

        /// <summary>
        ///   3. プレイヤーが調理をしている状態
        /// </summary>
        Cooking,

        /// <summary>
        ///   4. ゲームの結果が表示されている状態
        /// </summary>
        ShowingResult,

        /// <summary>
        ///   5. 相手との接続が切れた旨の通知が表示されている状態
        /// </summary>
        LostConnection,
    }


    public class PhaseManager : BaseMainGameManager {

        #region 子コンポーネント

        private PhaseStateChanger stateChanger = new PhaseStateChanger();

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   Subscribeしたパイプラインを一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        /// <summary>
        ///   各ゲームフェーズが開始したときにOnNext()が発行されるObservable
        /// </summary>
        private IObservable<Unit>[] onEnterPhaseObservables = null;

        /// <summary>
        ///   各ゲームフェーズが開始したときにOnNext()が発行されるObservable
        /// </summary>
        private IObservable<Unit>[] onExitPhaseObservables = null;

        /// <summary>
        ///   勝者が決定したときにOnNext()とOnCompleted()が発行されるSubject
        /// </summary>
        private Subject<PlayerID> onEndBattle = new Subject<PlayerID>();

        #endregion


        #region 公開するプロパティ

        /// <summary>
        ///   現在のゲームフェーズ
        /// </summary>
        public GamePhase CurrentGamePhase { get { return stateChanger.CurrentGamePhase; } }

        #endregion


        #region 公開するObservable

        public IObservable<GamePhase> OnChangeGamePhaseObservable { get { return stateChanger.OnChangeGamePhaseObservable; } }

        #endregion


        #region 初期化用の関数

        public override void BeforeLoadScene() {
            GenerateEnterAndExitPhaseObservable();
        }


        /// <summary>
        ///   各ゲームフェーズの開始/終了を通知するObservableを初期化する
        /// </summary>
        private void GenerateEnterAndExitPhaseObservable() {
            // 直前の状態と現在の状態をペアで記録するObservable
            IObservable<Pair<GamePhase>> stateBufferObservable = OnChangeGamePhaseObservable.Pairwise().Publish().RefCount();

            // 状態の総数
            int stateNum = Enum.GetNames(typeof(GamePhase)).Length;

            // 予め参照だけを生成しておく
            onEnterPhaseObservables = new IObservable<Unit>[stateNum];
            onExitPhaseObservables = new IObservable<Unit>[stateNum];

            // 各Observableを生成する
            for(int i=0; i<stateNum; i++) {
                GamePhase thisState = (GamePhase)Enum.ToObject(typeof(GamePhase), i);

                // 状態に入った時にOnNextされるObservable
                onEnterPhaseObservables[i] = stateBufferObservable
                    .Where(state => state.Previous != thisState && state.Current == thisState)
                    .AsUnitObservable()
                    .Publish()
                    .RefCount();

                // 状態から出た時にOnNextされるObservable
                onExitPhaseObservables[i] = stateBufferObservable
                    .Where(state => state.Previous == thisState && state.Current != thisState)
                    .AsUnitObservable()
                    .Publish()
                    .RefCount();
            }
        }


        public override void AfterLoadScene() {
            stateChanger.AfterLoadScene();
        }

        #endregion


        #region 終了処理用の関数

        public override void Dispose() {
            stateChanger.Dispose();
            onDispose.Dispose();
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   指定したゲームフェーズが開始したときに、OnNext()が発行されるObservableを返す
        /// </summary>
        /// <param name="scene">対象となるゲームフェーズ</para>
        /// <param name="isPublishOnStay">trueの場合、Subscribe時に既に対象のゲームフェーズが実行されていたらOnNext()を発行する</para>
        public IObservable<Unit> OnEnterPhase(GamePhase state, bool isPublishOnStay = true) {
            if (isPublishOnStay) {
                return Observable.Merge(
                    Observable.Defer(
                        () => {
                            if (CurrentGamePhase == state) return Observable.ReturnUnit();
                            else return Observable.Empty<Unit>();
                        }
                    ),
                    OnEnterPhase(state, isPublishOnStay: false)
                );
            } else {
                return onEnterPhaseObservables[(int)state];
            }
        }


        /// <summary>
        ///   指定したゲームフェーズが終了したときに、OnNext()が発行されるObservableを返す
        /// </summary>
        /// <param name="scene">対象となるゲームフェーズ</para>
        public IObservable<Unit> OnExitPhase(GamePhase state) {
            return onExitPhaseObservables[(int)state];
        }

        #endregion

    }
}
