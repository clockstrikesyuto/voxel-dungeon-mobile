using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.Loot
{
    [RequireComponent(typeof(Health))]
    public sealed class EnemyDropper : MonoBehaviour
    {
        [SerializeField, Min(0)] private int goldMin = 2;
        [SerializeField, Min(0)] private int goldMax = 5;
        [SerializeField, Range(0f, 1f)] private float upgradeChance = 0.18f;
        [SerializeField, Min(1)] private int upgradePower = 3;
        [SerializeField] private bool boss;

        private Health health;

        public void Configure(int minGold, int maxGold, float chance, int power, bool isBoss)
        {
            goldMin = Mathf.Max(0, minGold);
            goldMax = Mathf.Max(goldMin, maxGold);
            upgradeChance = Mathf.Clamp01(chance);
            upgradePower = Mathf.Max(1, power);
            boss = isBoss;
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
            int gold = Random.Range(goldMin, goldMax + 1);
            if (gold > 0)
                LootPickup.Spawn(transform.position, LootPickup.LootKind.Gold, gold);

            if (boss || Random.value <= upgradeChance)
            {
                LootPickup.LootKind kind = Random.value < 0.5f
                    ? LootPickup.LootKind.MeleePower
                    : LootPickup.LootKind.RangedPower;

                LootPickup.Spawn(transform.position + transform.right * 0.45f, kind, boss ? upgradePower * 2 : upgradePower);
            }
        }
    }
}
