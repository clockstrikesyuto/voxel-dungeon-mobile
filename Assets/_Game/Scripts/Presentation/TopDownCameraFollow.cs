using UnityEngine;

namespace VoxelDungeon.Presentation
{
    public sealed class TopDownCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 9f, -7f);
        [SerializeField, Min(0f)] private float followSharpness = 12f;
        [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1f, 0f);

        public void SetTarget(Transform value) => target = value;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = target.position + offset;
            float t = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, desired, t);
            transform.LookAt(target.position + lookOffset);
        }
    }
}
