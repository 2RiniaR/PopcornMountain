using System;
using UnityEngine;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornImpactView : MonoBehaviour {

        private Rigidbody2D _rigidbody = null;


        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
        }


        public void Impact(Vector2 force) {
            _rigidbody.AddForce(force, ForceMode2D.Impulse);
        }

    }
}
