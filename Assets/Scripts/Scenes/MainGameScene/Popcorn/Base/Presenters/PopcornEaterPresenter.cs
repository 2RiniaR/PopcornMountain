using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene.Popcorn {
    public enum EatingEffectType {
        GettingGoodBeans,
        GettingBadBeans,
        GettingBurnedBeans
    }
    
    public class PopcornEaterPresenter : PhaseSwitchablePresenterMonobehavior {

        private PopcornParameterModel parameterModel;
        private PopcornCookingStatusModel cookingStatusModel;

        private void Start() {
            AddEnablePhases(GamePhase.Cooking);
            parameterModel = GetComponent<PopcornParameterModel>();
            cookingStatusModel = GetComponent<PopcornCookingStatusModel>();
            var catchingStatusModel = GetComponent<PopcornCatchingStatusModel>();
            var clickView = GetComponent<PopcornClickView>();
            var colliderView = GetComponent<PopcornColliderView>();
            var animatorView = GetComponent<PopcornAnimatorView>();
            var textEffectView = GetComponent<PopcornTextEffectView>();
            var particleView = GetComponent<PopcornParticleView>();
            var audioView = GetComponent<PopcornAudioView>();
            var faintManager = GetManager<FaintManager>();
            var playerManager = GetManager<PlayerManager>();


            // キャッチ可能状態のときにクリックされたら、キャッチされた状態にする
            clickView
                .OnClickObservable
                .Where(_ => isEnablePhase)
                .Where(_ => catchingStatusModel.IsCatchable)
                .Where(_ => !faintManager.IsFainting)
                .Subscribe(_ => catchingStatusModel.Catch())
                .AddTo(this);

            // 落下エリアに触れたら、
            //   キャッチ可能状態ならば、落下した状態にする
            //   キャッチ不可能状態ならば、ゲームオブジェクトを削除する
            colliderView
                .OnHitToFallenArea
                .Where(_ => isEnablePhase)
                .Take(1)
                .Subscribe(_ => {
                    catchingStatusModel.Fall();
                    animatorView.Destroy();
                })
                .AddTo(this);

            colliderView
                .OnHitToFlownArea
                .Where(_ => isEnablePhase)
                .Take(1)
                .Subscribe(_ => {
                    catchingStatusModel.Fly();
                    animatorView.Destroy();
                })
                .AddTo(this);

            // キャッチされたら、現在のプレイヤーの得点を加算する
            catchingStatusModel
                .OnCatchedObservable
                .Subscribe(_ => {
                    playerManager.AddSelfPlayerScore(parameterModel.parameters.point);
                    var type = GetEatingEffectType();
                    textEffectView.PlayTextEffectOnGameObject(type);
                    audioView.PlayEatSound(type);
                    particleView.PlayEatenParticle();
                    animatorView.PlayEatenAnimation();
                })
                .AddTo(this);
        }
        
        // 豆が食べられたときにどのエフェクトを再生するかを返す
        private EatingEffectType GetEatingEffectType() {
            if (cookingStatusModel.CurrentCookingState == PopcornCookingStates.Burned) {
                return EatingEffectType.GettingBurnedBeans;
            } else if (parameterModel.parameters.point < 0) {
                return EatingEffectType.GettingBadBeans;
            } else {
                return EatingEffectType.GettingGoodBeans;
            }
        }
        
    }
}
