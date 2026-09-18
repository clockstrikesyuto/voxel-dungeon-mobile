#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;
using VoxelDungeon.Core;

namespace VoxelDungeon.EditorTools
{
    public static class HubWorldBuilder
    {
        public static void PopulateHub(GameObject player)
        {
            Material grass = VisualStyleBuilder.GetMaterial("Hub_Grass", new Color(0.24f, 0.48f, 0.27f), 0f, 0.20f);
            Material grassLight = VisualStyleBuilder.GetMaterial("Hub_GrassLight", new Color(0.34f, 0.60f, 0.33f), 0f, 0.18f);
            Material plaza = VisualStyleBuilder.GetMaterial("Hub_Plaza", new Color(0.62f, 0.68f, 0.68f), 0.05f, 0.40f);
            Material path = VisualStyleBuilder.GetMaterial("Hub_Path", new Color(0.55f, 0.50f, 0.40f), 0.02f, 0.30f);
            Material stone = VisualStyleBuilder.GetMaterial("Hub_Stone", new Color(0.56f, 0.61f, 0.62f), 0.08f, 0.28f);
            Material paleStone = VisualStyleBuilder.GetMaterial("Hub_PaleStone", new Color(0.72f, 0.77f, 0.76f), 0.05f, 0.32f);
            Material darkStone = VisualStyleBuilder.GetMaterial("Hub_DarkStone", new Color(0.20f, 0.24f, 0.27f), 0.10f, 0.28f);
            Material wood = VisualStyleBuilder.GetMaterial("Hub_Wood", new Color(0.43f, 0.23f, 0.11f), 0f, 0.24f);
            Material woodLight = VisualStyleBuilder.GetMaterial("Hub_WoodLight", new Color(0.58f, 0.34f, 0.16f), 0f, 0.24f);
            Material leaf = VisualStyleBuilder.GetMaterial("Hub_Leaves", new Color(0.18f, 0.46f, 0.23f), 0f, 0.18f);
            Material leafLight = VisualStyleBuilder.GetMaterial("Hub_LeavesLight", new Color(0.30f, 0.62f, 0.29f), 0f, 0.18f);
            Material water = VisualStyleBuilder.GetMaterial("Hub_Water", new Color(0.18f, 0.66f, 0.84f), 0.05f, 0.78f, true);
            Material cyan = VisualStyleBuilder.GetMaterial("Hub_Cyan", new Color(0.10f, 0.86f, 0.96f), 0f, 0.52f, true);
            Material orange = VisualStyleBuilder.GetMaterial("Hub_Orange", new Color(1f, 0.40f, 0.10f), 0f, 0.48f, true);
            Material violet = VisualStyleBuilder.GetMaterial("Hub_Violet", new Color(0.62f, 0.34f, 1f), 0f, 0.50f, true);
            Material gold = VisualStyleBuilder.GetMaterial("Hub_Gold", new Color(0.94f, 0.68f, 0.20f), 0.55f, 0.52f);
            Material flowerPink = VisualStyleBuilder.GetMaterial("Hub_FlowerPink", new Color(1f, 0.54f, 0.72f), 0f, 0.20f, true);
            Material flowerYellow = VisualStyleBuilder.GetMaterial("Hub_FlowerYellow", new Color(1f, 0.82f, 0.24f), 0f, 0.20f, true);

            GameObject ground = GameObject.Find("Hub Ground");
            if (ground != null)
            {
                Renderer renderer = ground.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = grass;
            }

            CreateBlock("CentralPlaza", new Vector3(0f, 0.03f, 0f), new Vector3(18f, 0.06f, 18f), plaza, false);
            CreatePath(new Vector3(0f, 0.05f, 13f), new Vector3(6f, 0.08f, 18f), path);
            CreatePath(new Vector3(-10f, 0.05f, 11f), new Vector3(14f, 0.08f, 4f), path);
            CreatePath(new Vector3(10f, 0.05f, 11f), new Vector3(14f, 0.08f, 4f), path);
            CreatePath(new Vector3(-8f, 0.05f, -7f), new Vector3(12f, 0.08f, 4f), path);
            CreatePath(new Vector3(8f, 0.05f, -7f), new Vector3(12f, 0.08f, 4f), path);
            CreatePath(new Vector3(0f, 0.05f, -12f), new Vector3(5f, 0.08f, 12f), path);

            CreatePond(new Vector3(18f, 0.0f, -10f), water, paleStone);
            CreateBridge(new Vector3(13.0f, 0.45f, -10f), wood, paleStone);

            CreateFountain(cyan, paleStone, water);
            CreateWorldMapTable(new Vector3(0f, 0f, 6.0f), paleStone, cyan, gold);
            CreateForge(new Vector3(-10f, 0f, -6f), paleStone, darkStone, orange, wood, woodLight);
            CreateMerchant(new Vector3(10f, 0f, -6f), paleStone, wood, woodLight, gold);
            CreateArmory(new Vector3(-17f, 0f, 4f), paleStone, darkStone, stone, cyan, wood);
            CreateArchivist(new Vector3(17f, 0f, 4f), paleStone, violet, gold, wood);
            CreateTraining(new Vector3(0f, 0f, -15f), paleStone, wood, orange);

            CreateStageGate(
                "Gate_CrystalCrypt",
                new Vector3(-12f, 0f, 20f),
                cyan,
                paleStone,
                "CRYSTAL CRYPT",
                "Enter the first expedition",
                "stage.crypt");

            CreateStageGate(
                "Gate_AshenForge",
                new Vector3(0f, 0f, 23f),
                orange,
                stone,
                "ASHEN FORGE",
                "Unlock by clearing Crystal Crypt",
                "stage.ashen");

            CreateStageGate(
                "Gate_VoidGarden",
                new Vector3(12f, 0f, 20f),
                violet,
                stone,
                "VOID GARDEN",
                "The distant frontier remains sealed",
                "stage.void");

            CreateTrees(wood, leaf, leafLight);
            CreateFlowerBeds(grassLight, flowerPink, flowerYellow, cyan);
            CreateLamps(paleStone, gold);
            CreateScenery(stone, grassLight, cyan, violet);
            CreateDistantMountains(stone, grass);

            GameObject controller = new GameObject("HubController");
            HubController hub = controller.AddComponent<HubController>();
            hub.SetPlayer(player.transform);

            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.62f, 0.80f, 0.88f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0045f;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.66f, 0.80f, 0.90f);
            RenderSettings.ambientEquatorColor = new Color(0.48f, 0.58f, 0.53f);
            RenderSettings.ambientGroundColor = new Color(0.24f, 0.28f, 0.22f);

