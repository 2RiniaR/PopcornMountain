using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornExplosion : BaseMainGameComponent {

        private Collider2D explosionCollider;
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
            var selfColliderObject = sourceObject.transform.Find("Collider").gameObject;
            var selfImpactReceiverView = sourceObject?.GetComponent<PopcornImpactReceiverView>();
            explosionCollider.OnTriggerEnter2DAsObservable()
                .Where(other => other.gameObject != selfColliderObject)
                .Subscribe(other => {
                    var otherImpactReceiverView = other?.GetComponent<PopcornImpactReceiverView>();
                    if (otherImpactReceiverView == null) return;

                    Vector2 force = CalculateExplosionForce(other.gameObject, power);
                    otherImpactReceiverView.Issue(force);
                    if(selfImpactReceiverView != null) {
                        selfImpactReceiverView.Issue(-force);
                    }
                })
                .AddTo(this);
        }


        private Vector2 CalculateExplosionForce(GameObject other, float power) {
            // 対象オブジェクトへ向かうベクトル
            Vector2 vectorBetweenObjects = (other.transform.position - transform.position);
            // 対象オブジェクトへの距離
            float distance = vectorBetweenObjects.magnitude;
            if (distance == 0f) return new Vector2(0, 0);

            // 障害物に応じて爆発力を減少させる
            var hits = Physics2D.RaycastAll(
                transform.position,
                vectorBetweenObjects,
                distance,
                LayerMask.GetMask("Popcorn")
            );
            foreach (var obj in hits) {
                if (obj.transform.gameObject == gameObject || obj.transform.gameObject == other) continue;
                power /= obj.transform.GetComponent<PopcornParameterModel>()?.parameters.diffence ?? 1f;
            }

            // 衝撃が与えられる方向
            Vector2 forceDirection = vectorBetweenObjects.normalized;
            // 対象に与えられる力。大きさは距離に反比例する。
            Vector2 forceVector = Mathf.Pow(distance, -1) * power * forceDirection;
            // 反発力は対象に与える力の向きを逆にしたものを与える。
            return forceVector;
        }

    }
}
