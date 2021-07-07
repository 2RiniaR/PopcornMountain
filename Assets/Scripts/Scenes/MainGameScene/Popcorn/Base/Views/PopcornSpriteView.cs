using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornSpriteView : BaseMainGameComponent {

        private SpriteRenderer spriteRenderer = null;
        private SpriteMask spriteMask = null;


        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteMask = GetComponent<SpriteMask>();

            spriteRenderer.ObserveEveryValueChanged(spriteRenderer => spriteRenderer.sprite)
                .Subscribe(x => spriteMask.sprite = x)
                .AddTo(this);
        }

    }
}
