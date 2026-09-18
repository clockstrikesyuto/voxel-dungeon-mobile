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

            regular.Add(CreateEnemy(
                "Scout_A",
                new Vector3(5.5f, 1f, 2.5f),
                new Vector3(0.72f, 0.82f, 0.72f),
                new Color(0.2f, 0.85f, 0.95f),
                55, 4.1f, 7, 0.75f, 0.22f,
                player.transform));

            regular.Add(CreateEnemy(
                "Scout_B",
                new Vector3(-5.5f, 1f, 2.5f),
                new Vector3(0.72f, 0.82f, 0.72f),
                new Color(0.2f, 0.85f, 0.95f),
                55, 4.1f, 7, 0.75f, 0.22f,
                player.transform));

            regular.Add(CreateEnemy(
                "Raider_A",
                new Vector3(4f, 1f, -4f),
                Vector3.one,
                new Color(1f, 0.48f, 0.16f),
                95, 2.65f, 11, 1.0f, 0.33f,
                player.transform));

            regular.Add(CreateEnemy(
                "Raider_B",
                new Vector3(-4f, 1f, -4f),
                Vector3.one,
                new Color(1f, 0.48f, 0.16f),
                95, 2.65f, 11, 1.0f, 0.33f,
                player.transform));

            regular.Add(CreateEnemy(
                "Brute_A",
                new Vector3(7f, 1.25f, -1.5f),
                new Vector3(1.35f, 1.35f, 1.35f),
                new Color(0.62f, 0.24f, 0.88f),
                170, 1.65f, 20, 1.4f, 0.62f,
                player.transform));

            regular.Add(CreateEnemy(
                "Brute_B",
                new Vector3(-7f, 1.25f, -1.5f),
                new Vector3(1.35f, 1.35f, 1.35f),
                new Color(0.62f, 0.24f, 0.88f),
                170, 1.65f, 20, 1.4f, 0.62f,
                player.transform));

            CreateTreasureChest(
                new Vector3(-7.5f, 0.45f, 7f),
                new Color(0.35f, 0.18f, 0.06f),
                18,
                7);

            CreateTreasureChest(
                new Vector3(7.5f, 0.45f, 7f),
                new Color(0.15f, 0.26f, 0.42f),
                24,
                9);

            GameObject boss = CreateBoss(player.transform);
            Health bossHealth = boss.GetComponent<Health>();

            GameObject mission = new GameObject("MissionController");
            MissionController controller = mission.AddComponent<MissionController>();
            controller.Configure(
                regular.ToArray(),
                boss,
                bossHealth,
                progress);
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

            Renderer renderer = enemy.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = color;

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

            EnemyDropper dropper = enemy.AddComponent<EnemyDropper>();
            dropper.Configure(2, 7, 0.22f, 3, false);

            return health;
        }

        private static GameObject CreateBoss(Transform player)
        {
            GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            boss.name = "Boss_StoneWarden";
            boss.transform.position = new Vector3(0f, 1.8f, 8f);
            boss.transform.localScale = new Vector3(1.75f, 1.75f, 1.75f);

            Object.DestroyImmediate(boss.GetComponent<CapsuleCollider>());

            CharacterController character = boss.AddComponent<CharacterController>();
            character.height = 2f;
            character.radius = 0.55f;
            character.center = new Vector3(0f, 1f, 0f);

            Renderer renderer = boss.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = new Color(0.18f, 0.08f, 0.08f);

            VisualStyleBuilder.ApplyBossVisual(boss);

            Health health = boss.AddComponent<Health>();
            health.ConfigureMaxHealth(460);

            boss.AddComponent<KnockbackMotor>();
            boss.AddComponent<HealthDamageReceiver>();
            boss.AddComponent<WorldHealthBar>();

            SimpleEnemyBrain brain = boss.AddComponent<SimpleEnemyBrain>();
            brain.Configure(2.0f, 18f, 1.9f, 1.15f, 0.58f, 24);
            brain.SetTarget(player);

            boss.AddComponent<BossPulseAttack>();

            EnemyDropper dropper = boss.AddComponent<EnemyDropper>();
            dropper.Configure(35, 50, 1f, 9, true);

            boss.SetActive(false);
            return boss;
        }

        private static void CreateTreasureChest(
            Vector3 position,
            Color color,
            int gold,
            int power)
        {
            GameObject root = new GameObject("TreasureChest");
            root.transform.position = position;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(1.35f, 0.65f, 0.9f);

            GameObject lid = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lid.name = "Lid";
            lid.transform.SetParent(root.transform, false);
            lid.transform.localPosition = new Vector3(0f, 0.55f, -0.32f);
            lid.transform.localScale = new Vector3(1.38f, 0.22f, 0.92f);

            Renderer bodyRenderer = body.GetComponent<Renderer>();
            Renderer lidRenderer = lid.GetComponent<Renderer>();
            if (bodyRenderer != null) bodyRenderer.material.color = color;
            if (lidRenderer != null) lidRenderer.material.color = color * 1.25f;

            Collider bodyCollider = body.GetComponent<Collider>();
            Collider lidCollider = lid.GetComponent<Collider>();
            if (bodyCollider != null) Object.DestroyImmediate(bodyCollider);
            if (lidCollider != null) Object.DestroyImmediate(lidCollider);

            TreasureChest chest = root.AddComponent<TreasureChest>();
            chest.Configure(lid.transform, gold, power);
        }
    }
}
#endif
