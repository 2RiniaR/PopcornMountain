using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornClickView : BaseMainGameComponent {

        private ObservableEventTrigger eventTrigger = null;

        public IObservable<Vector2> OnClickObservable { get; private set; } = null;
        public IObservable<Vector2> OnPointerObservable { get; private set; } = null;
        public IObservable<Vector2> OnDragBeginObservable { get; private set; } = null;
        public IObservable<Vector2> OnDragEndObservable { get; private set; } = null;
        public bool isMouseOver { get; private set; } = false;


        private void Awake() {
            eventTrigger = GetComponent<ObservableEventTrigger>() ?? gameObject.AddComponent<ObservableEventTrigger>();

            OnClickObservable = eventTrigger
                .OnPointerDownAsObservable()
                .Select(pos => pos.position)
                .Publish()
                .RefCount();

            OnDragBeginObservable = eventTrigger
                .OnPointerDownAsObservable()
                .Select(_ => TransScreenPositionToWorldPoint(Input.mousePosition))
                .Publish()
                .RefCount();

            OnDragEndObservable = eventTrigger
                .OnPointerUpAsObservable()
                .Select(_ => TransScreenPositionToWorldPoint(Input.mousePosition))
                .Publish()
                .RefCount();

            OnPointerObservable = Observable.EveryUpdate()
                .Select(_ => TransScreenPositionToWorldPoint(Input.mousePosition))
                .Publish()
                .RefCount();
        }

        private void OnMouseOver() {
            isMouseOver = true;
        }

        private void LateUpdate() {
            isMouseOver = false;
        }

        private Vector2 TransScreenPositionToWorldPoint(Vector3 screenPosition) {
            return Camera.main.ScreenToWorldPoint(Vector3.Scale(screenPosition, new Vector3(1, 1, 0)));
        }

    }
}
