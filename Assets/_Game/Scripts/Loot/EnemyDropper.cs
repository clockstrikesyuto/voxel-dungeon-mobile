using UnityEngine;
using UnityEngine.SceneManagement;
using VoxelDungeon.Combat;
using VoxelDungeon.Core;

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

                LootPickup.Spawn(
                    transform.position + transform.right * 0.45f,
                    kind,
                    boss ? upgradePower * 2 : upgradePower);
            }

            int xp = boss ? 55 : Random.Range(8, 14);
            ProfileProgress.AddExperience(xp);

            string material = ResolveStageMaterial();
            float materialChance = boss ? 1f : 0.34f;
            if (Random.value <= materialChance)
            {
                int amount = boss ? Random.Range(4, 7) : Random.Range(1, 3);
                AdventureItemPickup.Spawn(
                    transform.position - transform.right * 0.48f,
                    material,
                    amount);
            }

            if (boss)
            {
                string bossMaterial = ResolveBossMaterial();
                AdventureItemPickup.Spawn(
                    transform.position + transform.forward * 0.65f,
                    bossMaterial,
                    Random.Range(2, 4));

                AdventureItemPickup.Spawn(
                    transform.position - transform.forward * 0.65f,
                    "healing_potion",
                    1,
                    true);
            }
            else if (Random.value <= 0.08f)
            {
                AdventureItemPickup.Spawn(
                    transform.position + transform.forward * 0.42f,
                    "healing_potion",
                    1,
                    true);
            }
        }

        private static string ResolveStageMaterial()
        {
            string scene = SceneManager.GetActiveScene().name;
            if (scene == "Mission_Ashen")
                return Random.value < 0.65f ? "iron_ore" : "ember_core";
            if (scene == "Mission_Void")
                return Random.value < 0.60f ? "moon_bloom" : "void_fragment";
            return "crystal_shard";
        }

        private static string ResolveBossMaterial()
        {
            string scene = SceneManager.GetActiveScene().name;
            if (scene == "Mission_Ashen")
                return "ember_core";
            if (scene == "Mission_Void")
                return "void_fragment";
            return "ancient_relic";
        }
    }
}
