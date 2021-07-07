using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornPopView : BaseMainGameComponent {

        [SerializeField]
        private PolygonCollider2D poppedColliderPrefab = null;
        private PolygonCollider2D _collider = null;


        private void Awake() {
            _collider = transform.Find("Collider").GetComponent<PolygonCollider2D>();
        }

        private void ChangeColliderToPopped() {
            _collider.SetPath(0, poppedColliderPrefab.GetPath(0));
        }


        public void ChangeToPopped() {
            ChangeColliderToPopped();
        }

    }
}
