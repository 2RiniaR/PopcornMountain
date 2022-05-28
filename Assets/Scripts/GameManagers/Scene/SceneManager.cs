using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;
using PopcornMountain.Curtain;


namespace PopcornMountain {


    /// <summary>
    /// ゲームシーンの列挙
    /// </summary>
    /// <remark>
    ///   名前はシーンオブジェクトのファイル名と同一である
    /// </remark>
    public enum GameScene {
        /// <summary>
        /// 開始画面のシーン
        /// </summary>
        StartScene,
        
        /// <summary>
        /// ロビー画面のシーン
        /// </summary>
        LobbyScene,
        
        /// <summary>
        /// ゲームのシーン
        /// </summary>
        MainGameScene,
        
        /// <summary>
        /// シーン切り替えのアニメーション中
        /// </summary>
        Changing
    }


    public static class SceneManager {

        #region 定数

        /// <summary>
        ///   各ゲームシーンとそのマネージャクラスの対応
        /// </summary>
        private static readonly Dictionary<GameScene, Type> sceneManagersType = new Dictionary<GameScene, Type>() {
            { GameScene.StartScene, typeof(StartScene.StartSceneManager) },
            { GameScene.LobbyScene, typeof(LobbyScene.LobbySceneManager) },
            { GameScene.MainGameScene, typeof(MainGameScene.MainGameSceneManager) },
        };

        #endregion

        /// <sumamry>
        ///   現在実行中のゲームシーンのマネージャ
        /// </summary>
        private static BaseSceneManager currentSceneManager = null;

        private static BaseSceneManager previousSceneManager = null;

        /// <sumamry>
        ///   現在のゲームシーン(Changing含む)のReactiveProperty
        /// </summary>
        private static ReactiveProperty<GameScene> currentGameScene = new ReactiveProperty<GameScene>(GameScene.StartScene);

        /// <sumamry>
        ///   現在のゲームシーン(Changing含む)
        /// </summary>
        public static GameScene CurrentGameScene {
            get {
                return currentGameScene.Value;
            }
            private set {
                currentGameScene.SetValueAndForceNotify(value);
            }
        }

        /// <sumamry>
        ///   シーンの読み込みが開始したときにOnNext()が発行されるSubject
        /// </summary>
        private static Subject<GameScene> onBeginChangeScene = new Subject<GameScene>();

        /// <sumamry>
        ///   シーンの読み込みが完了したときにOnNext()が発行されるSubject
        /// </summary>
        private static Subject<GameScene> onFinishChangeScene = new Subject<GameScene>();


        #region 初期化用の関数

        static SceneManager() {
            // InitLoadSceneTriggers();
            
            // 開始時のシーンを発行する
            var initialGameScene = GetCurrentActiveGameScene();
            CurrentGameScene = initialGameScene;
            onBeginChangeScene.OnNext(initialGameScene);
            onFinishChangeScene.OnNext(initialGameScene);
            
            Type managerType = sceneManagersType[initialGameScene];
            currentSceneManager = Activator.CreateInstance(managerType) as BaseSceneManager;
            currentSceneManager.BeforeLoadScene();
            currentSceneManager.AfterLoadScene();
        }

        #endregion


        #region 非公開の関数

        private static async void ChangeSceneManager(GameScene scene) {
            if (currentSceneManager != null) {
                currentSceneManager.Dispose();
                currentSceneManager = null;
            }
            Type managerType = sceneManagersType[scene];
            currentSceneManager = Activator.CreateInstance(managerType) as BaseSceneManager;
            currentSceneManager.BeforeLoadScene();

            await ObservableSceneEvent.ActiveSceneChangedAsObservable().First();
            currentSceneManager.AfterLoadScene();
        }


