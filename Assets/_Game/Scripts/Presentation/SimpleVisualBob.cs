using UnityEngine;

namespace VoxelDungeon
{
    public sealed class SimpleVisualBob : MonoBehaviour
    {
        [SerializeField] private Transform visual;
        [SerializeField] private float amplitude = 0.04f;
        [SerializeField] private float speed = 5f;

        private Vector3 baseLocalPosition;
        private float phase;

        public void Configure(Transform target, float bobAmplitude, float bobSpeed)
        {
            visual = target;
            amplitude = bobAmplitude;
            speed = bobSpeed;
            if (visual != null)
                baseLocalPosition = visual.localPosition;
        }

        private void Awake()
        {
            phase = Random.value * Mathf.PI * 2f;
            if (visual != null)
                baseLocalPosition = visual.localPosition;
        }

        private void LateUpdate()
        {
            if (visual == null)
                return;

            Vector3 pos = baseLocalPosition;
            pos.y += Mathf.Sin(Time.time * speed + phase) * amplitude;
            visual.localPosition = pos;
        }
    }
}
