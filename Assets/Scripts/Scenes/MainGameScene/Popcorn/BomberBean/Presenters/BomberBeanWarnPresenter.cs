using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene.BomberBean {
    public class BomberBeanWarnPresenter : PhaseSwitchablePresenterMonobehavior {
        
        private void Start() {
            AddEnablePhases(GamePhase.Cooking);
            var cookingStatusModel = GetComponent<Popcorn.PopcornCookingStatusModel>();
            var bomberBeanAnimatorView = GetComponent<BomberBeanAnimatorView>();
            
            // 状態が変更された時、その状態に合わせてスプライトを変更する
            cookingStatusModel.OnHeatChangedObservable
                .Where(_ => isEnablePhase)
                .Select(_ => cookingStatusModel.GetHeatPercentToPop())
                .Subscribe(x => bomberBeanAnimatorView.ApplyWarnAnimation(x))
                .AddTo(this);
        }
        
    }
}
