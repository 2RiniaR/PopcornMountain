using UnityEngine;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornRigidbodyView : BaseMainGameComponent {

        private Rigidbody2D _rigidbody;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void UnableCollision() {
            _rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody.freezeRotation = true;
        }

        public void EnableCollision() {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.freezeRotation = false;
        }

        public void FreezePosition() {
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        public void ResetVelocity() {
            _rigidbody.velocity = new Vector2(0, 0);
            _rigidbody.angularVelocity = 0f;
        }

        public void MakePlacedState() {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.constraints = RigidbodyConstraints2D.None;
            _rigidbody.simulated = true;
        }

        public void SetMass(float mass) {
            _rigidbody.mass = mass;
        }

    }
}