            Light key = Object.FindFirstObjectByType<Light>();
            if (key != null && key.type == LightType.Directional)
            {
                key.color = new Color(1f, 0.91f, 0.75f);
                key.intensity = 1.45f;
                key.shadows = LightShadows.Soft;
                key.transform.rotation = Quaternion.Euler(42f, -28f, 0f);
            }

            WorldPostProcessingBuilder.ApplyHub();
        }

        private static void CreatePond(Vector3 center, Material water, Material stone)
        {
            CreateBlock("PondWater", center + new Vector3(0f, 0.04f, 0f), new Vector3(10f, 0.08f, 12f), water, false);

            CreateBlock("PondBank_N", center + new Vector3(0f, 0.25f, 6.2f), new Vector3(10.8f, 0.5f, 0.55f), stone);
            CreateBlock("PondBank_S", center + new Vector3(0f, 0.25f, -6.2f), new Vector3(10.8f, 0.5f, 0.55f), stone);
            CreateBlock("PondBank_E", center + new Vector3(5.2f, 0.25f, 0f), new Vector3(0.55f, 0.5f, 12f), stone);
            CreateBlock("PondBank_W", center + new Vector3(-5.2f, 0.25f, 0f), new Vector3(0.55f, 0.5f, 12f), stone);
        }

