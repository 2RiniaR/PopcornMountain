using UnityEngine;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornDummyView : BaseMainGameComponent {

        private GameObject dummyObject = null;
        private Rigidbody2D dummyRigidbody = null;
        private PopcornHeatReceiverView dummyHeatReceiver = null;
        private PopcornImpactReceiverView dummyImpactReceiver = null;

        private PolygonCollider2D _collider = null;
        private Rigidbody2D _rigidbody = null;
        private SpriteRenderer _spriteRenderer = null;
        private PopcornHeatReceiverView _heatReceiver = null;
        private PopcornImpactReceiverView _impactReceiver = null;


        public Vector2 dummyPosition { get { return dummyRigidbody.position; } }
        public float dummyRotation { get { return dummyRigidbody.rotation; } }


        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            var colliderObject = transform.Find("Collider");
            _heatReceiver = colliderObject.GetComponent<PopcornHeatReceiverView>();
            _impactReceiver = colliderObject.GetComponent<PopcornImpactReceiverView>();
            _collider = colliderObject.GetComponent<PolygonCollider2D>();
        }


        private void OnDestroy() {
            DestroyDummy();
        }


        public void CreateDummy() {
            if (dummyObject != null) return;

            // ダミーオブジェクトを生成する
            dummyObject = new GameObject(name + "(Dummy)");

            // Transformの設定
            dummyObject.transform.localScale = transform.localScale;
            dummyObject.transform.position = transform.position;
            dummyObject.transform.rotation = transform.rotation;

            // Rigidbodyの設定
            dummyRigidbody = dummyObject.AddComponent(_rigidbody);

            // Colliderの設定
            var dummyCollider = dummyObject.AddComponent(_collider);

            // Sprite Rendererの設定
            var dummyRenderer = dummyObject.AddComponent(_spriteRenderer);
            dummyRenderer.sortingLayerName = "GameObjects";
            dummyRenderer.sortingOrder = 1;
            dummyRenderer.color = new Color(1f, 1f, 1f, 0.5f);

            // 泡のエフェクトの設定
            // var dummyBubbleParticle = dummyObject.AddComponent<PopcornBubbleParticleView>();
            // dummyBubbleParticle.SetBubbleParticle

            // 熱の伝達元をダミーに変更する
            dummyHeatReceiver = dummyObject.AddComponent<PopcornHeatReceiverView>();
            _heatReceiver.SetDependent(dummyHeatReceiver);

            // 爆発力の伝達元をダミーに変更する
            dummyImpactReceiver = dummyObject.AddComponent<PopcornImpactReceiverView>();
            _impactReceiver.SetDependent(dummyImpactReceiver);

            // 自身の描画を前面にする
            _spriteRenderer.sortingOrder = 2;
        }


        public void DestroyDummy() {
            if (dummyObject == null) return;
            Destroy(dummyObject.gameObject);

            _spriteRenderer.sortingOrder = 1;

            _heatReceiver.ResetDependent();
            _impactReceiver.ResetDependent();
            dummyObject = null;
            dummyRigidbody = null;
            dummyHeatReceiver = null;
            dummyImpactReceiver = null;
        }

    }
}
