using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using PopcornMountain.MainGameScene.Popcorn;

namespace PopcornMountain.MainGameScene.BlazeBeans {
    public class BlazeBeansWarningPresenter : PhaseSwitchablePresenterMonobehavior {
        
        private void Start() {
            AddEnablePhases(GamePhase.Cooking);
            var blazeBeansAnimatorView = GetComponent<BlazeBeanAnimatorView>();
            var blazeBeanParticleView = GetComponent<BlazeBeanParticleView>();
            var placingStatusModel = GetComponent<PopcornPlacingStatusModel>();
            var cookingStatusModel = GetComponent<PopcornCookingStatusModel>();

            cookingStatusModel.OnHeatChangedObservable
                .Where(_ => isEnablePhase)
                .TakeUntil(cookingStatusModel.OnPoppedObservable)
                .Select(_ => cookingStatusModel.GetHeatPercentToPop())
                .Subscribe(
                    x => {
                        blazeBeansAnimatorView.ApplyWarnAnimation(x);
                        blazeBeanParticleView.SetBlazeFragmentParticle(x * 5);
                    },
                    () => {
                        blazeBeanParticleView.StopBlazeFragmentParticle();
                    }
                )
                .AddTo(this);
        }
        
    }
}
