using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace PopcornMountain.MainGameScene.TrollBeans {
    public class TrollBeansAnimatorView : BaseMainGameComponent {

        private Animator _animator;

        private void Start() {
            _animator = GetComponent<Animator>();
        }

        public void SetTroll(bool isTroll) {
            _animator.SetBool("isTroll", isTroll);
        }

    }
}
