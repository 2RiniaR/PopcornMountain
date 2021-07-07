using UnityEngine;


namespace PopcornMountain.MainGameScene.PlayerIndicator {
    public class PlayerIndicatorNameView : BaseMainGameComponent {

        [SerializeField]
        private TMPro.TextMeshProUGUI nicknameText = null;

        public void SetNickName(string nickname) {
            nicknameText.text = nickname;
        }

    }
}
