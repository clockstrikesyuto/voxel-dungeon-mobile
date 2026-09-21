using UnityEngine;
using VoxelDungeon.Core;
using VoxelDungeon.Items;

namespace VoxelDungeon.Loot
{
    public sealed class EquipmentPickup : MonoBehaviour
    {
        [SerializeField] private string itemId;
        [SerializeField] private float pickupRadius = 1.25f;
        private Transform player;
        private Vector3 basePosition;

        public void Configure(string id)
        {
            itemId = id;
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
            transform.Rotate(Vector3.up, 72f * Time.deltaTime, Space.World);
            transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.time * 3.2f) * 0.16f);

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
            EquipmentRecord item = EquipmentCatalog.Get(itemId);

            if (ProfileProgress.AddEquipment(itemId))
            {
                EquipmentToastUI.Show(item);
            }
            else if (ProfileProgress.TryImproveEquipmentRoll(itemId))
            {
                EquipmentToastUI.Show(item);
            }
            else
            {
                int rarityTier = item.Rarity switch
                {
                    ItemRarity.Legendary => 4,
                    ItemRarity.Epic => 3,
                    ItemRarity.Rare => 2,
                    _ => 1
                };

                ProfileProgress.AddGold(5 + rarityTier * 5);
                ProfileProgress.AddMaterial(
                    StageLootCatalog.PrimaryMaterial(),
                    Mathf.Max(1, rarityTier - 1));
            }

            Destroy(gameObject);
        }

        private void ApplyVisual()
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            if (renderer == null)
                return;

            EquipmentRecord item = EquipmentCatalog.Get(itemId);
            renderer.material.color = EquipmentCatalog.GetRarityColor(item.Rarity);
        }

        public static EquipmentPickup Spawn(Vector3 position, string itemId)
        {
            EquipmentRecord item = EquipmentCatalog.Get(itemId);

            GameObject go = GameObject.CreatePrimitive(
                item.Slot == EquipmentSlot.Melee ? PrimitiveType.Cube : PrimitiveType.Cylinder);

            go.name = "Equipment_" + item.Id;
            go.transform.position = position + Vector3.up * 0.75f;
            go.transform.localScale = item.Slot == EquipmentSlot.Melee
                ? new Vector3(0.20f, 0.95f, 0.18f)
                : new Vector3(0.32f, 0.48f, 0.32f);

            Collider collider = go.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            EquipmentPickup pickup = go.AddComponent<EquipmentPickup>();
            pickup.Configure(itemId);
            return pickup;
        }
    }
}
