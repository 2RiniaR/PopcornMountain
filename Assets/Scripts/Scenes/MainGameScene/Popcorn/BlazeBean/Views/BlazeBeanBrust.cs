using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.BlazeBeans {
    public class BlazeBeanBrust : BaseMainGameComponent {

        private Collider2D explosionCollider = null;
        public float power = 0f;
        public GameObject sourceObject = null;


        private void Awake() {
            explosionCollider = GetComponent<Collider2D>();
        }


        public void Explosion() {
            // 1フレーム後に削除する
            Observable.TimerFrame(1, FrameCountType.FixedUpdate)
                .DoOnSubscribe(() => explosionCollider.enabled = true)
                .First()
                .Subscribe(
                    _ => { },
                    () => Destroy(gameObject)
                )
                .AddTo(this);

            // 爆発の際に力を加える
            explosionCollider.OnTriggerEnter2DAsObservable()
                .Where(other => other.gameObject != sourceObject)
                .Select(other => other.gameObject.GetComponent<Popcorn.PopcornHeatReceiverView>())
                .Where(other => other != null)
                .Subscribe(other => {
                    float heat = CalculateHeat(other.transform, power);
                    other.Issue(heat);
                })
                .AddTo(this);
        }


        private float CalculateHeat(Transform other, float power) {
            // 対象オブジェクトへ向かうベクトル
            Vector2 vectorBetweenObjects = (other.transform.position - transform.position);
            // 対象オブジェクトへの距離
            float distance = vectorBetweenObjects.magnitude;
            return power / (distance + 1.0f);
        }

    }
}
