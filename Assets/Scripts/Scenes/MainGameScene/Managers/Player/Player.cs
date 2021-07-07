using System;
using UnityEngine;
using UniRx;

namespace PopcornMountain.MainGameScene {
    public class Player {

        #region 非公開のプロパティ

        /// <summary>
        ///   パラメータ
        /// </summary>
        public PlayerParameters parameters { get; private set; }

        /// <summary>
        ///   現在の満腹度のReactiveProperty
        /// </summary>
        private FloatReactiveProperty currentHungerPoint = new FloatReactiveProperty(0f);

        #endregion


        #region 公開するプロパティ

        /// <summary>
        ///   現在の空腹度
        /// </summary>
        public float CurrentHungerPoint { get { return currentHungerPoint.Value; } }

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   空腹度が変化したらOnNext()が発行されるObservable
        /// </summary>
        public IObservable<float> OnChangeHungerPoint { get { return currentHungerPoint; } }

        /// <summary>
        ///   空腹度が最大になったらOnNext()とOnCompleted()が発行されるObservable
        /// </summary>
        public IObservable<Unit> OnFullHungerPoint = null;

        #endregion


        #region 初期化用の関数

        public Player(PlayerParameters param) {
            parameters = param;

            // OnFullHungerPointを初期化する
            OnFullHungerPoint = currentHungerPoint
                .SkipWhile(p => p < parameters.maxHungerPoint)
                .First()
                .AsUnitObservable()
                .Publish()
                .RefCount();
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   満腹度を加算する
        /// </summary>
        public void AddHungerPoint(float point) {
            currentHungerPoint.Value = Mathf.Max(currentHungerPoint.Value + point, 0f);
        }


        /// <summary>
        ///   満腹度を設定する
        /// </summary>
        public void SetHungerPoint(float point) {
            currentHungerPoint.Value = Mathf.Max(point, 0f);
        }


        /// <summary>
        ///   ニックネームを変更する
        /// </summary>
        /// <param name="nickname">ニックネーム</param>
        public void SetNickname(string nickname) {
            parameters = new PlayerParameters(parameters.playerID, nickname, parameters.maxHungerPoint);
        }

        #endregion

    }
}
