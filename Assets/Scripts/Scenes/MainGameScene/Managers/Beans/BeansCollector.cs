using System;
using System.Linq;
using UnityEngine;
using UniRx;
using PopcornMountain.MainGameScene.Popcorn;


namespace PopcornMountain.MainGameScene {
    public class BeansCollector : BaseMainGameManager {

        #region 非公開のプロパティ

        /// <summary>
        ///   豆を登録したときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<GameObject> onRegistBeansSubject = new Subject<GameObject>();

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   豆が破裂したときにOnNext()が発行されるSubject
        /// </summary>
        public IObservable<GameObject> OnPopped { get; private set; }

        /// <summary>
        ///   豆が焦げたときにOnNext()が発行されるSubject
        /// </summary>
        public IObservable<GameObject> OnBurned { get; private set; }

        /// <summary>
        ///   豆が取得されたときにOnNext()が発行されるSubject
        /// </summary>
        public IObservable<GameObject> OnCatched { get; private set; }

        /// <summary>
        ///   豆が落下エリアに触れたときにOnNext()が発行されるSubject
        /// </summary>
        public IObservable<GameObject> OnFallen { get; private set; }

        /// <summary>
        ///   豆が送信エリアに触れたときOnNext()が発行されるSubject
        /// </summary>
        public IObservable<GameObject> OnFlown { get; private set; }

        /// <summary>
        ///   豆が持ち上げられたときにOnNext()が発行されるSubject
        /// </summary>
        public IObservable<GameObject> OnHeldBeans { get; private set; }

        /// <summary>
        ///   豆が離されたときにOnNext()が発行されるSubject
        /// </summary>
        public IObservable<GameObject> OnReleaseBeans { get; private set; }

        #endregion


        #region 初期化用の関数

        public BeansCollector() : base() {
            OnPopped = onRegistBeansSubject
                .SelectMany(
                    x => x.GetComponent<PopcornCookingStatusModel>().OnPoppedWithoutInitObservable,
                    (x, _) => x
                )
                .Publish()
                .RefCount();

            OnBurned = onRegistBeansSubject
                .SelectMany(
                    x => x.GetComponent<PopcornCookingStatusModel>().OnBurnedWithoutInitObservable,
                    (x, _) => x
                )
                .Publish()
                .RefCount();

            OnCatched = onRegistBeansSubject
                .SelectMany(
                    x => x.GetComponent<PopcornCatchingStatusModel>().OnCatchedObservable,
                    (x, _) => x
                )
                .Publish()
                .RefCount();

            OnFlown = onRegistBeansSubject
                .SelectMany(
                    x => x.GetComponent<PopcornCatchingStatusModel>().OnFlownObservable,
                    (x, _) => x
                )
                .Publish()
                .RefCount();

            OnFallen = onRegistBeansSubject
                .SelectMany(
                    x => x.GetComponent<PopcornCatchingStatusModel>().OnFallenObservable,
                    (x, _) => x
                )
                .Publish()
                .RefCount();

            OnHeldBeans = onRegistBeansSubject
                .SelectMany(
                    x => x.GetComponent<PopcornPlacingStatusModel>().OnHeldObservable,
                    (x, _) => x
                )
                .Publish()
                .RefCount();

            OnReleaseBeans = onRegistBeansSubject
                .SelectMany(
                    x => x.GetComponent<PopcornPlacingStatusModel>().OnReleasedObservable,
                    (x, _) => x
                )
                .Publish()
                .RefCount();
        }

        #endregion


        #region 公開する関数

        public void RegistBeans(GameObject beans) {
            onRegistBeansSubject.OnNext(beans);
        }

        #endregion

    }
}
