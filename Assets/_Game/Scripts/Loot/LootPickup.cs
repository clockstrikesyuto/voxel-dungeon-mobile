using UnityEngine;
using VoxelDungeon.Player;

namespace VoxelDungeon.Loot
{
    public sealed class LootPickup : MonoBehaviour
    {
        public enum LootKind
        {
            Gold,
            MeleePower,
            RangedPower
        }

        [SerializeField] private LootKind kind;
        [SerializeField, Min(1)] private int amount = 1;
        [SerializeField, Min(0.1f)] private float pickupRadius = 1.1f;
        [SerializeField, Min(0f)] private float bobHeight = 0.18f;
        [SerializeField, Min(0f)] private float bobSpeed = 3f;

        private Transform player;
        private Vector3 basePosition;

        public void Configure(LootKind pickupKind, int pickupAmount)
        {
            kind = pickupKind;
            amount = Mathf.Max(1, pickupAmount);
            ApplyVisual();
        }

        private void Start()
        {
            basePosition = transform.position;
            GameObject go = GameObject.Find("Player_Debug");
            if (go != null)
                player = go.transform;
            ApplyVisual();
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, 90f * Time.deltaTime, Space.World);
            transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);

            if (player == null)
            {
                GameObject go = GameObject.Find("Player_Debug");
                if (go != null)
                    player = go.transform;
                return;
            }

            Vector3 delta = player.position - transform.position;
            delta.y = 0f;

            if (delta.sqrMagnitude <= pickupRadius * pickupRadius)
                Collect(player);
        }

        private void Collect(Transform target)
        {
            PlayerProgress progress = target.GetComponent<PlayerProgress>();
            if (progress == null)
                return;

            switch (kind)
            {
                case LootKind.Gold:
                    progress.AddGold(amount);
                    break;
                case LootKind.MeleePower:
                    progress.AddMeleePower(amount);
                    break;
                case LootKind.RangedPower:
                    progress.AddRangedPower(amount);
                    break;
            }

            Destroy(gameObject);
        }

        private void ApplyVisual()
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer == null)
                return;

            switch (kind)
            {
                case LootKind.Gold:
                    renderer.material.color = new Color(1f, 0.78f, 0.12f, 1f);
                    break;
                case LootKind.MeleePower:
                    renderer.material.color = new Color(1f, 0.28f, 0.14f, 1f);
                    break;
                case LootKind.RangedPower:
                    renderer.material.color = new Color(0.45f, 0.3f, 1f, 1f);
                    break;
            }
        }

        public static LootPickup Spawn(Vector3 position, LootKind kind, int amount)
        {
            PrimitiveType primitive = kind == LootKind.Gold ? PrimitiveType.Sphere : PrimitiveType.Cube;
            GameObject go = GameObject.CreatePrimitive(primitive);
            go.name = $"Loot_{kind}";
            go.transform.position = position + Vector3.up * 0.55f;
            go.transform.localScale = kind == LootKind.Gold
                ? Vector3.one * 0.38f
                : new Vector3(0.45f, 0.65f, 0.45f);

            Collider collider = go.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            LootPickup pickup = go.AddComponent<LootPickup>();
            pickup.Configure(kind, amount);
            return pickup;
        }
    }
}
