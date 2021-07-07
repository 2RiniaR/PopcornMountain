using UnityEngine;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornExplosionView : BaseMainGameComponent {

        [SerializeField]
        private PopcornExplosion explosionObject = null;

        public void Explosion(float power) {
            if (explosionObject == null) return;
            var inst = Instantiate(explosionObject, transform.position, transform.rotation, transform.parent);
            inst.power = power;
            inst.sourceObject = gameObject;
            inst.Explosion();
        }

    }
}
