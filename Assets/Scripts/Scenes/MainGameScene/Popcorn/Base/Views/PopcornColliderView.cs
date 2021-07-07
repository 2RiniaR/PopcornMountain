using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornColliderView : BaseMainGameComponent {

        private Collider2D _collider = null;

        public IObservable<Unit> OnHitToFallenArea { get; private set; } = null;
        public IObservable<Unit> OnHitToFlownArea { get; private set; } = null;
        public IObservable<Unit> OnHitToMoveCancelArea { get; private set; } = null;


        private void Awake() {
            _collider = transform.Find("Collider").GetComponent<PolygonCollider2D>();

            // 落下判定エリアに触れたときに、発火する
            OnHitToFallenArea = _collider
                .OnTriggerEnter2DAsObservable()
                .Where(col => col.tag == "FallenArea")
                .AsUnitObservable()
                .Publish()
                .RefCount();

            // 送信判定エリアに触れたときに、発火する
            OnHitToFlownArea = _collider
                .OnTriggerEnter2DAsObservable()
                .Where(col => col.tag == "FlownArea")
                .AsUnitObservable()
                .Publish()
                .RefCount();

            OnHitToMoveCancelArea = _collider
                .OnTriggerStay2DAsObservable()
                .Where(col => col.tag == "ForceCancelMoveArea")
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }


        public void EnableTrigger() {
            _collider.isTrigger = true;
        }

        public void UnableTrigger() {
            _collider.isTrigger = false;
        }

    }
}
