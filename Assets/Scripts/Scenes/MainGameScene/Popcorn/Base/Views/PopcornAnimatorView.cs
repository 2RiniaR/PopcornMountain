using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornAnimatorView : BaseMainGameComponent {
        
        private Animator _animator;
        private int eatenStateHash = Animator.StringToHash("DestroyLayer.Eaten");
        private int disappearStateHash = Animator.StringToHash("DestroyLayer.Disappear");
        
        
        private void Start() {
            _animator = GetComponent<Animator>();
            var animationTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            
            // 消滅アニメーションの終了後、自身のゲームオブジェクトを破棄する
            animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == eatenStateHash)
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);
            
            // 消滅アニメーションの終了後、自身のゲームオブジェクトを破棄する
            animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == disappearStateHash)
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);
        }
        
        /// <summary>
        ///   <para>状態によってスプライトを変更する</para>
        /// </summary>
        public void ChangeSpriteWithState(PopcornCookingStates state) {
            if (_animator == null) return;
            switch(state) {
                case PopcornCookingStates.Popped:
                    _animator.SetTrigger("Pop");
                    break;
                case PopcornCookingStates.Burned:
                    AnimateScaleOnBurn();
                    _animator.SetTrigger("Burn");
                    break;
            }
        }
        
        public void SetInitSpriteState(PopcornCookingStates state) {
            _animator.SetInteger("InitState", (int)state);
        }
        
        
        /// <summary>
        ///   <para>焦げたときにサイズを縮小するアニメーションを疑似的に再現する</para>
        /// </summary>
        private void AnimateScaleOnBurn() {
            Observable.Concat(
                Observable.Return(1.0f).First(),
                this.UpdateAsObservable().Select(_ => -(0.5f / 20))
            )
            .Scan((acc, current) => acc + current)
            .TakeWhile(x => x > 0.5f)
            .Subscribe(x => transform.localScale = new Vector3(x, x, 1))
            .AddTo(this);
        }
        
        public void Destroy() {
            Debug.Log("destroy");
            Destroy(gameObject);
        }
        
        public void PlayEatenAnimation() {
            Destroy(gameObject);
        }
        
        public void PlayDisappearAnimation() {
            _animator.SetTrigger("Disappear");
        }
        
    }
}
