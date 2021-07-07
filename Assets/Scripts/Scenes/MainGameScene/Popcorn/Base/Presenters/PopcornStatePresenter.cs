using UniRx;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornStatePresenter : PhaseSwitchablePresenterMonobehavior {
        
        private void Start() {
            AddEnablePhases(GamePhase.Cooking);
            var cookingStatusModel = GetComponent<PopcornCookingStatusModel>();
            var animatorView = GetComponent<PopcornAnimatorView>();
            var rigidbodyView = GetComponent<PopcornRigidbodyView>();
            var colliderView = GetComponent<PopcornColliderView>();
            var audioView = GetComponent<PopcornAudioView>();
            var parameterModel = GetComponent<PopcornParameterModel>();
            var popView = GetComponent<PopcornPopView>();

            rigidbodyView.SetMass(parameterModel.parameters.unpoppedMass);

            cookingStatusModel.InitCookingStateObservable
                .Where(_ => isEnablePhase)
                .Subscribe(state => animatorView.SetInitSpriteState(state))
                .AddTo(this);

            cookingStatusModel.OnBurnedObservable
                .Where(_ => isEnablePhase)
                .Subscribe(_ => animatorView.Destroy())
                .AddTo(this);

            cookingStatusModel.OnBurnedWithoutInitObservable
                .Where(_ => isEnablePhase)
                .Subscribe(_ => audioView.PlaySoundWithState(PopcornCookingStates.Burned))
                .AddTo(this);

            cookingStatusModel.OnPoppedObservable
                .Where(_ => isEnablePhase)
                .Subscribe(_ => {
                    rigidbodyView.SetMass(parameterModel.parameters.poppedMass);
                    popView.ChangeToPopped();
                })
                .AddTo(this);

            cookingStatusModel.OnPoppedWithoutInitObservable
                .Where(_ => isEnablePhase)
                .Subscribe(_ => {
                    animatorView.ChangeSpriteWithState(PopcornCookingStates.Popped);
                    audioView.PlaySoundWithState(PopcornCookingStates.Popped);
                })
                .AddTo(this);
            
        }
        
    }
}
