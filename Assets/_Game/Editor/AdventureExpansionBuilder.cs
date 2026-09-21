#if UNITY_EDITOR
using UnityEngine;
using VoxelDungeon.Loot;
using VoxelDungeon.Player;

namespace VoxelDungeon.EditorTools
{
    public static class AdventureExpansionBuilder
    {
        public static void Decorate(string stageId)
        {
            bool forge = stageId == "stage.ashen";
            bool voidGarden = stageId == "stage.void";

            Color stoneColor = forge
                ? new Color(0.19f, 0.17f, 0.16f)
                : voidGarden
                    ? new Color(0.53f, 0.58f, 0.61f)
                    : new Color(0.15f, 0.20f, 0.23f);

            Color deepStoneColor = forge
                ? new Color(0.10f, 0.08f, 0.07f)
                : voidGarden
                    ? new Color(0.24f, 0.27f, 0.30f)
                    : new Color(0.07f, 0.10f, 0.13f);

            Color glowColor = forge
                ? new Color(1f, 0.28f, 0.04f)
                : voidGarden
                    ? new Color(0.36f, 1f, 0.62f)
                    : new Color(0.10f, 0.88f, 1f);

            Color outdoorColor = forge
                ? new Color(0.30f, 0.19f, 0.12f)
                : voidGarden
                    ? new Color(0.58f, 0.70f, 0.62f)
                    : new Color(0.28f, 0.43f, 0.42f);

            Material stone = VisualStyleBuilder.GetMaterial(
                Prefix(stageId) + "_AdventureStone",
                stoneColor,
                forge ? 0.20f : 0.05f,
                0.25f);

            Material deepStone = VisualStyleBuilder.GetMaterial(
                Prefix(stageId) + "_AdventureDeepStone",
                deepStoneColor,
                forge ? 0.28f : 0.05f,
                0.22f);

            Material glow = VisualStyleBuilder.GetMaterial(
                Prefix(stageId) + "_AdventureGlow",
                glowColor,
                0f,
                0.48f,
                true);

            Material outdoor = VisualStyleBuilder.GetMaterial(
                Prefix(stageId) + "_OutdoorGround",
                outdoorColor,
                0.02f,
                0.24f);

            CreateCaveSection("CaveEntrance", -22f, 18f, stone, deepStone, glow);
            CreateOutdoorSection(stageId, outdoor, stone, glow);
            CreateCaveSection("DeepCave", 31f, 20f, deepStone, stone, glow);
            CreateBossAntechamber(stone, glow);

            CreateCheckpoint("Checkpoint_CaveMouth", new Vector3(0f, 1.6f, -28f), "CAVE MOUTH", glow);
            CreateCheckpoint("Checkpoint_OuterRoute", new Vector3(0f, 1.6f, -4f), "OUTER ROUTE", glow);
            CreateCheckpoint("Checkpoint_DeepPassage", new Vector3(0f, 1.6f, 20f), "DEEP PASSAGE", glow);
            CreateCheckpoint("Checkpoint_Boss", new Vector3(0f, 1.6f, 40.2f), "BOSS ANTECHAMBER", glow);

            CreateUnderworld(stageId, deepStone, glow);
            CreateHarvestRoute(stageId, stone, glow);
            CreateSecretSidePath(stageId, stone, deepStone, glow);
        }

        private static void CreateCaveSection(
            string name,
            float centerZ,
            float length,
            Material wall,
            Material accent,
            Material glow)
        {
            GameObject root = new GameObject(name);

            float half = length * 0.5f;

            for (int i = 0; i < 5; i++)
            {
                float z = centerZ - half + 2f + i * ((length - 4f) / 4f);

                CreateBlock(root.transform, "Wall_L", new Vector3(-9.5f, 2.7f, z), new Vector3(3.0f, 5.4f, 4.0f), wall);
                CreateBlock(root.transform, "Wall_R", new Vector3(9.5f, 2.7f, z), new Vector3(3.0f, 5.4f, 4.0f), wall);

                // Partial ceiling shelves keep the top-down camera readable while
                // still making the route feel enclosed.
                CreateBlock(root.transform, "Ceiling_L", new Vector3(-7.2f, 6.2f, z), new Vector3(4.2f, 1.1f, 3.5f), accent, false);
                CreateBlock(root.transform, "Ceiling_R", new Vector3(7.2f, 6.2f, z), new Vector3(4.2f, 1.1f, 3.5f), accent, false);
            }

            CreateArch(root.transform, new Vector3(0f, 0f, centerZ - half), wall, glow);
            CreateArch(root.transform, new Vector3(0f, 0f, centerZ + half), wall, glow);

            for (int i = 0; i < 6; i++)
            {
                float side = i % 2 == 0 ? -1f : 1f;
                float z = centerZ - half + 2.5f + i * Mathf.Max(2.1f, (length - 5f) / 5f);
                GameObject shard = CreateBlock(
                    root.transform,
                    "CaveShard",
                    new Vector3(side * 7.1f, 1.0f + (i % 3) * 0.25f, z),
                    new Vector3(0.34f, 1.8f + (i % 2) * 0.7f, 0.34f),
                    glow,
                    false);
                shard.transform.rotation = Quaternion.Euler(0f, i * 22f, side * 15f);
            }
        }

