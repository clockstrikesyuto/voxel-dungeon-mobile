using UnityEngine;

namespace VoxelDungeon.Core
{
    [RequireComponent(typeof(Collider))]
    public sealed class BossArenaBarrier : MonoBehaviour
    {
        [SerializeField] private float revealDistance = 2.2f;
        private Renderer barrierRenderer;
        private Transform player;

        private void Awake()
        {
            barrierRenderer = GetComponent<Renderer>();
            if (barrierRenderer != null)
                barrierRenderer.enabled = false;
        }

        private void OnEnable()
        {
            ResolvePlayer();
            if (barrierRenderer != null)
                barrierRenderer.enabled = false;
        }

        private void Update()
        {
            if (player == null)
            {
                ResolvePlayer();
                return;
            }

            if (barrierRenderer == null)
                return;

            Vector3 local = transform.InverseTransformPoint(player.position);
            float depthDistance = Mathf.Abs(local.z * transform.localScale.z);
            bool near = depthDistance <= revealDistance;

            // Normally invisible. Approaching it reveals a quick magical pulse.
            barrierRenderer.enabled = near && Mathf.Sin(Time.time * 9f) > -0.25f;
        }

        private void ResolvePlayer()
        {
            GameObject go = GameObject.Find("Player_Debug");
            if (go != null)
                player = go.transform;
        }
    }
}
