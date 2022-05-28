using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornPenetrationFixerView : BaseMainGameComponent {

        private Rigidbody2D penetrationSimulateObject = null;
        private Rigidbody2D strictCollider = null;
        private Rigidbody2D _rigidbody = null;
        private PolygonCollider2D _collider = null;


        private void Start() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = transform.Find("Collider").GetComponent<PolygonCollider2D>();

            strictCollider = transform.Find("StrictCollider").GetComponent<Rigidbody2D>();
            strictCollider?.GetComponent<PolygonCollider2D>()?.SetPath(0, _collider.GetPath(0));
        }


        private async UniTask CreateDummy2() {
            // FixedUpdateのタイミングまで待つ
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

            // ダミーオブジェクトを生成する
            var dummyCollider = Instantiate(_collider);
            dummyCollider.isTrigger = false;
            
            var dummyObject = dummyCollider.gameObject;
            dummyObject.layer = LayerMask.NameToLayer("PopcornStrict");
            dummyObject.transform.localScale = transform.localScale;
            
            penetrationSimulateObject = dummyObject.AddComponent<Rigidbody2D>();
            penetrationSimulateObject.position = _rigidbody.position;
            penetrationSimulateObject.rotation = _rigidbody.rotation;
            penetrationSimulateObject.bodyType = RigidbodyType2D.Dynamic;
            penetrationSimulateObject.gravityScale = 0f;
            penetrationSimulateObject.mass = 0f;
            penetrationSimulateObject.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }


        public async UniTask<bool> ResolvePenetration() {
            strictCollider.simulated = false;
            await CreateDummy2();

            await UniTask.DelayFrame(2, PlayerLoopTiming.FixedUpdate);

            var fixedMoveVector = penetrationSimulateObject.position - _rigidbody.position;
            const float margin = 0.1f;
            penetrationSimulateObject.position += new Vector2(
                (fixedMoveVector.x < 0f) ? -margin : ((fixedMoveVector.x > 0f) ? margin : 0f),
                (fixedMoveVector.y < 0f) ? -margin : ((fixedMoveVector.y > 0f) ? margin : 0f)
            );

            await UniTask.DelayFrame(1, PlayerLoopTiming.FixedUpdate);

            bool isTouching = penetrationSimulateObject.IsTouchingLayers(LayerMask.GetMask("PopcornStrict"));
            Destroy(penetrationSimulateObject.gameObject);
            strictCollider.simulated = true;

            if (!isTouching) {
                _rigidbody.position = penetrationSimulateObject.position;
                _rigidbody.rotation = penetrationSimulateObject.rotation;
            }

            return !isTouching;
        }

    }
}
