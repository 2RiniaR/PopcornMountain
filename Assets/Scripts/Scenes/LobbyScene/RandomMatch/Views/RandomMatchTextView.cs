using UnityEngine;
using TMPro;


namespace PopcornMountain.LobbyScene.RandomMatch {
    public class RandomMatchTextView : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI indicatorText = null;

        private void Start() {
            indicatorText.text = "  人がプレイ中";
        }

        public void SetUserCount(int count) {
            indicatorText.text = ((count > 999) ? "999+" : count.ToString()) + " 人がプレイ中";
        }

    }
}
