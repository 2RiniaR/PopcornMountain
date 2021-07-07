using System;
using UniRx;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornCatchingStatusModel : BaseMainGameComponent {

        #region 非公開のプロパティ

        /// <summary>
        ///   「ポップコーンが現在のプレイヤーにキャッチされた瞬間」にOnNext()が発行されるSubject
        /// </summary>
        private Subject<Unit> onCatched = new Subject<Unit>();

        /// <summary>
        ///   「ポップコーンが落下エリアに触れた瞬間」にOnNext()が発行されるSubject
        /// </summary>
        private Subject<Unit> onFallen = new Subject<Unit>();

        /// <summary>
        ///   「ポップコーンが送信エリアに触れた瞬間」にOnNext()が発行されるSubject
        /// </summary>
        private Subject<Unit> onFlown = new Subject<Unit>();

        #endregion


        #region 公開するプロパティ

        /// <summary>
        ///   取得可能かどうか
        /// </summary>
        public bool IsCatchable { get; private set; } = false;

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   「ポップコーンが現在のプレイヤーにキャッチされた瞬間」にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnCatchedObservable { get { return onCatched; } }

        /// <summary>
        ///   「ポップコーンが落下エリアに触れた瞬間」にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnFallenObservable { get { return onFallen; } }

        /// <summary>
        ///   「ポップコーンが送信エリアに触れた瞬間」にOnNext()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnFlownObservable { get { return onFlown; } }

        #endregion


        #region 初期化用の関数

        private void Start() {
            var cookingModel = GetComponent<PopcornCookingStatusModel>();

            // 「初期状態が破裂後である」または「ポップコーンが弾けた」とき、一度だけ状態をCatchableにする
            cookingModel.OnPoppedObservable
                .Subscribe(
                    _ => {},
                    () => IsCatchable = true
                )
                .AddTo(this);
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   ポップコーンを現在のプレイヤーにキャッチされた状態にする
        /// </summary>
        public void Catch() {
            onCatched.OnNext(Unit.Default);
        }

        /// <summary>
        ///   ポップコーンを落下エリアに触れた状態にする
        /// </summary>
        public void Fall() {
            onFallen.OnNext(Unit.Default);
        }

        /// <summary>
        ///   ポップコーンを送信エリアに触れた状態にする
        /// </summary>
        public void Fly() {
            onFlown.OnNext(Unit.Default);
        }

        #endregion

    }
}
