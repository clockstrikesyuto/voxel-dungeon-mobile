using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.Loot
{
    [RequireComponent(typeof(Health))]
    public sealed class EquipmentDropper : MonoBehaviour
    {
        [SerializeField] private string[] itemIds;
        [SerializeField, Range(0f, 1f)] private float chance = 0.12f;
        [SerializeField] private bool guaranteed;

        private Health health;

        public void Configure(string[] ids, float dropChance, bool isGuaranteed = false)
        {
            itemIds = ids;
            chance = Mathf.Clamp01(dropChance);
            guaranteed = isGuaranteed;
        }

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            if (health != null)
                health.Died += Drop;
        }

        private void OnDisable()
        {
            if (health != null)
                health.Died -= Drop;
        }

        private void Drop()
        {
            if (itemIds == null || itemIds.Length == 0)
                return;

            if (!guaranteed && Random.value > chance)
                return;

            string id = itemIds[Random.Range(0, itemIds.Length)];
            EquipmentPickup.Spawn(transform.position + transform.right * 0.65f, id);
        }
    }
}
