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
    public static class VoidGardenSliceBuilder
    {
        public static void PopulateMission(GameObject player)
        {
            PlayerProgress progress = player.GetComponent<PlayerProgress>();
            if (progress == null)
                progress = player.AddComponent<PlayerProgress>();

            if (player.GetComponent<ProgressHudUI>() == null)
                player.AddComponent<ProgressHudUI>();

            List<Health> regular = new List<Health>();

            Health scoutA = CreateEnemy(
                "Scout_Void_A",
                new Vector3(-4f, 1f, -15f),
                new Vector3(0.78f, 0.88f, 0.78f),
                95, 4.6f, 12, 0.64f, 0.17f,
                player.transform);

            Health scoutB = CreateEnemy(
                "Scout_Void_B",
                new Vector3(4f, 1f, -10f),
                new Vector3(0.78f, 0.88f, 0.78f),
                95, 4.6f, 12, 0.64f, 0.17f,
                player.transform);

            regular.Add(scoutA);
            regular.Add(scoutB);

            Health raiderA = CreateEnemy(
                "Raider_Void_A",
                new Vector3(-5f, 1f, 0f),
                Vector3.one,
                150, 3.15f, 17, 0.82f, 0.26f,
                player.transform);

            Health bruteA = CreateEnemy(
                "Brute_Void_A",
                new Vector3(5f, 1.25f, 4f),
                new Vector3(1.42f, 1.42f, 1.42f),
                270, 1.95f, 28, 1.16f, 0.48f,
                player.transform);

            regular.Add(raiderA);
            regular.Add(bruteA);

            Health scoutC = CreateEnemy(
                "Scout_Void_C",
                new Vector3(-5f, 1f, 17f),
                new Vector3(0.80f, 0.90f, 0.80f),
                110, 4.8f, 13, 0.61f, 0.16f,
                player.transform);

            Health raiderB = CreateEnemy(
                "Raider_Void_B",
                new Vector3(5f, 1f, 21f),
                Vector3.one,
                165, 3.25f, 18, 0.80f, 0.24f,
                player.transform);

            Health bruteB = CreateEnemy(
                "Brute_Void_B",
                new Vector3(0f, 1.25f, 19f),
                new Vector3(1.42f, 1.42f, 1.42f),
                285, 2.0f, 29, 1.12f, 0.46f,
                player.transform);

            regular.Add(scoutC);
            regular.Add(raiderB);
            regular.Add(bruteB);

            Health raiderC = CreateEnemy(
                "Raider_Void_C",
                new Vector3(-5f, 1f, 33f),
                Vector3.one,
                180, 3.35f, 20, 0.76f, 0.22f,
                player.transform);

            Health bruteC = CreateEnemy(
                "Brute_Void_C",
                new Vector3(5f, 1.25f, 36f),
                new Vector3(1.46f, 1.46f, 1.46f),
                320, 2.05f, 31, 1.05f, 0.43f,
                player.transform);

            regular.Add(raiderC);
            regular.Add(bruteC);

            CreateEncounterZone(
                "VoidEncounter_MoonTerrace",
                new Vector3(0f, 1.8f, -13f),
                new Vector3(16f, 4f, 10f),
                scoutA.gameObject, scoutB.gameObject);

            CreateEncounterZone(
                "VoidEncounter_MirrorGrove",
                new Vector3(0f, 1.8f, 1f),
                new Vector3(19f, 4f, 14f),
                raiderA.gameObject, bruteA.gameObject);

            CreateEncounterZone(
                "VoidEncounter_StarShrine",
                new Vector3(0f, 1.8f, 18f),
                new Vector3(16f, 4f, 12f),
                scoutC.gameObject, raiderB.gameObject, bruteB.gameObject);

            CreateEncounterZone(
                "VoidEncounter_AstralCourt",
                new Vector3(0f, 1.8f, 34f),
                new Vector3(18f, 4f, 12f),
                raiderC.gameObject, bruteC.gameObject);

            CreateTreasureChest(
                new Vector3(12f, 0.45f, 18f),
                "VoidChest_Green",
                new Color(0.42f, 0.52f, 0.48f),
                new Color(0.20f, 0.88f, 0.56f),
                46, 14);

            CreateTreasureChest(
                new Vector3(-8f, 0.45f, 3f),
                "VoidChest_Moon",
                new Color(0.52f, 0.56f, 0.62f),
                new Color(0.62f, 0.30f, 1f),
                42, 13);

            CreateTreasureChest(
                new Vector3(-7f, 0.45f, 35f),
                "VoidChest_Star",
                new Color(0.60f, 0.62f, 0.66f),
                new Color(0.92f, 0.74f, 0.30f),
                55, 16);

            GameObject boss = CreateBoss(player.transform);
            Health bossHealth = boss.GetComponent<Health>();

            Material gateMaterial = VisualStyleBuilder.GetMaterial(
                "Void_SealedGate",
                new Color(0.58f, 0.62f, 0.62f),
                0.14f,
                0.44f);

            Material barrierMaterial = VisualStyleBuilder.GetMaterial(
                "Void_BossBarrier",
                new Color(0.62f, 0.30f, 1f),
                0f,
                0.56f,
                true);

            GameObject entryGate = CreateGate(
                "VoidBossEntrySeal",
                new Vector3(0f, 1.65f, 42.25f),
                new Vector3(6.4f, 3.3f, 0.40f),
                gateMaterial);

            GameObject closeBarrier = CreateGate(
                "VoidBossCloseBarrier",
                new Vector3(0f, 1.65f, 43.25f),
                new Vector3(12f, 3.3f, 0.32f),
                barrierMaterial);

            GameObject mission = new GameObject("MissionController");
            MissionController controller = mission.AddComponent<MissionController>();
            controller.Configure(
                regular.ToArray(),
                boss,
                bossHealth,
                progress,
                entryGate);

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

            string archetype = name.StartsWith("Scout")
                ? "Scout"
                : name.StartsWith("Raider")
                    ? "Raider"
                    : "Brute";

            VisualStyleBuilder.ApplyEnemyVisual(enemy, archetype);

            Health health = enemy.AddComponent<Health>();
            health.ConfigureMaxHealth(hp);

            enemy.AddComponent<KnockbackMotor>();
            enemy.AddComponent<HealthDamageReceiver>();
            enemy.AddComponent<WorldHealthBar>();

            SimpleEnemyBrain brain = enemy.AddComponent<SimpleEnemyBrain>();
            brain.Configure(speed, 14f, 1.65f, cooldown, windup, damage);
            brain.SetTarget(player);

            EquipmentDropper equipmentDropper = enemy.AddComponent<EquipmentDropper>();
            equipmentDropper.Configure(new[] { "moonblade", "starbow" }, 0.14f, false);

            EnemyDropper dropper = enemy.AddComponent<EnemyDropper>();
            dropper.Configure(7, 13, 0.30f, 5, false);

            return health;
        }

        private static GameObject CreateBoss(Transform player)
        {
            GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            boss.name = "Boss_AstralWarden";
            boss.transform.position = new Vector3(0f, 1.8f, 52f);
            boss.transform.localScale = new Vector3(2.1f, 2.1f, 2.1f);

            Object.DestroyImmediate(boss.GetComponent<CapsuleCollider>());

            CharacterController character = boss.AddComponent<CharacterController>();
            character.height = 2f;
            character.radius = 0.64f;
            character.center = new Vector3(0f, 1f, 0f);

            VisualStyleBuilder.ApplyBossVisual(boss);

            Health health = boss.AddComponent<Health>();
            health.ConfigureMaxHealth(820);

            boss.AddComponent<KnockbackMotor>();
            boss.AddComponent<HealthDamageReceiver>();
            boss.AddComponent<WorldHealthBar>();

            SimpleEnemyBrain brain = boss.AddComponent<SimpleEnemyBrain>();
            brain.Configure(2.30f, 21f, 2.15f, 0.90f, 0.48f, 34);
            brain.SetTarget(player);

            BossPulseAttack pulse = boss.AddComponent<BossPulseAttack>();
            pulse.Configure(4.8f, 3.8f, 0.62f, 29);

            BossPhaseController phases = boss.AddComponent<BossPhaseController>();
            phases.Configure(false);

            EquipmentDropper equipmentDropper = boss.AddComponent<EquipmentDropper>();
            equipmentDropper.Configure(new[] { "void_edge" }, 1f, true);

            EnemyDropper dropper = boss.AddComponent<EnemyDropper>();
            dropper.Configure(90, 120, 1f, 18, true);

            boss.SetActive(false);
            return boss;
        }

        private static void CreateEncounterZone(
            string name,
            Vector3 position,
            Vector3 size,
            params GameObject[] enemies)
        {
            GameObject zone = new GameObject(name);
            zone.transform.position = position;
            zone.AddComponent<BoxCollider>();
            EncounterZone encounter = zone.AddComponent<EncounterZone>();
            encounter.Configure(enemies, size);
        }

        private static GameObject CreateGate(
            string name,
            Vector3 position,
            Vector3 scale,
            Material material)
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
            Material bodyMaterial = VisualStyleBuilder.GetMaterial(
                materialPrefix + "_Body",
                bodyColor,
                0.10f,
                0.40f);

            Material accentMaterial = VisualStyleBuilder.GetMaterial(
                materialPrefix + "_Accent",
                accentColor,
                0.40f,
                0.60f,
                true);

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
