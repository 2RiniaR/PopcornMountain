using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornTextEffectView : BaseMainGameComponent {
        
        [SerializeField]
        private Image edgeTextPrefab;
        [SerializeField]
        private Image objectTextPrefab;
        
        /*
         * goodPhrases[5n+0]: 上向き
         * goodPhrases[5n+1]: 右向き
         * goodPhrases[5n+2]: 下向き
         * goodPhrases[5n+3]: 左向き
         * goodPhrases[5n+4]: 方向なし
        */
        [SerializeField]
        private Sprite[] goodPhrases;
        [SerializeField]
        private Sprite[] badPhrases;
        [SerializeField]
        private Sprite[] burnedPhrases;
        
        
        private RectTransform textEffectCanvas;
        
        
        private void Start() {
            // 何かがもどかしい、TextEffect用のマネージャクラスを作ったりして参照を持てないだろうか
            textEffectCanvas = GameObject.FindGameObjectWithTag("TextEffectCanvas").GetComponent<RectTransform>();
        }
        
        // クソコード錬成機と化したwatαno
        // 辞書形式にすればいいものを...
        public void PlayTextEffectOnGameObject(EatingEffectType type) {
            switch(type) {
                case EatingEffectType.GettingGoodBeans:
                    InstantiateTextObjectOnGameObject(
                        goodPhrases[UnityEngine.Random.Range(0, goodPhrases.Length / 5 - 1) * 5 + 4]
                    );
                    break;
                case EatingEffectType.GettingBadBeans:
                    InstantiateTextObjectOnGameObject(
                        badPhrases[UnityEngine.Random.Range(0, badPhrases.Length / 5 - 1) * 5 + 4]
                    );
                    break;
                case EatingEffectType.GettingBurnedBeans:
                    InstantiateTextObjectOnGameObject(
                        burnedPhrases[UnityEngine.Random.Range(0, burnedPhrases.Length / 5 - 1) * 5 + 4]
                    );
                    break;
            }
        }
        
        public void PlayTextEffectEdgeOfScreen(EatingEffectType type) {
            switch(type) {
                case EatingEffectType.GettingGoodBeans:
                    InstantiateTextObjectEdgeOfScreen(
                        goodPhrases.Skip(UnityEngine.Random.Range(0, goodPhrases.Length / 5 - 1) * 5).Take(4)
                    );
                    break;
                case EatingEffectType.GettingBadBeans:
                    InstantiateTextObjectEdgeOfScreen(
                        badPhrases.Skip(UnityEngine.Random.Range(0, badPhrases.Length / 5 - 1) * 5).Take(4)
                        // badPhrases[UnityEngine.Random.Range(0, badPhrases.Length-1)]
                    );
                    break;
                case EatingEffectType.GettingBurnedBeans:
                    InstantiateTextObjectEdgeOfScreen(
                        burnedPhrases.Skip(UnityEngine.Random.Range(0, burnedPhrases.Length / 5 - 1) * 5).Take(4)
                        // burnedPhrases[UnityEngine.Random.Range(0, burnedPhrases.Length-1)]
                    );
                    break;
            }
        }
        
        private void InstantiateTextObjectOnGameObject(Sprite sprite) {
            var pos = Vector3.Scale(transform.position, new Vector3(1, 1, 0));
            var rot = new Quaternion(0, 0, 0, 0);
            var inst = Instantiate(objectTextPrefab, pos, rot, textEffectCanvas.transform);
            inst.sprite = sprite;
        }
        
        private void InstantiateTextObjectEdgeOfScreen(IEnumerable<Sprite> sprites) {
            var pos = Vector3.Scale(transform.position, new Vector3(1, 1, 0));
            var rot = new Quaternion(0, 0, 0, 0);
            var inst = Instantiate(edgeTextPrefab, pos, rot, textEffectCanvas.transform);
            StrictPositionInCanvas(inst.rectTransform, textEffectCanvas);
            SetSpriteWithPosition(inst, sprites, inst.rectTransform);
        }
        
        /// <summary>
        ///   <para>obj の位置を canvas の中に収める</para>
        ///   <para>obj に対して破壊的動作のため注意</para>
        /// </summary>
        private void StrictPositionInCanvas(RectTransform obj, RectTransform canvas) {
            var canvasSize = canvas.sizeDelta;
            var objSize = obj.sizeDelta;
            var objPositionOnCanvas = obj.anchoredPosition;
            
            Vector2 anc = new Vector2(0.5f, 0.5f);
            Vector2 pos = objPositionOnCanvas;
            
            if ((canvasSize.x / 2f) < (objPositionOnCanvas.x + objSize.x / 2f)) {
                anc += new Vector2(0.5f, 0f);
                pos *= new Vector2(0, 1);
            } else if ((objPositionOnCanvas.x - objSize.x / 2f) < (-canvasSize.x / 2f)) {
                anc += new Vector2(-0.5f, 0f);
                pos *= new Vector2(0, 1);
            }
            
            if ((canvasSize.y / 2f) < (objPositionOnCanvas.y + objSize.y / 2f)) {
                anc += new Vector2(0f, 0.5f);
                pos *= new Vector2(1, 0);
            } else if ((objPositionOnCanvas.y - objSize.y / 2f) < (-canvasSize.y / 2f)) {
                anc += new Vector2(0f, -0.5f);
                pos *= new Vector2(1, 0);
            }
            
            obj.anchorMax = anc;
            obj.anchorMin = anc;
            obj.pivot = anc;
            obj.anchoredPosition = pos;
        }
        
        // オブジェクトの位置からスプライトを決定する
        private void SetSpriteWithPosition(Image image, IEnumerable<Sprite> sprites, RectTransform obj) {
            float[] marginValues = new float[4]{
                1f - obj.anchorMax.y,
                1f - obj.anchorMax.x,
                obj.anchorMax.y,
                obj.anchorMax.x
            };
            
            int spriteIndex = 0;
            float minValue = float.MaxValue;
            for (int i=0; i<marginValues.Length; i++) {
                if (marginValues[i] <= minValue) {
                    minValue = marginValues[i];
                    spriteIndex = i;
                }
            }
            image.sprite = sprites.ToArray()[spriteIndex];
        }
        
    }
}
