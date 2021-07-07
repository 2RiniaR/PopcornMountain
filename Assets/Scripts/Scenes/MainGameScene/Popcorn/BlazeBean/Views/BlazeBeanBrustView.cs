using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace PopcornMountain.MainGameScene.BlazeBeans {
    public class BlazeBeanBrustView : BaseMainGameComponent {

        [SerializeField]
        private BlazeBeanBrust brustObject = null;
        
        public void Brust(float power) {
            if (brustObject == null) return;
            var inst = Instantiate(brustObject, transform.position, transform.rotation, transform.parent);
            inst.power = power;
            inst.sourceObject = gameObject;
            inst.Explosion();
        }

    }
}
