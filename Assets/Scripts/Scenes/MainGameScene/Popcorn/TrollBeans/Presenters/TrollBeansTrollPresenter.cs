using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using PopcornMountain.MainGameScene.Popcorn;

namespace PopcornMountain.MainGameScene.TrollBeans {
    public class TrollBeansTrollPresenter : PhaseSwitchablePresenterMonobehavior {
        
        private void Start() {
            AddEnablePhases(GamePhase.Cooking);
            var trollBeansAnimatorView = GetComponent<TrollBeansAnimatorView>();
            var clickView = GetComponent<PopcornClickView>();

            this.UpdateAsObservable()
                .Where(_ => isEnablePhase)
                .Subscribe(_ => {
                    trollBeansAnimatorView.SetTroll(clickView.isMouseOver);
                })
                .AddTo(this);
        }
        
    }
}
