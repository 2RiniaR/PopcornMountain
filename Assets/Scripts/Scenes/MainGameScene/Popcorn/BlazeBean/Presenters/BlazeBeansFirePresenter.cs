using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using PopcornMountain.MainGameScene.Popcorn;

namespace PopcornMountain.MainGameScene.BlazeBeans {
    public class BlazeBeansFirePresenter : PhaseSwitchablePresenterMonobehavior {
        
        private void Start() {
            AddEnablePhases(GamePhase.Cooking);
            var cookingStatusModel = GetComponent<PopcornCookingStatusModel>();
            var placingStatusModel = GetComponent<PopcornPlacingStatusModel>();
            var blazeBeansColliderView = GetComponent<BlazeBeanBrustView>();
            
            // ポップコーンが破裂したとき、熱を放つ
            cookingStatusModel.OnPoppedWithoutInitObservable
                .AsUnitObservable()
                .Subscribe(_ => blazeBeansColliderView.Brust(25000f))
                .AddTo(this);
        }
        
    }
}