        private static void CreateArch(Transform root, Vector3 position, Material stone, Material glow)
        {
            CreateBlock(root, "Arch_L", position + new Vector3(-5.8f, 2.7f, 0f), new Vector3(2.2f, 5.4f, 1.2f), stone);
            CreateBlock(root, "Arch_R", position + new Vector3(5.8f, 2.7f, 0f), new Vector3(2.2f, 5.4f, 1.2f), stone);
            CreateBlock(root, "ArchTop_L", position + new Vector3(-3.8f, 5.45f, 0f), new Vector3(3.2f, 0.8f, 1.2f), stone, false);
            CreateBlock(root, "ArchTop_R", position + new Vector3(3.8f, 5.45f, 0f), new Vector3(3.2f, 0.8f, 1.2f), stone, false);
            CreateBlock(root, "ArchRune", position + new Vector3(0f, 5.45f, 0f), new Vector3(1.1f, 0.34f, 1.3f), glow, false);
        }

        private static void CreateOutdoorSection(string stageId, Material ground, Material stone, Material glow)
        {
            GameObject root = new GameObject("OutdoorPassage");

            CreateBlock(root.transform, "OutdoorFloorAccent", new Vector3(0f, 0.035f, 7f), new Vector3(18f, 0.07f, 20f), ground, false);

            for (int i = 0; i < 8; i++)
            {
                float z = -1f + i * 2.7f;
                float side = i % 2 == 0 ? -1f : 1f;

                GameObject cliff = CreateBlock(
                    root.transform,
                    "Cliff",
                    new Vector3(side * (12.5f + (i % 3)), 1.7f + (i % 2) * 0.4f, z),
                    new Vector3(4.5f, 3.4f + (i % 3), 3.3f),
                    stone);
                cliff.transform.rotation = Quaternion.Euler(0f, i * 17f, side * 5f);
            }

            if (stageId == "stage.ashen")
            {
                for (int i = 0; i < 6; i++)
                {
                    CreateBlock(
                        root.transform,
                        "EmberVent",
                        new Vector3((i % 2 == 0 ? -1f : 1f) * 7.4f, 0.45f, 0f + i * 3.0f),
                        new Vector3(0.5f, 0.9f, 0.5f),
                        glow,
                        false);
                }
            }
            else if (stageId == "stage.void")
            {
                for (int i = 0; i < 8; i++)
                {
                    CreateBlock(
                        root.transform,
                        "MoonFlower",
                        new Vector3((i % 2 == 0 ? -1f : 1f) * (5.5f + (i % 3)), 0.38f, -1f + i * 2.6f),
                        new Vector3(0.22f, 0.76f, 0.22f),
                        glow,
                        false);
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    GameObject crystal = CreateBlock(
                        root.transform,
                        "OuterCrystal",
                        new Vector3((i % 2 == 0 ? -1f : 1f) * (6.1f + (i % 3)), 0.70f, i * 2.6f),
                        new Vector3(0.30f, 1.35f, 0.30f),
                        glow,
                        false);
                    crystal.transform.rotation = Quaternion.Euler(0f, i * 30f, i % 2 == 0 ? 12f : -12f);
                }
            }
        }

        private static void CreateBossAntechamber(Material stone, Material glow)
        {
            GameObject root = new GameObject("BossAntechamber");
            CreateBlock(root.transform, "Pillar_L", new Vector3(-8.5f, 2.7f, 40.5f), new Vector3(2.3f, 5.4f, 2.3f), stone);
            CreateBlock(root.transform, "Pillar_R", new Vector3(8.5f, 2.7f, 40.5f), new Vector3(2.3f, 5.4f, 2.3f), stone);
            CreateBlock(root.transform, "Rune_L", new Vector3(-6.9f, 1.1f, 40.5f), new Vector3(0.18f, 1.9f, 0.18f), glow, false);
            CreateBlock(root.transform, "Rune_R", new Vector3(6.9f, 1.1f, 40.5f), new Vector3(0.18f, 1.9f, 0.18f), glow, false);
        }

        private static void CreateCheckpoint(string name, Vector3 position, string label, Material glow)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;

            BoxCollider trigger = root.AddComponent<BoxCollider>();
            StageCheckpoint checkpoint = root.AddComponent<StageCheckpoint>();
            checkpoint.Configure(label, new Vector3(12f, 3.4f, 2.2f), new Vector3(0f, -0.65f, -1.45f));

            GameObject rune = CreateBlock(
                root.transform,
                "CheckpointRune",
                position + new Vector3(0f, -1.50f, 0f),
                new Vector3(3.2f, 0.06f, 1.1f),
                glow,
                false);

            rune.transform.localRotation = Quaternion.Euler(0f, 45f, 0f);
        }

