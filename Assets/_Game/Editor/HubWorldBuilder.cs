#if UNITY_EDITOR
using UnityEngine;
using VoxelDungeon.Core;

namespace VoxelDungeon.EditorTools
{
    public static class HubWorldBuilder
    {
        public static void PopulateHub(GameObject player)
        {
            Material grass = VisualStyleBuilder.GetMaterial("Hub_Grass", new Color(0.055f, 0.13f, 0.11f), 0f, 0.22f);
            Material plaza = VisualStyleBuilder.GetMaterial("Hub_Plaza", new Color(0.10f, 0.13f, 0.15f), 0.08f, 0.38f);
            Material path = VisualStyleBuilder.GetMaterial("Hub_Path", new Color(0.13f, 0.16f, 0.17f), 0.05f, 0.32f);
            Material stone = VisualStyleBuilder.GetMaterial("Hub_Stone", new Color(0.12f, 0.13f, 0.15f), 0.12f, 0.25f);
            Material darkStone = VisualStyleBuilder.GetMaterial("Hub_DarkStone", new Color(0.05f, 0.06f, 0.08f), 0.16f, 0.28f);
            Material wood = VisualStyleBuilder.GetMaterial("Hub_Wood", new Color(0.21f, 0.10f, 0.055f), 0f, 0.22f);
            Material cyan = VisualStyleBuilder.GetMaterial("Hub_Cyan", new Color(0.08f, 0.80f, 0.94f), 0f, 0.48f, true);
            Material orange = VisualStyleBuilder.GetMaterial("Hub_Orange", new Color(1f, 0.34f, 0.08f), 0f, 0.48f, true);
            Material violet = VisualStyleBuilder.GetMaterial("Hub_Violet", new Color(0.56f, 0.22f, 1f), 0f, 0.5f, true);
            Material gold = VisualStyleBuilder.GetMaterial("Hub_Gold", new Color(0.92f, 0.62f, 0.14f), 0.55f, 0.5f);

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

            CreateFountain(cyan, stone);
            CreateWorldMapTable(new Vector3(0f, 0f, 6.0f), darkStone, cyan, gold);
            CreateForge(new Vector3(-10f, 0f, -6f), stone, darkStone, orange, wood);
            CreateMerchant(new Vector3(10f, 0f, -6f), stone, wood, gold);
            CreateTraining(new Vector3(0f, 0f, -15f), stone, wood, orange);

            CreateStageGate(
                "Gate_CrystalCrypt",
                new Vector3(-12f, 0f, 20f),
                cyan,
                stone,
                "CRYSTAL CRYPT",
                "Enter the first expedition",
                "stage.crypt");

            CreateStageGate(
                "Gate_AshenForge",
                new Vector3(0f, 0f, 23f),
                orange,
                darkStone,
                "ASHEN FORGE",
                "Unlock by clearing Crystal Crypt",
                "stage.ashen");

            CreateStageGate(
                "Gate_VoidGarden",
                new Vector3(12f, 0f, 20f),
                violet,
                darkStone,
                "VOID GARDEN",
                "The distant frontier remains sealed",
                "stage.void");

            CreateScenery(stone, grass, cyan, violet);

            GameObject controller = new GameObject("HubController");
            HubController hub = controller.AddComponent<HubController>();
            hub.SetPlayer(player.transform);

            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.045f, 0.075f, 0.075f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.010f;
            RenderSettings.ambientLight = new Color(0.16f, 0.20f, 0.18f);
        }

        private static void CreateFountain(Material glow, Material stone)
        {
            CreateBlock("FountainBase", new Vector3(0f, 0.30f, 0f), new Vector3(3.6f, 0.6f, 3.6f), stone);
            CreateBlock("FountainWater", new Vector3(0f, 0.64f, 0f), new Vector3(2.85f, 0.08f, 2.85f), glow, false);
            CreateBlock("FountainPillar", new Vector3(0f, 1.3f, 0f), new Vector3(0.65f, 2.0f, 0.65f), stone);
            CreateCrystal(new Vector3(0f, 2.65f, 0f), new Vector3(0.55f, 1.4f, 0.55f), glow);

            GameObject lightGo = new GameObject("FountainGlow");
            lightGo.transform.position = new Vector3(0f, 2.1f, 0f);
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.1f, 0.85f, 1f);
            light.intensity = 2.8f;
            light.range = 8f;
            light.shadows = LightShadows.None;
        }

