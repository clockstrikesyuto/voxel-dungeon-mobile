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
    public static class VerticalSliceBuilder
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
                "Scout_A", new Vector3(3.2f, 1f, -15f),
                new Vector3(0.72f, 0.82f, 0.72f), new Color(0.2f, 0.85f, 0.95f),
                55, 4.1f, 7, 0.75f, 0.22f, player.transform);
            Health scoutB = CreateEnemy(
                "Scout_B", new Vector3(-4.2f, 1f, -10f),
                new Vector3(0.72f, 0.82f, 0.72f), new Color(0.2f, 0.85f, 0.95f),
                55, 4.1f, 7, 0.75f, 0.22f, player.transform);
            regular.Add(scoutA);
            regular.Add(scoutB);

            Health raiderA = CreateEnemy(
                "Raider_A", new Vector3(-5.0f, 1f, 1f),
                Vector3.one, new Color(1f, 0.48f, 0.16f),
                95, 2.65f, 11, 1.0f, 0.33f, player.transform);
            Health raiderB = CreateEnemy(
                "Raider_B", new Vector3(5.0f, 1f, 5f),
                Vector3.one, new Color(1f, 0.48f, 0.16f),
                95, 2.65f, 11, 1.0f, 0.33f, player.transform);
            regular.Add(raiderA);
            regular.Add(raiderB);

            Health scoutC = CreateEnemy(
                "Scout_C", new Vector3(-5.0f, 1f, 17f),
                new Vector3(0.72f, 0.82f, 0.72f), new Color(0.2f, 0.85f, 0.95f),
                65, 4.25f, 8, 0.72f, 0.20f, player.transform);
            Health bruteA = CreateEnemy(
                "Brute_A", new Vector3(4.0f, 1.25f, 21f),
                new Vector3(1.35f, 1.35f, 1.35f), new Color(0.62f, 0.24f, 0.88f),
                170, 1.65f, 20, 1.4f, 0.62f, player.transform);
            regular.Add(scoutC);
            regular.Add(bruteA);

            Health raiderC = CreateEnemy(
                "Raider_C", new Vector3(-4.2f, 1f, 33f),
                Vector3.one, new Color(1f, 0.48f, 0.16f),
                110, 2.75f, 13, 0.95f, 0.31f, player.transform);
            Health bruteB = CreateEnemy(
                "Brute_B", new Vector3(5.0f, 1.25f, 36f),
                new Vector3(1.35f, 1.35f, 1.35f), new Color(0.62f, 0.24f, 0.88f),
                190, 1.7f, 22, 1.35f, 0.58f, player.transform);
            regular.Add(raiderC);
            regular.Add(bruteB);

            CreateEncounterZone(
                "Encounter_CrystalHall",
                new Vector3(0f, 1.8f, -13f),
                new Vector3(14f, 4f, 10f),
                scoutA.gameObject, scoutB.gameObject);

            CreateEncounterZone(
                "Encounter_Crossing",
                new Vector3(0f, 1.8f, 1f),
                new Vector3(19f, 4f, 14f),
                raiderA.gameObject, raiderB.gameObject);

            CreateEncounterZone(
                "Encounter_Gallery",
                new Vector3(0f, 1.8f, 18f),
                new Vector3(16f, 4f, 12f),
                scoutC.gameObject, bruteA.gameObject);

            CreateEncounterZone(
                "Encounter_Sanctum",
                new Vector3(0f, 1.8f, 34f),
                new Vector3(18f, 4f, 12f),
                raiderC.gameObject, bruteB.gameObject);

            CreateTreasureChest(
                new Vector3(-13f, 0.45f, 1f),
                "CryptChest_Cyan",
                new Color(0.28f, 0.42f, 0.46f),
                new Color(0.12f, 0.88f, 1f),
                18, 7);

            CreateTreasureChest(
                new Vector3(12f, 0.45f, 18f),
                "CryptChest_Blue",
                new Color(0.26f, 0.34f, 0.50f),
                new Color(0.18f, 0.50f, 1f),
                24, 9);

            CreateTreasureChest(
                new Vector3(-7f, 0.45f, 34f),
                "CryptChest_Violet",
                new Color(0.32f, 0.25f, 0.44f),
                new Color(0.63f, 0.30f, 1f),
                30, 11);

            GameObject boss = CreateBoss(player.transform);
            Health bossHealth = boss.GetComponent<Health>();

            Material gateMaterial = VisualStyleBuilder.GetMaterial(
                "Crypt_SealedGate",
                new Color(0.25f, 0.30f, 0.34f),
                0.28f,
                0.40f);

            Material barrierMaterial = VisualStyleBuilder.GetMaterial(
                "Crypt_BossBarrier",
                new Color(0.62f, 0.28f, 1f),
                0.0f,
                0.48f,
                true);

            GameObject entryGate = CreateGate(
                "BossEntrySeal",
                new Vector3(0f, 1.65f, 42.25f),
                new Vector3(6.4f, 3.3f, 0.40f),
                gateMaterial);

            GameObject closeBarrier = CreateGate(
                "BossCloseBarrier",
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
            BoxCollider triggerCollider = trigger.AddComponent<BoxCollider>();
            BossArenaTrigger arenaTrigger = trigger.AddComponent<BossArenaTrigger>();
            arenaTrigger.Configure(controller, closeBarrier, new Vector3(20f, 4f, 4f));
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

        private static Health CreateEnemy(
            string name,
            Vector3 position,
            Vector3 scale,
            Color color,
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
            brain.Configure(speed, 14f, 1.55f, cooldown, windup, damage);
            brain.SetTarget(player);

            EquipmentDropper equipmentDropper = enemy.AddComponent<EquipmentDropper>();
            equipmentDropper.Configure(new[] { "crystal_saber", "crystal_bow" }, 0.10f, false);

            EnemyDropper dropper = enemy.AddComponent<EnemyDropper>();
            dropper.Configure(3, 8, 0.24f, 3, false);

            return health;
        }

        private static GameObject CreateBoss(Transform player)
        {
            GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            boss.name = "Boss_StoneWarden";
            boss.transform.position = new Vector3(0f, 1.8f, 52f);
            boss.transform.localScale = new Vector3(1.9f, 1.9f, 1.9f);

            Object.DestroyImmediate(boss.GetComponent<CapsuleCollider>());

            CharacterController character = boss.AddComponent<CharacterController>();
            character.height = 2f;
            character.radius = 0.60f;
            character.center = new Vector3(0f, 1f, 0f);

            VisualStyleBuilder.ApplyBossVisual(boss);

            Health health = boss.AddComponent<Health>();
            health.ConfigureMaxHealth(520);

            boss.AddComponent<KnockbackMotor>();
            boss.AddComponent<HealthDamageReceiver>();
            boss.AddComponent<WorldHealthBar>();

            SimpleEnemyBrain brain = boss.AddComponent<SimpleEnemyBrain>();
            brain.Configure(2.05f, 20f, 2.0f, 1.10f, 0.58f, 26);
            brain.SetTarget(player);

            boss.AddComponent<BossPulseAttack>();
            BossPhaseController phaseController = boss.AddComponent<BossPhaseController>();
            phaseController.Configure(false);

            EquipmentDropper equipmentDropper = boss.AddComponent<EquipmentDropper>();
            equipmentDropper.Configure(new[] { "warden_cleaver" }, 1f, true);

            EnemyDropper dropper = boss.AddComponent<EnemyDropper>();
            dropper.Configure(45, 60, 1f, 10, true);

            boss.SetActive(false);
            return boss;
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
                0.05f,
                0.32f);

            Material accentMaterial = VisualStyleBuilder.GetMaterial(
                materialPrefix + "_Accent",
                accentColor,
                0.45f,
                0.55f,
                true);

            GameObject root = new GameObject("TreasureChest");
            root.transform.position = position;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = Vector3.zero;
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

            Renderer bodyRenderer = body.GetComponent<Renderer>();
            Renderer lidRenderer = lid.GetComponent<Renderer>();
            Renderer lockRenderer = lockPlate.GetComponent<Renderer>();
            if (bodyRenderer != null) bodyRenderer.sharedMaterial = bodyMaterial;
            if (lidRenderer != null) lidRenderer.sharedMaterial = bodyMaterial;
            if (lockRenderer != null) lockRenderer.sharedMaterial = accentMaterial;

            Collider bodyCollider = body.GetComponent<Collider>();
            Collider lidCollider = lid.GetComponent<Collider>();
            Collider lockCollider = lockPlate.GetComponent<Collider>();
            if (bodyCollider != null) Object.DestroyImmediate(bodyCollider);
            if (lidCollider != null) Object.DestroyImmediate(lidCollider);
            if (lockCollider != null) Object.DestroyImmediate(lockCollider);

            TreasureChest chest = root.AddComponent<TreasureChest>();
            chest.Configure(lid.transform, gold, power);
        }
    }
}
#endif