        /// <summary>
        ///   <para>Subscribe時にシーンをアニメーション付きで遷移させるObservableを返す</para>
        /// </summary>
        /// <remarks>
        ///   <para>Subscribe時、カーテンを閉じるアニメーションが再生される</para>
        ///   <para>カーテンが閉じた後、指定したシーンの読み込みが行われる</para>
        ///   <para>シーンの読み込みが完了すると、カーテンを開くアニメーションが再生される</para>
        ///   <para>カーテンを開くアニメーションが終了すると、即座にOnCompleted()が発行される</para>
        /// </remarks>
        /// <param name="processObservable">
        ///   <para>カーテンが閉じている間にシーンの読み込みと並列して処理を行い、処理の終了時にOnCompleted()が発行されるObservable</para>
        /// </param>
        private static IObservable<Unit> CreateSceneChangeObservable(GameScene scene, IObservable<Unit> processObservable) {
            if (scene == GameScene.Changing) return null;

            Subject<float> progressSubject = new Subject<float>();
            IObservable<float> loadSceneObservable = progressSubject.TakeUntil(
                Observable.Defer(
                    () => UnityEngine.SceneManagement.SceneManager
                        .LoadSceneAsync(scene.ToString(), LoadSceneMode.Single)
                        .AsAsyncOperationObservable(Progress.Create<float>(progress => progressSubject.OnNext(progress)))
                )
            )
                .DoOnSubscribe(() => onBeginChangeScene.OnNext(scene))
                .DoOnSubscribe(() => ChangeSceneManager(scene))
                .DoOnCompleted(() => onFinishChangeScene.OnNext(scene))
                .Publish()
                .RefCount();

            return CurtainAnimationView.Instance.CreateCurtainAnimationObservable(
                Observable.Merge(
                    loadSceneObservable,
                    processObservable.Select(_ => 0f).IgnoreElements()
                )
            )
                .DoOnSubscribe(() => CurrentGameScene = GameScene.Changing)
                .DoOnCompleted(() => CurrentGameScene = scene)
                .Publish()
                .RefCount();
        }

        #endregion


        #region 公開する関数

        /// <summary>
        ///   指定したGameSceneが開始したときに、OnNext()が発行されるObservableを返す
        /// </summary>
        /// <param name="scene">対象となるゲームシーン</para>
        /// <param name="isPublishOnStay">trueの場合、Subscribe時に既に対象のゲームシーンが実行されていたらOnNext()を発行する</para>
        public static IObservable<Unit> OnUpdateScene(GameScene scene, bool isPublishOnStay = true) {
            if (isPublishOnStay) {
                return Observable.Merge(
                    Observable.Defer(
                        () => {
                            if (CurrentGameScene == scene) return Observable.ReturnUnit();
                            else return Observable.Empty<Unit>();
                        }
                    ),
                    OnUpdateScene(scene, isPublishOnStay: false)
                );
            } else {
                return currentGameScene
                    .Where(x => x == scene)
                    .AsUnitObservable()
                    .Publish()
                    .RefCount();
            }
        }


        /// <summary>
        ///   指定したシーンの読み込みが開始したときに、OnNext()が発行されるObservableを返す
        /// </summary>
        /// <remarks>
        ///   Changing(シーンの切り替え中)を指定した場合、nullが返される
        /// </remarks>
        /// <param name="scene">対象のシーン</param>
        public static IObservable<Unit> OnBeginLoadScene(GameScene scene) {
            if (scene == GameScene.Changing) {
                return null;
            }
            return onBeginChangeScene.Where(x => x == scene).AsUnitObservable().Publish().RefCount();
        }


        /// <summary>
        ///   指定したシーンの読み込みが終了したときに、OnNext()が発行されるObservableを返す
        /// </summary>
        /// <remarks>
        ///   Changing(シーンの切り替え中)を指定した場合、nullが返される
        /// </remarks>
        /// <param name="scene">対象のシーン</param>
        public static IObservable<Unit> OnFinishLoadScene(GameScene scene) {
            if (scene == GameScene.Changing) {
                return null;
            }
            return onFinishChangeScene.Where(x => x == scene).AsUnitObservable().Publish().RefCount();
        }


        /// <summary>
        ///   カーテンのアニメーションとともにシーンを変更する
        /// </summary>
        /// <param name="scene">変更先のシーン</param>
        /// <param name="processObservable">シーン変更と並列で処理を行い、完了時にOnCompleted()が発行されるObservable</param>
        public static void ChangeSceneWithAnimation(GameScene scene, IObservable<Unit> processObservable = null) {
            if (processObservable == null) {
                processObservable = Observable.Empty<Unit>();
            }
            CreateSceneChangeObservable(scene, processObservable).Publish().Connect();
        }


        /// <summary>
        ///   現在実行中のゲームシーンのマネージャを返す
        /// </summary>
        public static T GetCurrentSceneManager<T>() where T : BaseSceneManager {
            return currentSceneManager as T;
        }


        /// <summary>
        ///   現在実行中のゲームシーンを返す
        /// </summary>
        public static GameScene GetCurrentActiveGameScene() {
            var currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            return (GameScene)Enum.Parse(typeof(GameScene), currentSceneName);
        }


        /// <summary>
        ///   ゲームを終了する
        /// </summary>
        public static void ExitGame() {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
            #endif
        }

        #endregion

    }
}
