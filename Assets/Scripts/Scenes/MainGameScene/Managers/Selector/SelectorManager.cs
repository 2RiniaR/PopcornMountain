using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {

    /// <summary>
    ///   選択できるアイテムの列挙
    /// </summary>
    public enum SelectorItem {
        None,
        HardBean,
        BomberBean,
        GoldenBean,
        BlazeBean,
        SoggyBean,
        Lid
    }


    public class SelectorManager : BaseMainGameManager {

        #region 定数

        /// <summary>
        ///   一度に選択肢として生成するアイテムの数
        /// </summary>
        private const int itemEmittionCount = 2;

        /// <summary>
        ///   「いくつのポップコーンを取得したらセレクターを表示するか」の初期値
        /// </summary>
        private const int defaultTriggerCount = 3;

        #endregion


        #region コンポーネント参照

        private BeansManager beansManager = GetManager<BeansManager>();
        private LidManager lidManager = GetManager<LidManager>();

        #endregion


        #region 非公開のプロパティ

        /// <summary>
        ///   Subscribeしたパイプラインを一括で破棄するためのオブジェクト
        /// </summary>
        private CompositeDisposable onDispose = new CompositeDisposable();

        /// <summary>
        ///   セレクターの選択肢が生成されたときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<IEnumerable<SelectorItem>> selectorSubject = new Subject<IEnumerable<SelectorItem>>();

        /// <summary>
        ///   各アイテムの生成確率を格納する配列
        /// </summary>
        private float[] selectorItemProbablities = null;

        /// <summary>
        ///   セレクター生成までの残りポップコーン取得数
        /// </summary>
        private IntReactiveProperty selectorEmitLeftCount = new IntReactiveProperty(defaultTriggerCount);

        private Subject<SelectorItem> onItemEmitSubject = new Subject<SelectorItem>();

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   セレクターの選択肢が生成されたときにOnNext()が発行されるObservable
        /// </summary>
        public IObservable<IEnumerable<SelectorItem>> OnSelectorShowObservable { get { return selectorSubject; } }

        public IObservable<SelectorItem> OnItemEmitObservable { get { return onItemEmitSubject; } }

        #endregion


        public bool IsSelectorShowing { get; private set; } = false;



        #region 初期化用の関数

        public SelectorManager() : base() {
            InitProbablities();
            InitShowSelectorObservable();
        }


        /// <summary>
        ///   各アイテムの生成確立を初期化する
        /// </summary>
        private void InitProbablities() {
            selectorItemProbablities = new float[Enum.GetValues(typeof(SelectorItem)).Length];
            for (int i = 0; i < selectorItemProbablities.Length; i++) {
                selectorItemProbablities[i] = 1f;
            }
            selectorItemProbablities[(int)SelectorItem.None] = 0f;
        }


        /// <summary>
        ///   一定回数以上ポップコーンを取得したらセレクターを生成するObservableを初期化する
        /// </summary>
        private void InitShowSelectorObservable() {
            // ポップコーンが取得されたとき、セレクター生成までのカウントを1減らす
            beansManager.OnCatchedBeans
                .Where(_ => !IsSelectorShowing)
                .Subscribe(_ => {
                    selectorEmitLeftCount.Value--;
                    if (selectorEmitLeftCount.Value <= 0) {
                        selectorEmitLeftCount.Value = defaultTriggerCount;
                        EmitSelector();
                    }
                })
                .AddTo(onDispose);
        }

        #endregion


        #region 終了処理用の関数

        public override void Dispose() {
            onDispose.Dispose();
        }

        #endregion


        #region 非公開の関数

        /// <summary>
        ///   確率分布に従った抽選結果を指定個数分だけ返す
        /// </summary>
        /// <param name="probabilities">確率分布の配列</param>
        /// <param name="selectNum">抽選する個数</param>
        private IEnumerable<SelectorItem> SelectItems(IEnumerable<float> probabilities, int selectNum) {
            const int defaultReturnValue = 0;

            List<int> itemList = Enumerable.Range(0, probabilities.Count()).ToList();
            List<float> probabilitiesList = probabilities.ToList();
            float totalValue = probabilities.Sum();

            SelectorItem[] selectedElements = new SelectorItem[selectNum];

            for (int i = 0; i < selectNum; i++) {
                int result = 0;
                float lockValue = 0f;
                float randomValue = UnityEngine.Random.Range(0f, totalValue);
                selectedElements[i] = (SelectorItem)Enum.ToObject(typeof(SelectorItem), defaultReturnValue);

                for (result = 0; result < itemList.Count; result++) {
                    lockValue += probabilitiesList[result];
                    if (randomValue < lockValue) {
                        selectedElements[i] = (SelectorItem)Enum.ToObject(typeof(SelectorItem), itemList[result]);
                        break;
                    };
                }

                if (result == probabilitiesList.Count) continue;

                totalValue -= probabilitiesList[result];
                itemList.RemoveAt(result);
                probabilitiesList.RemoveAt(result);
            }

            return selectedElements;
        }


        private void ResetSelector() {
            selectorEmitLeftCount.Value = defaultTriggerCount;
            IsSelectorShowing = false;
        }


        /// <summary>
        ///   生成したアイテムから発生確率を調整する
        /// </summary>
        /// <param name="type">生成したアイテムの種類</param>
        private void UpdateProbablitiesWithEmit(SelectorItem type) {
            for (int i = 0; i < selectorItemProbablities.Length; i++) {
                selectorItemProbablities[i] *= ((int)type == i) ? 0.5f : 2f;
            }
        }


        /// <summary>
        ///   セレクターの選択肢を生成する
        /// </summary>
        /// <param name="types">選択肢となるアイテムの種類</param>
        private void EmitSelector(IEnumerable<SelectorItem> types) {
            if (IsSelectorShowing) return;
            IsSelectorShowing = true;

            var itemTypesList = types.ToList();
            itemTypesList.Add(SelectorItem.None);
            selectorSubject.OnNext(itemTypesList);

            foreach (var type in types) {
                UpdateProbablitiesWithEmit(type);
            }
        }

        #endregion


        #region 公開する関数

        public void EmitSelector() {
            EmitSelector(SelectItems(selectorItemProbablities, itemEmittionCount));
        }


        /// <summary>
        ///   アイテムを生成する
        /// </summary>
        /// <param name="item">生成するアイテムの種類</param>
        public void EmitItem(SelectorItem item) {
            switch(item) {
                case SelectorItem.HardBean:
                    beansManager.EmitBean(PopcornType.Hard);
                    break;
                case SelectorItem.BomberBean:
                    beansManager.EmitBean(PopcornType.Bomber);
                    break;
                case SelectorItem.GoldenBean:
                    beansManager.EmitBean(PopcornType.Golden);
                    break;
                case SelectorItem.BlazeBean:
                    beansManager.EmitBean(PopcornType.Blaze);
                    break;
                case SelectorItem.SoggyBean:
                    beansManager.EmitBean(PopcornType.Soggy);
                    break;
                case SelectorItem.Lid:
                    lidManager.GenerateLid();
                    break;
            }
            ResetSelector();
            onItemEmitSubject.OnNext(item);
        }

        #endregion

    }
}
