using UnityEngine;
using TMPro;


namespace PopcornMountain.LobbyScene.User {
    public class UserRankView : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI indicatorText = null;


        public void SetText(string text) {
            indicatorText.text = text;
        }

    }
}
