using UniRx;
using UnityEngine;
using System.Linq;


namespace PopcornMountain.MainGameScene.BeansSelector {
    public class BeansSelectorPresenter : BaseMainGameComponent {

        private void Start() {
            var beansSelectorView = GetComponent<BeansSelectorView>();
            var selectorManager = GetManager<SelectorManager>();
            var faintManager = GetManager<FaintManager>();

            selectorManager.OnSelectorShowObservable
                .Subscribe(async items => {
                    await Observable.EveryUpdate().TakeWhile(_ => beansSelectorView.IsShowing).AsSingleUnitObservable();
                    await beansSelectorView.CreateShowObservable(items);
                    selectorManager.EmitItem(items.ToArray()[await beansSelectorView.OnPushButtonObservable.First()]);
                    await beansSelectorView.CreateHideObservable();
                })
                .AddTo(this);

            faintManager.OnChangeIsFaintingObservable
                .Subscribe(isFainting => beansSelectorView.SetEnable(!isFainting))
                .AddTo(this);
        }

    }
}