        private static void CreateUnderworld(string stageId, Material deepStone, Material glow)
        {
            GameObject root = new GameObject("UnderworldBackdrop");

            // Visual only: no collider, so falling still reaches the recovery threshold.
            CreateBlock(root.transform, "DeepFloor", new Vector3(0f, -24f, 10f), new Vector3(60f, 1.4f, 110f), deepStone, false);

            for (int i = 0; i < 18; i++)
            {
                float x = -22f + (i % 6) * 8.8f;
                float z = -30f + (i / 6) * 35f + (i % 2) * 6f;
                float y = -15f - (i % 4) * 2.1f;

                GameObject beacon = CreateBlock(
                    root.transform,
                    "AbyssGlow",
                    new Vector3(x, y, z),
                    new Vector3(0.5f + (i % 3) * 0.18f, 2.4f + (i % 4), 0.5f + (i % 2) * 0.2f),
                    glow,
                    false);
                beacon.transform.rotation = Quaternion.Euler(i * 7f, i * 23f, i % 2 == 0 ? 12f : -12f);
            }

            Color lightColor = stageId == "stage.ashen"
                ? new Color(1f, 0.20f, 0.03f)
                : stageId == "stage.void"
                    ? new Color(0.36f, 0.80f, 0.58f)
                    : new Color(0.18f, 0.55f, 1f);

            for (int i = 0; i < 3; i++)
            {
                GameObject lightGo = new GameObject("AbyssLight");
                lightGo.transform.SetParent(root.transform, false);
                lightGo.transform.position = new Vector3((i - 1) * 13f, -14f, -8f + i * 20f);
                Light light = lightGo.AddComponent<Light>();
                light.type = LightType.Point;
                light.color = lightColor;
                light.intensity = 3.2f;
                light.range = 18f;
                light.shadows = LightShadows.None;
            }
        }

        private static void CreateHarvestRoute(string stageId, Material stone, Material glow)
        {
            string[] materials = stageId == "stage.ashen"
                ? new[] { "iron_ore", "iron_ore", "ember_core", "iron_ore" }
                : stageId == "stage.void"
                    ? new[] { "moon_bloom", "void_fragment", "moon_bloom", "void_fragment" }
                    : new[] { "crystal_shard", "crystal_shard", "ancient_relic", "crystal_shard" };

            Vector3[] positions =
            {
                new Vector3(-6.8f, 0.85f, -16f),
                new Vector3(7.2f, 0.85f, 6f),
                new Vector3(-7.0f, 0.85f, 25f),
                new Vector3(6.6f, 0.85f, 35f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject node = CreateBlock(
                    null,
                    "Harvest_" + materials[i],
                    positions[i],
                    new Vector3(0.72f, 1.35f, 0.72f),
                    i % 2 == 0 ? glow : stone,
                    false);

                HarvestNode harvest = node.AddComponent<HarvestNode>();
                harvest.Configure(materials[i], 1, i == 2 ? 3 : 2);
            }
        }

        private static void CreateSecretSidePath(string stageId, Material stone, Material deepStone, Material glow)
        {
            GameObject root = new GameObject("SecretSidePath");

            float side = stageId == "stage.ashen" ? 1f : -1f;
            CreateBlock(root.transform, "SecretBridge", new Vector3(side * 10.5f, 0.16f, 12f), new Vector3(8f, 0.32f, 2.2f), stone);
            CreateBlock(root.transform, "SecretPocket", new Vector3(side * 14.5f, 0.18f, 12f), new Vector3(6f, 0.36f, 6f), deepStone);
            CreateBlock(root.transform, "SecretMarker", new Vector3(side * 15f, 1.4f, 14f), new Vector3(0.35f, 2.7f, 0.35f), glow, false);

            GameObject cache = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cache.name = "SecretCache";
            cache.transform.position = new Vector3(side * 14.5f, 0.72f, 11.4f);
            cache.transform.localScale = Vector3.one * 0.72f;
            Collider collider = cache.GetComponent<Collider>();
            if (collider != null)
                Object.DestroyImmediate(collider);
            Renderer renderer = cache.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = glow;

            AdventureItemPickup pickup = cache.AddComponent<AdventureItemPickup>();
            string reward = stageId == "stage.ashen"
                ? "ember_core"
                : stageId == "stage.void"
                    ? "void_fragment"
                    : "ancient_relic";
            pickup.Configure(reward, 3, false);
        }

        private static string Prefix(string stageId)
        {
            if (stageId == "stage.ashen") return "Forge";
            if (stageId == "stage.void") return "Void";
            return "Crypt";
        }

        private static GameObject CreateBlock(
            Transform parent,
            string name,
            Vector3 position,
            Vector3 scale,
            Material material,
            bool collider = true)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;

            if (parent != null)
                go.transform.SetParent(parent, false);

            go.transform.position = position;
            go.transform.localScale = scale;

            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;

            if (!collider)
            {
                Collider c = go.GetComponent<Collider>();
                if (c != null)
                    Object.DestroyImmediate(c);
            }

            return go;
        }
    }
}
#endif
