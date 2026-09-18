#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using VoxelDungeon.AI;
using VoxelDungeon.Combat;
using VoxelDungeon.Core;
using VoxelDungeon.Loot;
using VoxelDungeon.Player;
using VoxelDungeon.UI;

namespace VoxelDungeon.EditorTools
{
    public static class AshenForgeSliceBuilder
    {
        public static void PopulateMission(GameObject player)
        {
            PlayerProgress progress = player.GetComponent<PlayerProgress>();
            if (progress == null)
                progress = player.AddComponent<PlayerProgress>();

            if (player.GetComponent<ProgressHudUI>() == null)
                player.AddComponent<ProgressHudUI>();

            List<Health> regular = new List<Health>();

            Health raiderA = CreateEnemy("Raider_Forge_A", new Vector3(-4f, 1f, -15f), Vector3.one, 120, 2.9f, 14, 0.92f, 0.30f, player.transform);
            Health raiderB = CreateEnemy("Raider_Forge_B", new Vector3(4f, 1f, -10f), Vector3.one, 120, 2.9f, 14, 0.92f, 0.30f, player.transform);
            regular.Add(raiderA); regular.Add(raiderB);

            Health scoutA = CreateEnemy("Scout_Forge_A", new Vector3(-5f, 1f, 0f), new Vector3(0.75f, 0.84f, 0.75f), 80, 4.4f, 10, 0.68f, 0.18f, player.transform);
            Health bruteA = CreateEnemy("Brute_Forge_A", new Vector3(5f, 1.25f, 4f), new Vector3(1.38f, 1.38f, 1.38f), 220, 1.8f, 24, 1.28f, 0.54f, player.transform);
            regular.Add(scoutA); regular.Add(bruteA);

            Health raiderC = CreateEnemy("Raider_Forge_C", new Vector3(-5f, 1f, 17f), Vector3.one, 135, 3.0f, 15, 0.88f, 0.28f, player.transform);
            Health raiderD = CreateEnemy("Raider_Forge_D", new Vector3(5f, 1f, 21f), Vector3.one, 135, 3.0f, 15, 0.88f, 0.28f, player.transform);
            Health scoutB = CreateEnemy("Scout_Forge_B", new Vector3(0f, 1f, 19f), new Vector3(0.75f, 0.84f, 0.75f), 85, 4.5f, 11, 0.66f, 0.18f, player.transform);
            regular.Add(raiderC); regular.Add(raiderD); regular.Add(scoutB);

            Health bruteB = CreateEnemy("Brute_Forge_B", new Vector3(-5f, 1.25f, 33f), new Vector3(1.42f, 1.42f, 1.42f), 245, 1.85f, 26, 1.22f, 0.52f, player.transform);
            Health bruteC = CreateEnemy("Brute_Forge_C", new Vector3(5f, 1.25f, 36f), new Vector3(1.42f, 1.42f, 1.42f), 245, 1.85f, 26, 1.22f, 0.52f, player.transform);
            regular.Add(bruteB); regular.Add(bruteC);

            CreateEncounterZone("ForgeEncounter_Smelter", new Vector3(0f, 1.8f, -13f), new Vector3(16f, 4f, 10f), raiderA.gameObject, raiderB.gameObject);
            CreateEncounterZone("ForgeEncounter_Cross", new Vector3(0f, 1.8f, 1f), new Vector3(19f, 4f, 14f), scoutA.gameObject, bruteA.gameObject);
            CreateEncounterZone("ForgeEncounter_Furnace", new Vector3(0f, 1.8f, 18f), new Vector3(16f, 4f, 12f), raiderC.gameObject, raiderD.gameObject, scoutB.gameObject);
            CreateEncounterZone("ForgeEncounter_Core", new Vector3(0f, 1.8f, 34f), new Vector3(18f, 4f, 12f), bruteB.gameObject, bruteC.gameObject);

            CreateTreasureChest(new Vector3(-12f, 0.45f, 18f), "ForgeChest_Amber", new Color(0.34f, 0.24f, 0.15f), new Color(1f, 0.48f, 0.08f), 34, 12);
            CreateTreasureChest(new Vector3(8f, 0.45f, 3f), "ForgeChest_Iron", new Color(0.30f, 0.31f, 0.32f), new Color(1f, 0.72f, 0.16f), 28, 10);
            CreateTreasureChest(new Vector3(-7f, 0.45f, 35f), "ForgeChest_Core", new Color(0.34f, 0.18f, 0.12f), new Color(1f, 0.82f, 0.18f), 40, 14);

            GameObject boss = CreateBoss(player.transform);
            Health bossHealth = boss.GetComponent<Health>();

            Material gateMaterial = VisualStyleBuilder.GetMaterial("Forge_SealedGate", new Color(0.28f, 0.28f, 0.29f), 0.62f, 0.46f);
            Material barrierMaterial = VisualStyleBuilder.GetMaterial("Forge_BossBarrier", new Color(1f, 0.26f, 0.04f), 0f, 0.54f, true);

            GameObject entryGate = CreateGate("ForgeBossEntrySeal", new Vector3(0f, 1.65f, 42.25f), new Vector3(6.4f, 3.3f, 0.40f), gateMaterial);
            GameObject closeBarrier = CreateGate("ForgeBossCloseBarrier", new Vector3(0f, 1.65f, 43.25f), new Vector3(12f, 3.3f, 0.32f), barrierMaterial);

            GameObject mission = new GameObject("MissionController");
            MissionController controller = mission.AddComponent<MissionController>();
            controller.Configure(regular.ToArray(), boss, bossHealth, progress, entryGate);

            GameObject trigger = new GameObject("BossArenaTrigger");
            trigger.transform.position = new Vector3(0f, 1.8f, 46f);
            trigger.AddComponent<BoxCollider>();
            BossArenaTrigger arenaTrigger = trigger.AddComponent<BossArenaTrigger>();
            arenaTrigger.Configure(controller, closeBarrier, new Vector3(20f, 4f, 4f));
        }