        private static void CreateWorldMapTable(Vector3 position, Material stone, Material glow, Material gold)
        {
            GameObject root = new GameObject("WorldMapTable");
            root.transform.position = position;

            CreateBlockChild(root.transform, "Base", new Vector3(0f, 0.55f, 0f), new Vector3(2.8f, 1.1f, 2.0f), stone);
            CreateBlockChild(root.transform, "MapSurface", new Vector3(0f, 1.16f, 0f), new Vector3(2.6f, 0.10f, 1.8f), glow);
            CreateBlockChild(root.transform, "Frame_N", new Vector3(0f, 1.25f, 0.92f), new Vector3(2.8f, 0.18f, 0.18f), gold);
            CreateBlockChild(root.transform, "Frame_S", new Vector3(0f, 1.25f, -0.92f), new Vector3(2.8f, 0.18f, 0.18f), gold);

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(
                HubPointKind.WorldMap,
                "FRONTIER MAP",
                "View all known regions and routes",
                radius: 2.8f);

            CreateWorldLabel(root.transform, "WORLD MAP", new Vector3(0f, 2.15f, 0f), Color.white, 42);
        }

        private static void CreateForge(Vector3 position, Material stone, Material dark, Material glow, Material wood)
        {
            GameObject root = new GameObject("EmberForge");
            root.transform.position = position;

            CreateBlockChild(root.transform, "Floor", new Vector3(0f, 0.12f, 0f), new Vector3(6f, 0.24f, 5f), stone);
            CreateBlockChild(root.transform, "BackWall", new Vector3(0f, 2f, 2.1f), new Vector3(6f, 4f, 0.45f), dark);
            CreateBlockChild(root.transform, "Roof", new Vector3(0f, 4.0f, 0.7f), new Vector3(6.4f, 0.35f, 3.4f), wood);
            CreateBlockChild(root.transform, "Anvil", new Vector3(0f, 0.75f, -0.5f), new Vector3(1.4f, 0.65f, 0.75f), stone);
            CreateBlockChild(root.transform, "ForgeFire", new Vector3(-1.9f, 1.0f, 1.55f), new Vector3(1.1f, 1.6f, 0.3f), glow, false);

            GameObject lightGo = new GameObject("ForgeLight");
            lightGo.transform.SetParent(root.transform, false);
            lightGo.transform.localPosition = new Vector3(-1.9f, 1.6f, 0.2f);
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.28f, 0.06f);
            light.intensity = 4f;
            light.range = 7f;

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(
                HubPointKind.Forge,
                "EMBER FORGE",
                "Spend gold on permanent weapon upgrades",
                radius: 3.3f);

