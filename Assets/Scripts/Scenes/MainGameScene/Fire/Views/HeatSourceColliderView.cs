using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene {
    public class HeatSourceColliderView : MonoBehaviour {

        [SerializeField]
        private Collider2D heatArea;
        [SerializeField]
        private Collider2D heatSource;

        public IObservable<Tuple<Popcorn.PopcornHeatReceiverView, float>> OnPopcornStayInHeatAreaObservable { get; private set; } = null;


        private void Awake() {
            OnPopcornStayInHeatAreaObservable = heatArea.OnTriggerStay2DAsObservable()
                .Select(col => new Tuple<Popcorn.PopcornHeatReceiverView, float>(
                    col.GetComponent<Popcorn.PopcornHeatReceiverView>(),
                    heatSource.Distance(col).distance
                ))
                .Where(x => x.Item1 != null)
                .Publish()
                .RefCount();
        }

    }
}