        private static Health CreateEnemy(
            string name,
            Vector3 position,
            Vector3 scale,
            int hp,
            float speed,
            int damage,
            float cooldown,
            float windup,
            Transform player)
        {
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = name;
            enemy.transform.position = position;
            enemy.transform.localScale = scale;

            Object.DestroyImmediate(enemy.GetComponent<CapsuleCollider>());

            CharacterController character = enemy.AddComponent<CharacterController>();
            character.height = 2f;
            character.radius = 0.5f;
            character.center = new Vector3(0f, 1f, 0f);

            string archetype = name.StartsWith("Scout") ? "Scout" : name.StartsWith("Raider") ? "Raider" : "Brute";
            VisualStyleBuilder.ApplyEnemyVisual(enemy, archetype);

            Health health = enemy.AddComponent<Health>();
            health.ConfigureMaxHealth(hp);

            enemy.AddComponent<KnockbackMotor>();
            enemy.AddComponent<HealthDamageReceiver>();
            enemy.AddComponent<WorldHealthBar>();

            SimpleEnemyBrain brain = enemy.AddComponent<SimpleEnemyBrain>();
            brain.Configure(speed, 14f, 1.6f, cooldown, windup, damage);
            brain.SetTarget(player);

            EnemyDropper dropper = enemy.AddComponent<EnemyDropper>();
            dropper.Configure(5, 10, 0.28f, 4, false);

            return health;
        }

        private static GameObject CreateBoss(Transform player)
        {
            GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            boss.name = "Boss_ForgeColossus";
            boss.transform.position = new Vector3(0f, 1.8f, 52f);
            boss.transform.localScale = new Vector3(2.05f, 2.05f, 2.05f);

            Object.DestroyImmediate(boss.GetComponent<CapsuleCollider>());

            CharacterController character = boss.AddComponent<CharacterController>();
            character.height = 2f;
            character.radius = 0.62f;
            character.center = new Vector3(0f, 1f, 0f);

            VisualStyleBuilder.ApplyBossVisual(boss);

            Health health = boss.AddComponent<Health>();
            health.ConfigureMaxHealth(680);

            boss.AddComponent<KnockbackMotor>();
            boss.AddComponent<HealthDamageReceiver>();
            boss.AddComponent<WorldHealthBar>();

            SimpleEnemyBrain brain = boss.AddComponent<SimpleEnemyBrain>();
            brain.Configure(2.15f, 20f, 2.1f, 1.0f, 0.52f, 31);
            brain.SetTarget(player);

            boss.AddComponent<BossPulseAttack>();

            EnemyDropper dropper = boss.AddComponent<EnemyDropper>();
            dropper.Configure(65, 85, 1f, 14, true);

            boss.SetActive(false);
            return boss;
        }

        private static void CreateEncounterZone(string name, Vector3 position, Vector3 size, params GameObject[] enemies)
        {
            GameObject zone = new GameObject(name);
            zone.transform.position = position;
            zone.AddComponent<BoxCollider>();
            EncounterZone encounter = zone.AddComponent<EncounterZone>();
            encounter.Configure(enemies, size);
        }

        private static GameObject CreateGate(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject gate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gate.name = name;
            gate.transform.position = position;
            gate.transform.localScale = scale;
            Renderer renderer = gate.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;
            return gate;
        }

        private static void CreateTreasureChest(
            Vector3 position,
            string materialPrefix,
            Color bodyColor,
            Color accentColor,
            int gold,
            int power)
        {
            Material bodyMaterial = VisualStyleBuilder.GetMaterial(materialPrefix + "_Body", bodyColor, 0.14f, 0.38f);
            Material accentMaterial = VisualStyleBuilder.GetMaterial(materialPrefix + "_Accent", accentColor, 0.48f, 0.58f, true);

            GameObject root = new GameObject("TreasureChest");
            root.transform.position = position;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localScale = new Vector3(1.45f, 0.72f, 0.95f);

            GameObject lid = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lid.name = "Lid";
            lid.transform.SetParent(root.transform, false);
            lid.transform.localPosition = new Vector3(0f, 0.58f, -0.34f);
            lid.transform.localScale = new Vector3(1.48f, 0.24f, 0.98f);

            GameObject lockPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lockPlate.name = "Lock";
            lockPlate.transform.SetParent(root.transform, false);
            lockPlate.transform.localPosition = new Vector3(0f, 0.18f, 0.51f);
            lockPlate.transform.localScale = new Vector3(0.32f, 0.42f, 0.08f);

            body.GetComponent<Renderer>().sharedMaterial = bodyMaterial;
            lid.GetComponent<Renderer>().sharedMaterial = bodyMaterial;
            lockPlate.GetComponent<Renderer>().sharedMaterial = accentMaterial;

            Object.DestroyImmediate(body.GetComponent<Collider>());
            Object.DestroyImmediate(lid.GetComponent<Collider>());
            Object.DestroyImmediate(lockPlate.GetComponent<Collider>());

            TreasureChest chest = root.AddComponent<TreasureChest>();
            chest.Configure(lid.transform, gold, power);
        }
    }
}
#endif
