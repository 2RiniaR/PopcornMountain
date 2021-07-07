using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using PopcornMountain.MainGameScene.Popcorn;


namespace PopcornMountain.MainGameScene.DangerBean {
    public class DangerBeanFaintPresenter : PhaseSwitchablePresenterMonobehavior {

        private void Start() {
            var cookingStatusModel = GetComponent<PopcornCookingStatusModel>();
            var animatorView = GetComponent<PopcornAnimatorView>();
            var dangerBeanParticleView = GetComponent<DangerBeanParticleView>();
            var playerManager = GetManager<PlayerManager>();
            var faintManager = GetManager<FaintManager>();

            cookingStatusModel.OnPoppedWithoutInitObservable
                .Subscribe(_ => {
                    playerManager.AddSelfPlayerScore(-15f);
                    faintManager.Faint(5f);
                    dangerBeanParticleView.PlayBlastParticleOnParent();
                    animatorView.Destroy();
                })
                .AddTo(this);
        }
        
    }
}