        private static void CreateBridge(Vector3 center, Material wood, Material stone)
        {
            for (int i = -3; i <= 3; i++)
            {
                CreateBlock(
                    "BridgePlank",
                    center + new Vector3(i * 0.72f, 0f, 0f),
                    new Vector3(0.62f, 0.18f, 3.2f),
                    wood);
            }

            CreateBlock("BridgeRail_L", center + new Vector3(-2.65f, 0.65f, 0f), new Vector3(0.18f, 1.0f, 3.4f), stone);
            CreateBlock("BridgeRail_R", center + new Vector3(2.65f, 0.65f, 0f), new Vector3(0.18f, 1.0f, 3.4f), stone);
        }

        private static void CreateFountain(Material glow, Material stone, Material water)
        {
            CreateBlock("FountainBase", new Vector3(0f, 0.30f, 0f), new Vector3(4.2f, 0.6f, 4.2f), stone);
            CreateBlock("FountainWater", new Vector3(0f, 0.64f, 0f), new Vector3(3.4f, 0.08f, 3.4f), water, false);
            CreateBlock("FountainPillar", new Vector3(0f, 1.3f, 0f), new Vector3(0.72f, 2.0f, 0.72f), stone);
            CreateCrystal(new Vector3(0f, 2.70f, 0f), new Vector3(0.62f, 1.55f, 0.62f), glow);

            GameObject lightGo = new GameObject("FountainGlow");
            lightGo.transform.position = new Vector3(0f, 2.2f, 0f);
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.2f, 0.9f, 1f);
            light.intensity = 2.1f;
            light.range = 8f;
            light.shadows = LightShadows.None;
        }

        private static void CreateWorldMapTable(Vector3 position, Material stone, Material glow, Material gold)
        {
            GameObject root = new GameObject("WorldMapTable");
            root.transform.position = position;

            CreateBlockChild(root.transform, "Base", new Vector3(0f, 0.55f, 0f), new Vector3(2.9f, 1.1f, 2.1f), stone);
            CreateBlockChild(root.transform, "MapSurface", new Vector3(0f, 1.16f, 0f), new Vector3(2.65f, 0.10f, 1.85f), glow);
            CreateBlockChild(root.transform, "Frame_N", new Vector3(0f, 1.25f, 0.97f), new Vector3(2.9f, 0.18f, 0.18f), gold);
            CreateBlockChild(root.transform, "Frame_S", new Vector3(0f, 1.25f, -0.97f), new Vector3(2.9f, 0.18f, 0.18f), gold);

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(HubPointKind.WorldMap, "FRONTIER MAP", "View all known regions and routes", radius: 2.8f);
            CreateWorldLabel(root.transform, "WORLD MAP", new Vector3(0f, 2.15f, 0f), Color.white, 42);
        }

        private static void CreateForge(Vector3 position, Material stone, Material dark, Material glow, Material wood, Material woodLight)
        {
            GameObject root = new GameObject("EmberForge");
            root.transform.position = position;

            CreateBlockChild(root.transform, "Floor", new Vector3(0f, 0.12f, 0f), new Vector3(7f, 0.24f, 5.8f), stone);
            CreateBlockChild(root.transform, "BackWall", new Vector3(0f, 2.15f, 2.25f), new Vector3(7f, 4.3f, 0.45f), dark);
            CreateBlockChild(root.transform, "Beam_L", new Vector3(-2.8f, 2.2f, 0.5f), new Vector3(0.35f, 4.4f, 0.35f), woodLight);
            CreateBlockChild(root.transform, "Beam_R", new Vector3(2.8f, 2.2f, 0.5f), new Vector3(0.35f, 4.4f, 0.35f), woodLight);
            CreateBlockChild(root.transform, "Roof", new Vector3(0f, 4.2f, 0.7f), new Vector3(7.4f, 0.35f, 3.8f), wood);
            CreateBlockChild(root.transform, "Anvil", new Vector3(0f, 0.75f, -0.6f), new Vector3(1.5f, 0.65f, 0.8f), stone);
            CreateBlockChild(root.transform, "ForgeFire", new Vector3(-2.1f, 1.15f, 1.7f), new Vector3(1.25f, 1.8f, 0.3f), glow, false);
            CreateBlockChild(root.transform, "WorkBench", new Vector3(2.0f, 0.75f, 1.25f), new Vector3(2.0f, 0.55f, 0.9f), woodLight);

            GameObject lightGo = new GameObject("ForgeLight");
            lightGo.transform.SetParent(root.transform, false);
            lightGo.transform.localPosition = new Vector3(-2.1f, 1.8f, 0.3f);
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.40f, 0.08f);
            light.intensity = 3.2f;
            light.range = 7f;

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(HubPointKind.Forge, "EMBER FORGE", "Spend gold on permanent weapon upgrades", radius: 3.6f);
            CreateWorldLabel(root.transform, "EMBER FORGE", new Vector3(0f, 5.0f, 1.8f), new Color(1f, 0.5f, 0.15f), 34);
        }

        private static void CreateMerchant(Vector3 position, Material stone, Material wood, Material woodLight, Material gold)
        {
            GameObject root = new GameObject("FrontierMerchant");
            root.transform.position = position;

            CreateBlockChild(root.transform, "Platform", new Vector3(0f, 0.14f, 0f), new Vector3(7f, 0.28f, 5.8f), stone);
            CreateBlockChild(root.transform, "Counter", new Vector3(0f, 1.0f, -1.0f), new Vector3(5.2f, 1.2f, 0.75f), wood);
            CreateBlockChild(root.transform, "Awning", new Vector3(0f, 3.5f, 0.2f), new Vector3(6.5f, 0.28f, 3.7f), gold);
            CreateBlockChild(root.transform, "Post_L", new Vector3(-2.7f, 1.8f, 0.4f), new Vector3(0.28f, 3.6f, 0.28f), woodLight);
            CreateBlockChild(root.transform, "Post_R", new Vector3(2.7f, 1.8f, 0.4f), new Vector3(0.28f, 3.6f, 0.28f), woodLight);
            CreateBlockChild(root.transform, "Crate_L", new Vector3(-2.0f, 0.65f, 1.2f), new Vector3(1.1f, 1.1f, 1.1f), wood);
            CreateBlockChild(root.transform, "Crate_R", new Vector3(2.0f, 0.65f, 1.2f), new Vector3(1.1f, 1.1f, 1.1f), wood);

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(HubPointKind.Merchant, "FRONTIER MERCHANT", "Browse supplies and rotating loot", radius: 3.6f);
            CreateWorldLabel(root.transform, "MERCHANT", new Vector3(0f, 4.45f, 0.5f), new Color(1f, 0.75f, 0.18f), 34);
        }

        private static void CreateArmory(
            Vector3 position,
            Material stone,
            Material dark,
            Material steel,
            Material glow,
            Material wood)
        {
            GameObject root = new GameObject("FrontierArmory");
            root.transform.position = position;

            CreateBlockChild(root.transform, "Platform", new Vector3(0f, 0.12f, 0f), new Vector3(6.5f, 0.24f, 5.5f), stone);
            CreateBlockChild(root.transform, "BackWall", new Vector3(0f, 2.0f, 2.0f), new Vector3(6.5f, 4.0f, 0.40f), dark);
            CreateBlockChild(root.transform, "WeaponRack", new Vector3(0f, 1.35f, 1.55f), new Vector3(4.8f, 2.2f, 0.25f), wood);
            CreateBlockChild(root.transform, "Blade_L", new Vector3(-1.6f, 1.55f, 1.35f), new Vector3(0.16f, 1.8f, 0.12f), steel, false);
            CreateBlockChild(root.transform, "Blade_C", new Vector3(0f, 1.55f, 1.35f), new Vector3(0.16f, 1.8f, 0.12f), glow, false);
            CreateBlockChild(root.transform, "Blade_R", new Vector3(1.6f, 1.55f, 1.35f), new Vector3(0.16f, 1.8f, 0.12f), steel, false);

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(HubPointKind.Armory, "FRONTIER ARSENAL", "Equip weapons collected on expeditions", radius: 3.5f);

            CreateWorldLabel(root.transform, "ARSENAL", new Vector3(0f, 4.6f, 1.7f), glow.color, 34);
        }

        private static void CreateArchivist(
            Vector3 position,
            Material stone,
            Material glow,
            Material gold,
            Material wood)
        {
            GameObject root = new GameObject("ArchivistLuma");
            root.transform.position = position;

            CreateBlockChild(root.transform, "ResearchFloor", new Vector3(0f, 0.10f, 0f), new Vector3(5.8f, 0.20f, 5.0f), stone);
            CreateBlockChild(root.transform, "Desk", new Vector3(0f, 0.85f, 0.8f), new Vector3(3.6f, 1.0f, 1.0f), wood);
            CreateBlockChild(root.transform, "MapCrystal", new Vector3(0f, 1.65f, 0.75f), new Vector3(0.55f, 1.1f, 0.55f), glow, false);
            CreateBlockChild(root.transform, "Book_L", new Vector3(-1.0f, 1.45f, 0.55f), new Vector3(0.75f, 0.12f, 0.55f), gold, false);
            CreateBlockChild(root.transform, "Book_R", new Vector3(1.0f, 1.45f, 0.55f), new Vector3(0.75f, 0.12f, 0.55f), gold, false);

            GameObject npc = new GameObject("Luma");
            npc.transform.SetParent(root.transform, false);
            CreateBlockChild(npc.transform, "Body", new Vector3(0f, 1.0f, -0.8f), new Vector3(0.65f, 1.2f, 0.50f), glow, false);
            CreateBlockChild(npc.transform, "Head", new Vector3(0f, 1.85f, -0.8f), new Vector3(0.58f, 0.58f, 0.58f), gold, false);

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(HubPointKind.Quest, "ARCHIVIST LUMA", "Frontier research quest", radius: 3.2f);

            CreateWorldLabel(root.transform, "ARCHIVIST", new Vector3(0f, 4.35f, 0.8f), glow.color, 32);
        }

        private static void CreateTraining(Vector3 position, Material stone, Material wood, Material glow)
        {
            GameObject root = new GameObject("TrainingYard");
            root.transform.position = position;

            CreateBlockChild(root.transform, "Floor", new Vector3(0f, 0.1f, 0f), new Vector3(11f, 0.2f, 7.5f), stone);
            CreateBlockChild(root.transform, "DummyA", new Vector3(-2.5f, 1.1f, 0.5f), new Vector3(0.7f, 2.2f, 0.7f), wood);
            CreateBlockChild(root.transform, "DummyB", new Vector3(0f, 1.1f, 1.1f), new Vector3(0.7f, 2.2f, 0.7f), wood);
            CreateBlockChild(root.transform, "DummyC", new Vector3(2.5f, 1.1f, 0.5f), new Vector3(0.7f, 2.2f, 0.7f), wood);
            CreateBlockChild(root.transform, "Banner", new Vector3(0f, 2.8f, 3.0f), new Vector3(5.4f, 1.0f, 0.18f), glow, false);

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(HubPointKind.Training, "TRAINING YARD", "Practice movement and weapon handling", radius: 4.2f);
            CreateWorldLabel(root.transform, "TRAINING", new Vector3(0f, 4.2f, 2.7f), new Color(1f, 0.48f, 0.14f), 34);
        }

        private static void CreateStageGate(string name, Vector3 position, Material glow, Material stone, string stageName, string subtitle, string stageId)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;

            CreateBlockChild(root.transform, "Pillar_L", new Vector3(-2.3f, 2.8f, 0f), new Vector3(1.2f, 5.6f, 1.2f), stone);
            CreateBlockChild(root.transform, "Pillar_R", new Vector3(2.3f, 2.8f, 0f), new Vector3(1.2f, 5.6f, 1.2f), stone);
            CreateBlockChild(root.transform, "Lintel", new Vector3(0f, 5.35f, 0f), new Vector3(5.8f, 1.0f, 1.2f), stone);
            CreateBlockChild(root.transform, "Portal", new Vector3(0f, 2.85f, 0.05f), new Vector3(3.3f, 4.1f, 0.16f), glow, false);
            CreateBlockChild(root.transform, "Step", new Vector3(0f, 0.22f, -1.1f), new Vector3(6.0f, 0.44f, 2.4f), stone);

            string sceneName = stageId switch
            {
                "stage.ashen" => "Mission_Ashen",
                "stage.void" => "Mission_Void",
                _ => "Mission_Test"
            };

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(HubPointKind.StageGate, stageName, subtitle, stageId, sceneName, 3.8f);

            CreateWorldLabel(root.transform, stageName, new Vector3(0f, 6.55f, 0f), glow.color, 32);

            GameObject lightGo = new GameObject(name + "_Light");
            lightGo.transform.SetParent(root.transform, false);
            lightGo.transform.localPosition = new Vector3(0f, 2.8f, -1.0f);
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = glow.color;
            light.intensity = 2.4f;
            light.range = 8f;
            light.shadows = LightShadows.None;
        }

        private static void CreateTrees(Material wood, Material leaf, Material leafLight)
        {
            Vector3[] positions =
            {
                new Vector3(-20f, 0f, -8f), new Vector3(-23f, 0f, 2f), new Vector3(-20f, 0f, 12f),
                new Vector3(20f, 0f, 3f), new Vector3(23f, 0f, 13f), new Vector3(18f, 0f, -20f),
                new Vector3(-18f, 0f, -20f), new Vector3(-13f, 0f, 27f), new Vector3(15f, 0f, 28f)
            };

            for (int i = 0; i < positions.Length; i++)
                CreateTree(positions[i], wood, i % 2 == 0 ? leaf : leafLight, 1f + (i % 3) * 0.10f);
        }

        private static void CreateTree(Vector3 position, Material wood, Material leaves, float scale)
        {
            CreateBlock("TreeTrunk", position + new Vector3(0f, 1.55f * scale, 0f), new Vector3(0.65f, 3.1f, 0.65f) * scale, wood);
            CreateBlock("TreeLeaves", position + new Vector3(0f, 3.75f * scale, 0f), new Vector3(3.1f, 2.3f, 3.1f) * scale, leaves, false);
            CreateBlock("TreeLeaves", position + new Vector3(0.9f, 4.1f, 0.3f) * scale, new Vector3(1.8f, 1.7f, 1.8f) * scale, leaves, false);
            CreateBlock("TreeLeaves", position + new Vector3(-0.8f, 4.0f, -0.4f) * scale, new Vector3(1.7f, 1.6f, 1.7f) * scale, leaves, false);
        }

        private static void CreateFlowerBeds(Material grassLight, Material pink, Material yellow, Material cyan)
        {
            Vector3[] patches =
            {
                new Vector3(-6f, 0.15f, 7f), new Vector3(6f, 0.15f, 7f),
                new Vector3(-7f, 0.15f, -2f), new Vector3(7f, 0.15f, -2f)
            };

            for (int p = 0; p < patches.Length; p++)
            {
                CreateBlock("FlowerBed", patches[p], new Vector3(3.2f, 0.12f, 1.8f), grassLight, false);

                for (int i = 0; i < 8; i++)
                {
                    float x = -1.2f + (i % 4) * 0.8f;
                    float z = -0.55f + (i / 4) * 1.1f;
                    Material mat = (i + p) % 3 == 0 ? cyan : ((i + p) % 2 == 0 ? pink : yellow);
                    CreateBlock("Flower", patches[p] + new Vector3(x, 0.28f, z), new Vector3(0.16f, 0.45f, 0.16f), mat, false);
                }
            }
        }

        private static void CreateLamps(Material stone, Material glow)
        {
            Vector3[] positions =
            {
                new Vector3(-5.5f, 0f, 9f), new Vector3(5.5f, 0f, 9f),
                new Vector3(-5.5f, 0f, -8f), new Vector3(5.5f, 0f, -8f)
            };

            foreach (Vector3 p in positions)
            {
                CreateBlock("LampPost", p + new Vector3(0f, 1.3f, 0f), new Vector3(0.22f, 2.6f, 0.22f), stone);
                CreateBlock("LampGlow", p + new Vector3(0f, 2.75f, 0f), new Vector3(0.42f, 0.55f, 0.42f), glow, false);
            }
        }

        private static void CreateScenery(Material stone, Material grass, Material cyan, Material violet)
        {
            Vector3[] rocks =
            {
                new Vector3(-25f, 1.2f, 7f), new Vector3(25f, 1.0f, 8f),
                new Vector3(-22f, 1.5f, -16f), new Vector3(22f, 1.3f, -19f),
                new Vector3(-19f, 1.0f, 24f), new Vector3(19f, 1.1f, 25f)
            };

            for (int i = 0; i < rocks.Length; i++)
            {
                GameObject rock = CreateBlock($"Rock_{i}", rocks[i], new Vector3(2.5f + (i % 2), 2.0f + (i % 3), 2.3f), stone);
                rock.transform.rotation = Quaternion.Euler(0f, i * 27f, 5f * (i % 3));
            }

            CreateCrystal(new Vector3(-18f, 1f, 14f), new Vector3(0.8f, 2.6f, 0.8f), cyan);
            CreateCrystal(new Vector3(-16.8f, 0.7f, 13.2f), new Vector3(0.5f, 1.8f, 0.5f), cyan);
            CreateCrystal(new Vector3(18f, 1f, 14f), new Vector3(0.8f, 2.6f, 0.8f), violet);
            CreateCrystal(new Vector3(16.8f, 0.7f, 13.2f), new Vector3(0.5f, 1.8f, 0.5f), violet);
        }

        private static void CreateDistantMountains(Material stone, Material grass)
        {
            for (int i = 0; i < 14; i++)
            {
                float angle = i / 14f * Mathf.PI * 2f;
                float radius = 35f;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 2.5f, Mathf.Sin(angle) * radius);
                float height = 5f + (i % 4) * 1.6f;

                GameObject mountain = CreateBlock(
                    "DistantHill",
                    pos,
                    new Vector3(6f, height, 6f),
                    i % 3 == 0 ? stone : grass);
                mountain.transform.rotation = Quaternion.Euler(0f, angle * Mathf.Rad2Deg, i % 2 == 0 ? 10f : -8f);
            }
        }

        private static void CreatePath(Vector3 pos, Vector3 scale, Material material)
        {
            CreateBlock("Path", pos, scale, material, false);
        }

        private static void CreateCrystal(Vector3 pos, Vector3 scale, Material material)
        {
            GameObject crystal = CreateBlock("Crystal", pos, scale, material, false);
            crystal.transform.rotation = Quaternion.Euler(0f, 45f, 18f);
        }

        private static GameObject CreateBlock(string name, Vector3 position, Vector3 scale, Material material, bool keepCollider = true)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = position;
            go.transform.localScale = scale;

            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer != null)
                renderer.sharedMaterial = material;

            if (!keepCollider)
            {
                Collider collider = go.GetComponent<Collider>();
                if (collider != null)
                    Object.DestroyImmediate(collider);
            }

            return go;
        }

        private static GameObject CreateBlockChild(Transform parent, string name, Vector3 localPosition, Vector3 scale, Material material, bool keepCollider = true)
        {
            GameObject go = CreateBlock(name, Vector3.zero, scale, material, keepCollider);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            return go;
        }

        private static void CreateWorldLabel(Transform parent, string value, Vector3 localPosition, Color color, int fontSize)
        {
            GameObject go = new GameObject("Label_" + value);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.Euler(18f, 180f, 0f);

            TextMesh text = go.AddComponent<TextMesh>();
            text.text = value;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = fontSize;
            text.characterSize = 0.075f;
            text.color = color;
        }
    }
}
#endif
