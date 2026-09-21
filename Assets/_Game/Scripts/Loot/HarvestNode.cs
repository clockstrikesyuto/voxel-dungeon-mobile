using UnityEngine;

namespace VoxelDungeon.Loot
{
    public sealed class HarvestNode : MonoBehaviour
    {
        [SerializeField] private string materialId = "crystal_shard";
        [SerializeField] private int minAmount = 1;
        [SerializeField] private int maxAmount = 3;
        [SerializeField] private float harvestRadius = 1.55f;

        private Transform player;
        private Vector3 baseScale;
        private bool harvested;

        public void Configure(string id, int min, int max, float radius = 1.55f)
        {
            materialId = id;
            minAmount = Mathf.Max(1, min);
            maxAmount = Mathf.Max(minAmount, max);
            harvestRadius = Mathf.Max(0.5f, radius);
        }

        private void Start()
        {
            baseScale = transform.localScale;
            ResolvePlayer();
        }

        private void Update()
        {
            if (harvested)
                return;

            float pulse = 1f + Mathf.Sin(Time.time * 2.6f) * 0.045f;
            transform.localScale = baseScale * pulse;
            transform.Rotate(Vector3.up, 16f * Time.deltaTime, Space.World);

            if (player == null)
            {
                ResolvePlayer();
                return;
            }

            Vector3 delta = player.position - transform.position;
            delta.y = 0f;

            if (delta.sqrMagnitude <= harvestRadius * harvestRadius)
                Harvest();
        }

        private void ResolvePlayer()
        {
            GameObject go = GameObject.Find("Player_Debug");
            if (go != null)
                player = go.transform;
        }

        private void Harvest()
        {
            harvested = true;
            int amount = Random.Range(minAmount, maxAmount + 1);

            for (int i = 0; i < amount; i++)
            {
                Vector3 offset = new Vector3(
                    (i - (amount - 1) * 0.5f) * 0.35f,
                    0f,
                    0.25f * (i % 2));

                AdventureItemPickup.Spawn(
                    transform.position + offset,
                    materialId,
                    1);
            }

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer != null)
                    renderer.enabled = false;
            }

            Destroy(gameObject, 0.15f);
        }
    }
}
