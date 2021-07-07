using UniRx;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornParticlePresenter : PhaseSwitchablePresenterMonobehavior {
        
        private void Start() {
            var particleView = GetComponent<PopcornParticleView>();
            var bubbleParticleView = GetComponent<PopcornBubbleParticleView>();
            var shakeView = GetComponent<PopcornShakeView>();
            var cookingStatusModel = GetComponent<PopcornCookingStatusModel>();
            var parameterModel = GetComponent<PopcornParameterModel>();
            var audioView = GetComponent<PopcornAudioView>();

            // ポップコーンが破裂前の状態の時、
            //   破裂に必要な熱量が50%未満の場合は湯気のパーティクルを再生せず
            //   50%以上の場合は、熱量に応じて 毎秒3.0～7.0個 の間で湯気のパーティクルを再生する
            cookingStatusModel.OnHeatChangedObservable
                .TakeUntil(cookingStatusModel.OnPoppedObservable)
                .Select(_ => cookingStatusModel.GetHeatPercentToPop())
                .Subscribe(
                    per => {
                        particleView.SetSteamParticle(parameterModel.parameters.steam * per * 2);
                        bubbleParticleView.SetBubbleParticle(per);
                        shakeView.SetPower(per);
                        audioView.SetFringSound(per);
                    },
                    () => {
                        particleView.StopSteamParticle();
                        bubbleParticleView.StopBubbleParticle();
                        shakeView.Stop();
                        audioView.StopFringSound();
                    }
                )
                .AddTo(this);

            // 破裂したとき、弾けるパーティクルを再生する
            cookingStatusModel.OnPoppedWithoutInitObservable
                .Subscribe(_ => {
                    particleView.PlayPopParticle();
                })
                .AddTo(this);

            // 焦げたときに黒煙のパーティクルを再生する
            cookingStatusModel.OnBurnedWithoutInitObservable
                .Subscribe(_ => {
                    particleView.PlayImpactSmokeParticle();
                })
                .AddTo(this);
        }
        
    }
}
