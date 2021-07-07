using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.WaitingPlayerLabel {
    public class WaitingPlayerLabelView : MonoBehaviour {
        
        [SerializeField]
        private Animator _animator = null;

        [SerializeField]
        private TMPro.TextMeshProUGUI selfPlayerText = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI otherPlayerText = null;
        [SerializeField]
        private AudioClip joinPlayerSound = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI roomIdText = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI modeText = null;


        private int startStateHash = Animator.StringToHash("Base Layer.Start");
        private int endStateHash = Animator.StringToHash("Base Layer.End");
        private IObservable<Unit> onFinishStartAnimationObservable = null;
        private ObservableStateMachineTrigger animationTrigger = null;

        private void Awake() {
            _animator = GetComponent<Animator>();
            animationTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();

            onFinishStartAnimationObservable = animationTrigger
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == startStateHash)
                .AsUnitObservable()
                .First()
                .PublishLast()
                .RefCount();

            animationTrigger
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == endStateHash)
                .AsUnitObservable()
                .First()
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);
        }


        private void PlayJoinPlayerAnimation(string otherPlayerNickname) {
            otherPlayerText.text = otherPlayerNickname;
            _animator.SetBool("OtherPlayerJoined", true);
            AudioManager.Instance.PlayOneShot(joinPlayerSound);
        }

        private void PlayShowAnimation(string selfPlayerNickname) {
            selfPlayerText.text = selfPlayerNickname;
            _animator.SetTrigger("Show");
            roomIdText.text = PhotonManager.GetRoomId();
            modeText.text = PhotonManager.IsCurrentRoomVisible() ? "ランダムルーム" : "プライベートル－ム";
        }



        public IObservable<Unit> CreateShowAnimationObservable(string selfPlayerNickname) {
            return animationTrigger
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == startStateHash)
                .AsUnitObservable()
                .DoOnSubscribe(() => PlayShowAnimation(selfPlayerNickname))
                .First()
                .Publish()
                .RefCount();
        }


        public IObservable<Unit> CreateJoinAnimationObservable(string otherPlayerNickname) {
            return animationTrigger
                .OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == endStateHash)
                .AsUnitObservable()
                .DoOnSubscribe(() => PlayJoinPlayerAnimation(otherPlayerNickname))
                .First()
                .Publish()
                .RefCount();
        }
        
    }
}
