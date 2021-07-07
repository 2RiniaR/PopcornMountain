using UnityEngine;
using System;
using UniRx;
using UniRx.Async;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornMoverPresenter : PhaseSwitchablePresenterMonobehavior {

        private PopcornClickView clickView = null;
        private PopcornAudioView audioView = null;
        private PopcornColliderView colliderView = null;
        private PopcornPlacingStatusModel placingStatusModel = null;
        private PopcornDummyView dummyView = null;
        private PopcornPositionView positionView = null;
        private PopcornRigidbodyView rigidbodyView = null;
        private PopcornPenetrationFixerView penetrationFixerView = null;
        private PopcornCookingStatusModel cookingStatusModel = null;
        private FaintManager faintManager = null;
        private HandManager handManager = null;
        private PopcornImpactReceiverView impactReceiverView = null;
        private PopcornImpactView impactView = null;


        private void Start() {
            AddEnablePhases(GamePhase.Cooking);
            clickView = GetComponent<PopcornClickView>();
            colliderView = GetComponent<PopcornColliderView>();
            placingStatusModel = GetComponent<PopcornPlacingStatusModel>();
            var catchingStatusModel = GetComponent<PopcornCatchingStatusModel>();
            cookingStatusModel = GetComponent<PopcornCookingStatusModel>();
            dummyView = GetComponent<PopcornDummyView>();
            positionView = GetComponent<PopcornPositionView>();
            rigidbodyView = GetComponent<PopcornRigidbodyView>();
            audioView = GetComponent<PopcornAudioView>();
            faintManager = GetManager<FaintManager>();
            handManager = GetManager<HandManager>();
            impactReceiverView = transform.Find("Collider").GetComponent<PopcornImpactReceiverView>();
            penetrationFixerView = GetComponent<PopcornPenetrationFixerView>();
            impactView = GetComponent<PopcornImpactView>();


            // ドラッグが開始された時、持ち上げる
            clickView.OnDragBeginObservable
                .Where(_ => isEnablePhase)
                .Where(_ => !catchingStatusModel.IsCatchable)
                .Where(_ => !faintManager.IsFainting)
                .AsUnitObservable()
                .Subscribe(async _ => await Move())
                .AddTo(this);
        }


        private void BeginMove() {
            audioView.PlayPickupSound();
            dummyView.CreateDummy();
            rigidbodyView.UnableCollision();
            colliderView.EnableTrigger();
            rigidbodyView.ResetVelocity();
            placingStatusModel.Pickup();
        }


        private async UniTask PlaceCheck() {
            bool isMovingAccepted = await penetrationFixerView.ResolvePenetration();
            if (isMovingAccepted) {
                Place();
            } else {
                Cancel();
            }
        }


        private void Place() {
            placingStatusModel.Place();
            EndMove();
        }


        private void Cancel() {
            positionView.MoveTo(dummyView.dummyPosition, dummyView.dummyRotation);
            placingStatusModel.CancelMove();
            EndMove();
        }


        private void EndMove() {
            dummyView.DestroyDummy();
            colliderView.UnableTrigger();
            rigidbodyView.EnableCollision();
        }


        private async UniTask Move() {
            if (placingStatusModel.IsMoving) return;

            BeginMove();

            using (var onEndMove = new CompositeDisposable()) {

                // 終了条件のObservable
                var cancelObservable = Observable.Merge(

                    // ドラッグが終了したとき、移動する
                    clickView.OnDragEndObservable
                        .AsUnitObservable()
                        .Do(async _ => await PlaceCheck()),

                    // 持ち上げているときに気絶したとき、強制的に移動する
                    faintManager.OnChangeIsFaintingObservable
                        .AsUnitObservable()
                        .Do(async _ => await PlaceCheck()),

                    // 持ち上げているときに破裂したとき、強制的に移動をキャンセルする
                    cookingStatusModel.OnPoppedObservable
                        .Do(_ => Cancel()),

                    // 持ち上げているときに手の耐久値が0になったら、強制的に移動する
                    handManager.OnDurabilityEmptyObservable
                        .Do(async _ => await PlaceCheck()),

                    // 持ち上げているときに一定以上の強さの爆発を受けたら、強制的に移動をキャンセルして吹き飛ぶようにする
                    impactReceiverView.OnReceiveObservable
                        .Where(force => force.magnitude > 0.2f)
                        .Do(force => {
                            Cancel();
                            impactView.Impact(force);
                        })
                        .AsUnitObservable(),

                    // 持ち上げているときに移動制限エリアに触れたら、強制的に移動をキャンセルする
                    colliderView.OnHitToMoveCancelArea
                        .Do(_ => Cancel()),

                    // 持ち上げているときにゲームオブジェクトが破棄されたとき、ストリームの購読を終了する
                    this.OnDestroyAsObservable()

                );


                // 移動中の処理
                await Observable.EveryFixedUpdate()
                    .WithLatestFrom(clickView.OnPointerObservable, (_, pos) => pos)
                    .Do(worldPosition => positionView.MoveTo(worldPosition))
                    .TakeUntil(cancelObservable)
                    .AsSingleUnitObservable();
            }
        }

    }
}
