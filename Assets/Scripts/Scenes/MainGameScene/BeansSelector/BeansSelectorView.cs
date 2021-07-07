using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.BeansSelector {
    public class BeansSelectorView : MonoBehaviour {

        private static readonly int showStateHash = Animator.StringToHash("Base Layer.Show");
        private static readonly int hideStateHash = Animator.StringToHash("Base Layer.Hide");

        [SerializeField] private Animator _animator = null;
        [SerializeField] private List<Button> itemButtons = null;
        [SerializeField] private List<Image> itemImages = null;
        [SerializeField] private List<Sprite> spriteList = null;
        private ObservableStateMachineTrigger animationTrigger = null;
        public IObservable<int> OnPushButtonObservable { get; private set; } = null;
        public bool IsShowing { get; private set; } = false;


        private void Awake() {
            animationTrigger = _animator.GetBehaviour<ObservableStateMachineTrigger>();

            animationTrigger.OnStateEnterAsObservable()
                .Where(state => state.StateInfo.fullPathHash == showStateHash)
                .Subscribe(_ => IsShowing = true)
                .AddTo(this);

            animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == hideStateHash)
                .Subscribe(_ => IsShowing = false)
                .AddTo(this);
        }

        private void Start() {
            var buttonsSubject = new Subject<Tuple<Button, int>>();
            OnPushButtonObservable = buttonsSubject.SelectMany(
                x => x.Item1.OnClickAsObservable(),
                (x, _) => x.Item2
            )
            .Publish()
            .RefCount();

            OnPushButtonObservable.Publish().Connect();

            for (int i = 0; i < itemButtons.Count; i++) {
                buttonsSubject.OnNext(new Tuple<Button, int>(itemButtons[i], i));
            }
        }


        private void Show() {
            _animator.SetBool("isShow", true);
        }

        private void Hide() {
            _animator.SetBool("isShow", false);
        }

        private void SetItemSprites(IEnumerable<SelectorItem> items) {
            var itemsList = items.ToList();
            for (int i = 0; i < itemButtons.Count; i++) {
                if (itemImages.Count <= i) continue;
                var image = itemImages[i];
                var itemIndex = itemsList[i];
                
                if (spriteList.Count <= (int)itemIndex) {
                    image.sprite = null;
                    continue;
                }
                image.sprite = spriteList[(int)itemIndex];
            }
        }


        public IObservable<Unit> CreateShowObservable(IEnumerable<SelectorItem> items) {
            var itemsList = items.ToList();
            itemsList.Add(SelectorItem.None);

            return animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == showStateHash)
                .DoOnSubscribe(() => {
                    SetItemSprites(itemsList);
                    Show();
                })
                .AsUnitObservable()
                .First()
                .Publish()
                .RefCount();
        }


        public IObservable<Unit> CreateHideObservable() {
            return animationTrigger.OnStateExitAsObservable()
                .Where(state => state.StateInfo.fullPathHash == hideStateHash)
                .DoOnSubscribe(Hide)
                .AsUnitObservable()
                .First()
                .Publish()
                .RefCount();
        }


        public void SetEnable(bool isEnable) {
            foreach (var button in itemButtons) {
                button.interactable = isEnable;
            }
        }

    }
}
