using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace PopcornMountain.MainGameScene.Popcorn {
    public class PopcornBubbleParticleView : MonoBehaviour {

        [SerializeField]
        private ParticleSystem bubbleParticle = null;
        private static Mesh baseMesh = null;
        private List<CombineInstance> combineInstances = new List<CombineInstance>();
        private float power = 0f;


        private void Awake() {
            if (bubbleParticle == null) return;
            var s = bubbleParticle.shape;
            s.mesh = new Mesh();
        }


        private void Update() {
            if (bubbleParticle == null) return;
            var s = bubbleParticle.shape;
            s.mesh.Clear();
            if (combineInstances.Count > 0) {
                SetEmission(this.power);
                s.mesh.CombineMeshes(combineInstances.ToArray());
            } else {
                StopEmission();
            }
        }


        private void FixedUpdate() {
            combineInstances.Clear();
        }


        private void OnCollisionStay2D(Collision2D other) {
            var points = new List<ContactPoint2D>();
            other.GetContacts(points);
            combineInstances.AddRange(
                points.Select(point => new CombineInstance() {
                    mesh = baseMesh,
                    transform = Matrix4x4.Translate(Vector3.Scale(transform.localScale, transform.InverseTransformPoint(point.point)))
                })
            );
            points.Clear();
        }


        private void OnDestroy() {
            if (bubbleParticle == null) return;
            var s = bubbleParticle.shape;
            Destroy(s.mesh);
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void GenerateBaseMesh() {
            baseMesh = new Mesh();
            baseMesh.vertices = new Vector3[] {
                new Vector3(0f, 0f),
                new Vector3(0.1f, 0f),
                new Vector3(0.1f, 0.2f),
                new Vector3(0f, 0.2f),
            };
            baseMesh.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
            baseMesh.RecalculateNormals();
            baseMesh.RecalculateBounds();
        }


        private void SetEmission(float power) {
            if (bubbleParticle == null) return;
            var e = bubbleParticle.emission;
            e.rateOverTimeMultiplier = Mathf.Pow(power, 2) * 50f;
            if (power <= 0f) {
                bubbleParticle.Stop();
                return;
            }
            if (!bubbleParticle.isPlaying) {
                bubbleParticle.Play();
            }
        }

        private void StopEmission() {
            SetEmission(0f);
        }



        /// <summary>
        ///   <para></para>
        ///   <param name="power">泡の強さ。(0 <= power <= 1)</param>
        /// </summary>
        public void SetBubbleParticle(float power) {
            this.power = power;
        }

        public void StopBubbleParticle() {
            SetBubbleParticle(0f);
        }

    }
}
