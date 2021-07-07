using System;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene.Lid {
    public class LidView : BaseMainGameComponent {

        private void Start() {
            Observable.Timer(TimeSpan.FromSeconds(14f))
                .First()
                .Subscribe(
                    _ => { },
                    () => Destroy(gameObject)
                )
                .AddTo(this);
        }

        private void OnTriggerEnter(Collider other){
            if (other.tag == "FallenArea") {
                Destroy(gameObject);
            }
        }

    }
}
