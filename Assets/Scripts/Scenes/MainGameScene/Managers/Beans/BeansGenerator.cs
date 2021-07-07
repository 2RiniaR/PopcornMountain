using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using PopcornMountain.MainGameScene.Popcorn;


namespace PopcornMountain.MainGameScene {
    public class BeansGenerator : BaseMainGameManager {

        private struct PopcornGenerationProperty {
            public PopcornParameters parameters;
            public float initialHeat;
            public Vector2 initialVelocity;
        }


        #region 非公開のプロパティ

        /// <summary>
        ///   豆のGameObjectがインスタンス化されたときにOnNext()が発行されるSubject
        /// </summary>
        private Subject<GameObject> onInstantiateBeansSubject = new Subject<GameObject>();

        /// <summary>
        ///   各豆のPrefabを格納する
        /// </summary>
        private GameObject[] popcornPrefabs;

        /// <summary>
        ///   豆を生成する親オブジェクトのタグ
        /// </summary>
        private const string beansParentTagName = "BeansParent";

        /// <summary>
        ///   Resourcesフォルダを起点とした、各豆のPrefabへのパス
        /// </summary>
        private static readonly string[] popcornPrefabsPath = new string[] {
            "Prefabs/Popcorns/NormalBean",
            "Prefabs/Popcorns/HardBean",
            "Prefabs/Popcorns/BomberBean",
            "Prefabs/Popcorns/GoldenBean",
            "Prefabs/Popcorns/DangerBean",
            "Prefabs/Popcorns/BlazeBean",
            "Prefabs/Popcorns/SoggyBean",
            "Prefabs/Popcorns/TrollBean",
        };

        private Queue<PopcornGenerationProperty> popcornGenerationBuffer = new Queue<PopcornGenerationProperty>();
        private CompositeDisposable onDispose = new CompositeDisposable();
        private BeansGeneratePositionManager positionManager = null;

        #endregion


        #region 公開するObservable

        /// <summary>
        ///   豆のGameObjectがインスタンス化されたときにOnNext()が発行されるObservable
        /// </summary>
        /// <value></value>
        public IObservable<GameObject> OnInstantiateBeansObservable { get { return onInstantiateBeansSubject; } }

        #endregion


        #region 初期化用の関数

        public BeansGenerator() : base() {
            LoadPopcornPrefab();
        }


        public override void AfterLoadScene() {
            positionManager = new BeansGeneratePositionManager();
            InitGeneratePopcornObservable();
        }


        /// <summary>
        ///   各豆のPrefabを読み込む
        /// </summary>
        private void LoadPopcornPrefab() {
            popcornPrefabs = new GameObject[popcornPrefabsPath.Length];
            for (int i = 0; i < popcornPrefabsPath.Length; i++) {
                popcornPrefabs[i] = (GameObject)Resources.Load(popcornPrefabsPath[i]);
            }
        }


        private void InitGeneratePopcornObservable() {
            Observable.EveryUpdate()
                .Subscribe(_ => {
                    while (popcornGenerationBuffer.Count > 0) {
                        var position = positionManager.GetGeneratePosition();
                        if (!position.HasValue) break;
                        GenerateBean(popcornGenerationBuffer.Dequeue(), position.Value);
                    }
                })
                .AddTo(onDispose);

            Observable.EveryFixedUpdate()
                .Subscribe(_ => {
                    positionManager.RefleshGenerationOrder();
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

        /// <sumamry>
        ///   ポップコーンの種類とシード値からパラメータを生成する
        /// </summary>
        /// <param name="type">生成するポップコーンの種類</param>
        /// <param name="seed">生成するポップコーンのシード値</param>
        private PopcornParameters GenerateParametersFromSeed(PopcornType type, float seed) {
            var param = popcornPrefabs[(int)type].GetComponent<PopcornParameterModel>().parameters;
            param.popHeat *= seed;
            param.blackHeat *= seed;
            param.explosionPower *= seed;
            param.point *= seed;
            param.steam *= seed;
            param.diffence *= seed;
            param.scale = seed;
            return param;
        }


        /// <sumamry>
        ///   ポップコーンのインスタンスに指定したパラメータと熱量を適用する
        /// </summary>
        /// <param name="beansInstance">適用先のポップコーンのインスタンス(破壊的変更がされます)</param>
        /// <param name="parameters">適用するパラメータ</param>
        /// <param name="initialHeat">適用する熱量</param>
        private void SetParameterToBeansInstance(
            GameObject beansInstance,
            PopcornGenerationProperty property
        ) {
            // パラメータとスケールを設定する
            beansInstance.transform.localScale = new Vector3(property.parameters.scale * 1.7f, property.parameters.scale * 1.7f, 1f);
            beansInstance.GetComponent<PopcornParameterModel>().Init(property.parameters);

            // 熱量の初期化
            beansInstance.GetComponent<PopcornCookingStatusModel>().Init(property.initialHeat);

            // 速度の初期化
            var rigidbody = beansInstance.GetComponent<Rigidbody2D>();
            rigidbody.velocity = property.initialVelocity;
        }


        /// <sumamry>
        ///   ポップコーンのGameObjectを生成する
        /// </summary>
        /// <param name="param">生成するポップコーンの初期パラメータ</param>
        /// <param name="worldPosition">生成するワールド座標</param>
        /// <param name="rotation">生成する回転</param>
        /// <param name="initialHeat">初期化時の熱量</param>
        private void GenerateBean(PopcornGenerationProperty property, Vector2 worldPosition) {
            var beansParent = GameObject.FindWithTag(beansParentTagName);
            var newInstance = UnityEngine.Object.Instantiate(
                popcornPrefabs[(int)property.parameters.type],
                (Vector3)worldPosition + new Vector3(0f, 0f, -3f),
                Quaternion.identity,
                beansParent.transform
            );
            SetParameterToBeansInstance(newInstance, property);
            onInstantiateBeansSubject.OnNext(newInstance);
        }

        #endregion


        #region 公開する関数

        /// <sumamry>
        ///   ポップコーンのGameObjectを生成する
        /// </summary>
        /// <param name="type">生成するポップコーンの種類</param>
        /// <param name="seed">生成するポップコーンのシード値</param>
        /// <param name="rotation">生成する回転</param>
        /// <param name="initialHeat">初期化時の熱量</param>
        public void AddGenerationBuffer(PopcornType type, float seed, float initialHeat = 0f, Vector2? initialVelocity = null) {
            AddGenerationBuffer(GenerateParametersFromSeed(type, seed), initialHeat, initialVelocity);
        }


        public void AddGenerationBuffer(PopcornParameters parameters, float initialHeat = 0f, Vector2? initialVelocity = null) {
            popcornGenerationBuffer.Enqueue(new PopcornGenerationProperty {
                parameters = parameters,
                initialHeat = initialHeat,
                initialVelocity = initialVelocity ?? new Vector2(0f, 0f)
            });
        }

        #endregion

    }
}
