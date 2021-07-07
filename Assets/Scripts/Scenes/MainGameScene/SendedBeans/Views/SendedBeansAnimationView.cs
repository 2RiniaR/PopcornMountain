using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.SendedBeans {
    public class SendedBeansAnimationView : BaseMainGameComponent {

        [SerializeField]
        private GameObject sendedBeansPrefab = null;
        private static readonly Vector3 defaultPosition = new Vector3(-538f, 671f, 0);
        private static readonly Quaternion defaultRotation = Quaternion.identity;
        private static readonly int sendStateHash = Animator.StringToHash("Base Layer.Send");
        private static readonly int receiveStateHash = Animator.StringToHash("Base Layer.Receive");

        [SerializeField]
        private List<Sprite> unpoppedBeansSprites = null;
        [SerializeField]
        private List<Sprite> poppedBeansSprites = null;


        private Sprite GetSpriteFromParameter(PopcornParameters parameters, float heat) {
            if (heat >= parameters.popHeat) {
                return poppedBeansSprites[(int)parameters.type];
            } else {
                return unpoppedBeansSprites[(int)parameters.type];
            }
        }


        public IObservable<Unit> CreatePlaySendAnimationObservable(PopcornParameters parameters, float heat) {
            var inst = Instantiate(sendedBeansPrefab, defaultPosition, defaultRotation, transform);
            inst.GetComponent<Image>().sprite = GetSpriteFromParameter(parameters, heat);
            var instanceAnimator = inst.GetComponent<Animator>();
            var animationTrigger = instanceAnimator.GetBehaviour<ObservableStateMachineTrigger>();

            return animationTrigger.OnStateExitAsObservable()
                .DoOnSubscribe(() => instanceAnimator.SetTrigger("Send"))
                .Where(state => state.StateInfo.fullPathHash == sendStateHash)
                .First()
                .DoOnCompleted(() => Destroy(inst))
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }


        public IObservable<Unit> CreatePlayReceiveAnimationObservable(PopcornParameters parameters, float heat) {
            var inst = Instantiate(sendedBeansPrefab, defaultPosition, defaultRotation, transform);
            inst.GetComponent<Image>().sprite = GetSpriteFromParameter(parameters, heat);
            var instanceAnimator = inst.GetComponent<Animator>();
            var animationTrigger = instanceAnimator.GetBehaviour<ObservableStateMachineTrigger>();

            return animationTrigger.OnStateExitAsObservable()
                .DoOnSubscribe(() => instanceAnimator.SetTrigger("Receive"))
                .Where(state => state.StateInfo.fullPathHash == receiveStateHash)
                .First()
                .DoOnCompleted(() => Destroy(inst))
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }

    }
}
