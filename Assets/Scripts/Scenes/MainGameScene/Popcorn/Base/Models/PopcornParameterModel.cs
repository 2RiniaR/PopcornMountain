namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornParameterModel : BaseMainGameComponent {

        #region 公開するプロパティ

        /// <summary>
        ///   <para>現在のポップコーンのパラメータ</para>
        /// </summary>
        public PopcornParameters parameters;

        #endregion


        #region 初期化用の関数

        public void Init(PopcornParameters initialParameters) {
            parameters = initialParameters;
        }

        #endregion

    }
}
