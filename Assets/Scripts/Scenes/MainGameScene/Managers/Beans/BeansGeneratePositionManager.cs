using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public class BeansGeneratePositionManager {

        private const string gameObjectName = "PopcornGenerateGrid";
        private const int xGridCount = 4;
        private const int yGridCount = 3;
        private static readonly Vector2 gridSize = new Vector2(2f, 2f);
        private static readonly Vector2 bottomLeftPosition = new Vector2(-3f, 8f);
        private int popcornLayerMask = 0;
        private List<int> usedGenerationOrder = new List<int>();
        private List<int> generationOrder = Enumerable.Range(0, xGridCount * yGridCount).OrderBy(i => Guid.NewGuid()).ToList();
        private BoxCollider2D[] generationGridsCollider = null;


        public BeansGeneratePositionManager() {
            popcornLayerMask = LayerMask.GetMask("Popcorn");
            var gameObject = new GameObject(gameObjectName);
            generationGridsCollider = new BoxCollider2D[xGridCount * yGridCount];
            for (int i = 0; i < xGridCount * yGridCount; i++) {
                var x = i / yGridCount;
                var y = i % xGridCount;
                var collider = gameObject.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.offset = bottomLeftPosition + Vector2.Scale(gridSize, new Vector2(x, y));
                collider.size = gridSize;
                generationGridsCollider[i] = collider;
            }
        }


        public Vector2? GetGeneratePosition() {
            for (int i = 0; i < generationOrder.Count; i++) {
                int num = generationOrder[i];
                if (generationGridsCollider[num].IsTouchingLayers(popcornLayerMask)) {
                    continue;
                }
                generationOrder.RemoveAt(i);
                usedGenerationOrder.Add(num);
                return generationGridsCollider[num].offset;
            }

            return null;
        }


        public void RefleshGenerationOrder() {
            generationOrder.AddRange(usedGenerationOrder);
            usedGenerationOrder.Clear();
        }

    }
}
