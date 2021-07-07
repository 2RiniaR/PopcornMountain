using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain {
/*
    public static class CursorManager {

        #region 定数

        private const string openHandImagePath = "Sprites/hand_normal";
        private const string closeHandImagePath = "Sprites/hand_grap";
        private static readonly Vector2 cursorImagePositionOffset = new Vector2(15f, 15f);
        private const int leftClickMouseButtonNumber = 0;

        #endregion


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            var openHandImage = Resources.Load(openHandImagePath) as Texture2D;
            var closeHandImage = Resources.Load(closeHandImagePath) as Texture2D;

            Observable.EveryUpdate()
                .Select(_ => Input.GetMouseButton(leftClickMouseButtonNumber))
                .Subscribe(isButtonDown => Cursor.SetCursor(isButtonDown ? closeHandImage : openHandImage, cursorImagePositionOffset, CursorMode.ForceSoftware));
        }
    }
*/


    public sealed class CursorManager : SingletonMonoBehaviour<CursorManager> {

        #region 定数

        private const string cursorCanvasPrefabPath = "Prefabs/UI/CursorCanvas";
        private static readonly Vector2 cursorImagePositionOffset = new Vector2(15f, -15f);
        private const int leftClickMouseButtonNumber = 0;

        #endregion


        private static RectTransform canvasRectTransform = null;
        [SerializeField]
        private RectTransform cursorRectTransform = null;
        [SerializeField]
        private Image cursorHandImage = null;
        [SerializeField]
        private Image disableImage = null;
        [SerializeField]
        private Sprite openHandSprite = null;
        [SerializeField]
        private Sprite closeHandSprite = null;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() {
            Cursor.visible = false;
            GameObject.Instantiate((GameObject)Resources.Load(cursorCanvasPrefabPath));
        }


        protected override void Awake() {
            base.Awake();
            canvasRectTransform = GetComponent<RectTransform>();

            Observable.EveryUpdate()
                .Subscribe(_ => {
                    Cursor.visible = false;
                    UpdateCursorPosition();
                });

            Observable.EveryUpdate()
                .Select(_ => Input.GetMouseButton(leftClickMouseButtonNumber))
                .ToReactiveProperty()
                .Subscribe(isButtonDown => {
                    cursorHandImage.sprite = isButtonDown ? closeHandSprite : openHandSprite;
                });
        }


        private void UpdateCursorPosition() {
            Vector2 viewPoint = Camera.main.ScreenToViewportPoint(Vector3.Scale(Input.mousePosition, new Vector3(1, 1, 0)));
            Vector2 localpoint = new Vector2(
                ((viewPoint .x * canvasRectTransform.sizeDelta.x) - (canvasRectTransform.sizeDelta.x * 0.5f)),
                ((viewPoint .y * canvasRectTransform.sizeDelta.y) - (canvasRectTransform.sizeDelta.y * 0.5f))
            );
            cursorRectTransform.anchoredPosition = localpoint + cursorImagePositionOffset;
        }


        public void SetDisableImageDisplay(bool isDisplay) {
            disableImage.enabled = isDisplay;
        }

    }

}