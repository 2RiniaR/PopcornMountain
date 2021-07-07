using System;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class BeansCounter : BaseMainGameManager {

        #region 定数

        private const int maxPopcornEmissionCount = 40;

        #endregion


        #region 非公開のプロパティ

        private ReactiveProperty<int> popcornEmissionCount = new ReactiveProperty<int>(0);

        #endregion


        #region 公開するプロパティ

        public IObservable<bool> IsOverEmissionChangedObservable { get; private set; } = null;

        #endregion


        public BeansCounter() {
            IsOverEmissionChangedObservable = popcornEmissionCount.Pairwise()
                .Where(x => (x.Previous >= maxPopcornEmissionCount) ^ (x.Current >= maxPopcornEmissionCount))
                .Select(x => x.Current >= maxPopcornEmissionCount)
                .Publish()
                .RefCount();
        }


        public void AddCount(int count) {
            popcornEmissionCount.Value += count;
        }

    }
}
