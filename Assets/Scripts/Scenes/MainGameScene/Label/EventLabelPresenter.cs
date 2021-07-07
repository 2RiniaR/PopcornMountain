using UniRx;


namespace PopcornMountain.MainGameScene.Label {
    public class EventLabelPresenter : BaseMainGameComponent {

        #region 定数

        /// <summary>
        ///   各イベント発生時にラベルに表示するテキスト
        /// </summary>
        private static readonly string[] eventLabelTexts = new string[] {
            "ばくだんラッシュ！",
            "あつあつラッシュ！",
            "きらきらラッシュ！",
            "ご注文はなんなりと！",
            "危険物にご注意！"
        };

        #endregion


        private void Start() {
            var labelView = GetComponent<LabelView>();
            var eventManager = GetManager<EventManager>();

            eventManager.OnCookingEventObservable
                .Subscribe(type => {
                    string eventText = eventLabelTexts[(int)type];
                    labelView.CreatePlayingObservable(eventText)
                        .Subscribe(
                            _ => { },
                            () => eventManager.RunEvent(type)
                        )
                        .AddTo(this);
                })
                .AddTo(this);
        }

    }
}
