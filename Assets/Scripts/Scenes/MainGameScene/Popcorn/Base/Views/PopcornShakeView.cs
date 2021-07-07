using UnityEngine;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornShakeView : MonoBehaviour {

        private Rigidbody2D _rigidbody;
        private float power = 0f;


        private void Awake() {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            Shake(power);
        }

        private void Shake(float power) {
            if (power <= 0 || UnityEngine.Random.Range(0f, 4f) > Mathf.Pow(power, 2f)) return;
            _rigidbody.AddForceAtPosition(
                _rigidbody.mass * new Vector2(0, UnityEngine.Random.Range(-0.5f, 0f)),
                (Vector2)transform.position + Vector2.Scale(transform.localScale, new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))),
                ForceMode2D.Impulse
            );
        }


        public void SetPower(float power) {
            this.power = power;
        }

        public void Stop() {
            this.power = 0f;
        }

    }
}
