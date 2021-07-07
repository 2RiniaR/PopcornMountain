using System;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornPlacingStatusModel : BaseMainGameComponent {

        #region 非公開のプロパティ

        /// <summary>
        ///   「ポップコーンを設置した瞬間」にOnNext()が実行されるSubject
        /// </summary>
        private Subject<Unit> onPlace = new Subject<Unit>();

        /// <summary>
        ///   「ポップコーンの移動をキャンセルした瞬間」にOnNext()が実行されるSubject
        /// </summary>
        private Subject<Unit> onCancelMove = new Subject<Unit>();

        /// <summary>
        ///   「ポップコーンを持ち上げた瞬間」にOnNext()が実行されるSubject
        /// </summary>
        private Subject<Unit> onHeld = new Subject<Unit>();

        #endregion


        #region 公開するプロパティ

        /// <summary>
        ///   現在移動中かどうか
        /// </summary>
        public bool IsMoving { get; private set; } = false;

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   「ポップコーンを設置した瞬間」にOnNext()が実行されるObservable
        /// </summary>
        public IObservable<Unit> OnPlacedObservable { get { return onPlace; } }

        /// <summary>
        ///   「ポップコーンの移動をキャンセルした瞬間」にOnNext()が実行されるObservable
        /// </summary>
        public IObservable<Unit> OnCanceledMoveObservable { get { return onCancelMove; } }

        /// <summary>
        ///   「ポップコーンを持ち上げた瞬間」にOnNext()が実行されるObservable
        /// </summary>
        public IObservable<Unit> OnHeldObservable { get { return onHeld; } }

        /// <summary>
        ///   <para>「ポップコーンを離した瞬間」にOnNext()が実行されるObservable</para>
        ///   <para>OnCancelMoveとOnPickupをMergeしたもの</para>
        /// </summary>
        public IObservable<Unit> OnReleasedObservable { get; private set; }

        #endregion


        #region 初期化用の関数

        private void Awake() {
            // OnReleasedObservableを初期化する
            OnReleasedObservable = Observable
                .Merge(onPlace, onCancelMove, this.OnDestroyAsObservable().Where(_ => IsMoving))
                .Publish()
                .RefCount();
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   <para>ポップコーンを持ち上げた状態にする</para>
        /// </summary>
        public void Pickup() {
            if (IsMoving) return;
            IsMoving = true;
            onHeld.OnNext(Unit.Default);
        }

        /// <summary>
        ///   <para>ポップコーンを設置した状態にする</para>
        /// </summary>
        public void Place() {
            if (!IsMoving) return;
            IsMoving = false;
            onPlace.OnNext(Unit.Default);
        }

        /// <summary>
        ///   <para>ポップコーンの移動をキャンセルする</para>
        /// </summary>
        public void CancelMove() {
            if (!IsMoving) return;
            IsMoving = false;
            onCancelMove.OnNext(Unit.Default);
        }

        #endregion

    }
}