            CreateWorldLabel(root.transform, "EMBER FORGE", new Vector3(0f, 4.8f, 1.8f), new Color(1f, 0.5f, 0.15f), 34);
        }

        private static void CreateMerchant(Vector3 position, Material stone, Material wood, Material gold)
        {
            GameObject root = new GameObject("FrontierMerchant");
            root.transform.position = position;

            CreateBlockChild(root.transform, "Platform", new Vector3(0f, 0.14f, 0f), new Vector3(6f, 0.28f, 5f), stone);
            CreateBlockChild(root.transform, "Counter", new Vector3(0f, 1.0f, -1.0f), new Vector3(4.8f, 1.2f, 0.75f), wood);
            CreateBlockChild(root.transform, "Awning", new Vector3(0f, 3.4f, 0.2f), new Vector3(5.7f, 0.28f, 3.4f), gold);
            CreateBlockChild(root.transform, "Crate_L", new Vector3(-2.0f, 0.65f, 1.2f), new Vector3(1.1f, 1.1f, 1.1f), wood);
            CreateBlockChild(root.transform, "Crate_R", new Vector3(2.0f, 0.65f, 1.2f), new Vector3(1.1f, 1.1f, 1.1f), wood);

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(
                HubPointKind.Merchant,
                "FRONTIER MERCHANT",
                "Browse supplies and rotating loot",
                radius: 3.3f);

            CreateWorldLabel(root.transform, "MERCHANT", new Vector3(0f, 4.35f, 0.5f), new Color(1f, 0.72f, 0.18f), 34);
        }

        private static void CreateTraining(Vector3 position, Material stone, Material wood, Material glow)
        {
            GameObject root = new GameObject("TrainingYard");
            root.transform.position = position;

            CreateBlockChild(root.transform, "Floor", new Vector3(0f, 0.1f, 0f), new Vector3(10f, 0.2f, 7f), stone);
            CreateBlockChild(root.transform, "DummyA", new Vector3(-2.5f, 1.1f, 0.5f), new Vector3(0.7f, 2.2f, 0.7f), wood);
            CreateBlockChild(root.transform, "DummyB", new Vector3(0f, 1.1f, 1.1f), new Vector3(0.7f, 2.2f, 0.7f), wood);
            CreateBlockChild(root.transform, "DummyC", new Vector3(2.5f, 1.1f, 0.5f), new Vector3(0.7f, 2.2f, 0.7f), wood);
            CreateBlockChild(root.transform, "Banner", new Vector3(0f, 2.8f, 3.0f), new Vector3(5.2f, 1.0f, 0.18f), glow, false);

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(
                HubPointKind.Training,
                "TRAINING YARD",
                "Practice movement and weapon handling",
                radius: 4.0f);

            CreateWorldLabel(root.transform, "TRAINING", new Vector3(0f, 4.2f, 2.7f), new Color(1f, 0.48f, 0.14f), 34);
        }

        private static void CreateStageGate(
            string name,
            Vector3 position,
            Material glow,
            Material stone,
            string stageName,
            string subtitle,
            string stageId)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;

            CreateBlockChild(root.transform, "Pillar_L", new Vector3(-2.2f, 2.6f, 0f), new Vector3(1.15f, 5.2f, 1.15f), stone);
            CreateBlockChild(root.transform, "Pillar_R", new Vector3(2.2f, 2.6f, 0f), new Vector3(1.15f, 5.2f, 1.15f), stone);
            CreateBlockChild(root.transform, "Lintel", new Vector3(0f, 5.0f, 0f), new Vector3(5.5f, 1.0f, 1.15f), stone);
            CreateBlockChild(root.transform, "Portal", new Vector3(0f, 2.65f, 0.05f), new Vector3(3.2f, 3.9f, 0.16f), glow, false);
            CreateBlockChild(root.transform, "Step", new Vector3(0f, 0.22f, -1.1f), new Vector3(5.6f, 0.44f, 2.4f), stone);

            HubPoint point = root.AddComponent<HubPoint>();
            point.Configure(
                HubPointKind.StageGate,
                stageName,
                subtitle,
                stageId,
                "Mission_Test",
                3.6f);

            CreateWorldLabel(root.transform, stageName, new Vector3(0f, 6.2f, 0f), glow.color, 32);

            GameObject lightGo = new GameObject(name + "_Light");
            lightGo.transform.SetParent(root.transform, false);
            lightGo.transform.localPosition = new Vector3(0f, 2.8f, -1.0f);
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = glow.color;
            light.intensity = 3.0f;
            light.range = 8f;
            light.shadows = LightShadows.None;
        }

        private static void CreateScenery(Material stone, Material grass, Material cyan, Material violet)
        {
            Vector3[] rocks =
            {
                new Vector3(-22f, 1.2f, 8f), new Vector3(23f, 1.0f, 7f),
                new Vector3(-20f, 1.5f, -15f), new Vector3(18f, 1.3f, -18f),
                new Vector3(-17f, 1.0f, 21f), new Vector3(17f, 1.1f, 23f)
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

            for (int i = 0; i < 16; i++)
            {
                float angle = i / 16f * Mathf.PI * 2f;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 27f, 0.65f, Mathf.Sin(angle) * 27f);
                GameObject border = CreateBlock($"BoundaryStone_{i}", pos, new Vector3(3.0f, 1.3f, 3.0f), grass);
                border.transform.rotation = Quaternion.Euler(0f, angle * Mathf.Rad2Deg, 0f);
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

        private static GameObject CreateBlock(
            string name,
            Vector3 position,
            Vector3 scale,
            Material material,
            bool keepCollider = true)
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

        private static GameObject CreateBlockChild(
            Transform parent,
            string name,
            Vector3 localPosition,
            Vector3 scale,
            Material material,
            bool keepCollider = true)
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
