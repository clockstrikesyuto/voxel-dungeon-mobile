using UnityEngine;
using VoxelDungeon.Core;

namespace VoxelDungeon.Loot
{
    public sealed class AdventureItemPickup : MonoBehaviour
    {
        [SerializeField] private string itemId;
        [SerializeField] private int amount = 1;
        [SerializeField] private bool consumable;
        [SerializeField] private float pickupRadius = 1.15f;

        private Transform player;
        private Vector3 basePosition;

        public void Configure(string id, int count, bool isConsumable)
        {
            itemId = id;
            amount = Mathf.Max(1, count);
            consumable = isConsumable;
            ApplyVisual();
        }

        private void Start()
        {
            basePosition = transform.position;
            ResolvePlayer();
            ApplyVisual();
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, 70f * Time.deltaTime, Space.World);
            transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.time * 3.1f) * 0.14f);

            if (player == null)
            {
                ResolvePlayer();
                return;
            }

            Vector3 delta = player.position - transform.position;
            delta.y = 0f;
            if (delta.sqrMagnitude <= pickupRadius * pickupRadius)
                Collect();
        }

        private void ResolvePlayer()
        {
            GameObject go = GameObject.Find("Player_Debug");
            if (go != null)
                player = go.transform;
        }

        private void Collect()
        {
            if (consumable)
            {
                ProfileProgress.AddConsumable(itemId, amount);
                MissionRunStats.AddConsumable(itemId, amount);
            }
            else
            {
                ProfileProgress.AddMaterial(itemId, amount);
                MissionRunStats.AddMaterial(itemId, amount);
            }

            Destroy(gameObject);
        }

        private void ApplyVisual()
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer == null)
                return;

            renderer.material.color = GetColor(itemId, consumable);
        }

        private static Color GetColor(string id, bool isConsumable)
        {
            if (isConsumable)
            {
                if (id == "healing_potion") return new Color(0.18f, 0.95f, 0.42f);
                if (id == "power_tonic") return new Color(1f, 0.28f, 0.12f);
                if (id == "guard_tonic") return new Color(0.20f, 0.62f, 1f);
                if (id == "haste_tonic") return new Color(1f, 0.78f, 0.18f);
                if (id == "fire_bomb") return new Color(1f, 0.34f, 0.05f);
                return new Color(0.42f, 0.78f, 1f);
            }

            if (id == "crystal_shard") return new Color(0.10f, 0.90f, 1f);
            if (id == "iron_ore") return new Color(0.52f, 0.55f, 0.58f);
            if (id == "ember_core") return new Color(1f, 0.28f, 0.04f);
            if (id == "moon_bloom") return new Color(0.72f, 0.58f, 1f);
            if (id == "void_fragment") return new Color(0.42f, 1f, 0.62f);
            return new Color(1f, 0.78f, 0.18f);
        }

        public static AdventureItemPickup Spawn(Vector3 position, string id, int amount, bool consumable = false)
        {
            GameObject go = GameObject.CreatePrimitive(consumable ? PrimitiveType.Capsule : PrimitiveType.Cube);
            go.name = (consumable ? "Item_" : "Material_") + id;
            go.transform.position = position + Vector3.up * 0.58f;
            go.transform.localScale = consumable
                ? new Vector3(0.28f, 0.42f, 0.28f)
                : new Vector3(0.36f, 0.48f, 0.36f);

            Collider collider = go.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            AdventureItemPickup pickup = go.AddComponent<AdventureItemPickup>();
            pickup.Configure(id, amount, consumable);
            return pickup;
        }
    }
}
