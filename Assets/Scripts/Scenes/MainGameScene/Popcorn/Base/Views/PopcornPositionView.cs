using UnityEngine;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornPositionView : BaseMainGameComponent {

        private Rigidbody2D _rigidbody = null;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
        }


        public void MoveTo(Vector2? worldPosition = null, float? rotation = null) {
            if (worldPosition.HasValue) {
                _rigidbody.position = worldPosition.Value;
            }
            if (rotation.HasValue) {
                _rigidbody.rotation = rotation.Value;
            }
        }

    }
}
