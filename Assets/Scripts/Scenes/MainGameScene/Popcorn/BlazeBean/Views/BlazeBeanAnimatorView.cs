using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace PopcornMountain.MainGameScene.BlazeBeans {
    public class BlazeBeanAnimatorView : BaseMainGameComponent {

        private Animator _animator = null;

        private void OnEnable() {
            _animator = GetComponent<Animator>();
        }

        public void ApplyWarnAnimation(float popProgress) {
            _animator.SetFloat("PopProgress", popProgress);
        }

    }
}
