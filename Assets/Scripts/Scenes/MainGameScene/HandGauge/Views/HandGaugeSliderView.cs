using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene {
    public class HandGaugeSliderView : MonoBehaviour {

        private static readonly int showStateHash = Animator.StringToHash("Base Layer.Show");
        private static readonly int hideStateHash = Animator.StringToHash("Base Layer.Hide");
        private static readonly Vector2 positionOffset = new Vector2(0, 50f);

        [SerializeField]
        private Image fill = null;
        [SerializeField]
        private Animator animator = null;
        private RectTransform parentCanvasRectTransform = null;
        private RectTransform rectTransform = null;
        private ObservableStateMachineTrigger animationTrigger = null;


        private void Awake() {
            animationTrigger = animator.GetBehaviour<ObservableStateMachineTrigger>();
            parentCanvasRectTransform = transform.parent.GetComponent<RectTransform>();
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update() {
            Vector2 viewPoint = Camera.main.ScreenToViewportPoint(Vector3.Scale(Input.mousePosition, new Vector3(1, 1, 0)));
            Vector2 localpoint = new Vector2(
                ((viewPoint .x * parentCanvasRectTransform.sizeDelta.x) - (parentCanvasRectTransform.sizeDelta.x * 0.5f)),
                ((viewPoint .y * parentCanvasRectTransform.sizeDelta.y) - (parentCanvasRectTransform.sizeDelta.y * 0.5f))
            );
            rectTransform.anchoredPosition = localpoint + positionOffset;
        }


        private void PlayHideAnimation() {
            animator.SetBool("isShow", false);
        }

        private void PlayShowAnimation() {
            animator.SetBool("isShow", true);
        }


        public void SetValue(float value, float maxValue) {
            fill.fillAmount = value / maxValue;
        }

        public IObservable<Unit> CreateShowAnimationObsrvable() {
            return animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == showStateHash)
                .AsUnitObservable()
                .DoOnSubscribe(PlayShowAnimation)
                .Publish()
                .RefCount();
        }

        public IObservable<Unit> CreateHideAnimationObsrvable() {
            return animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == hideStateHash)
                .AsUnitObservable()
                .DoOnSubscribe(PlayHideAnimation)
                .Publish()
                .RefCount();
        }

        public void PlayEmptyAnimation() {
            animator.SetTrigger("Empty");
        }

    }
}
