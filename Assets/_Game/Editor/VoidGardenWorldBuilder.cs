#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;

namespace VoxelDungeon.EditorTools
{
    public static class VoidGardenWorldBuilder
    {
        public static void Build()
        {
            Material floor = VisualStyleBuilder.GetMaterial("Void_Floor", new Color(0.72f, 0.76f, 0.74f), 0.04f, 0.40f);
            Material pale = VisualStyleBuilder.GetMaterial("Void_PaleStone", new Color(0.82f, 0.86f, 0.84f), 0.05f, 0.42f);
            Material wall = VisualStyleBuilder.GetMaterial("Void_Wall", new Color(0.50f, 0.56f, 0.58f), 0.06f, 0.34f);
            Material moss = VisualStyleBuilder.GetMaterial("Void_Moss", new Color(0.24f, 0.56f, 0.34f), 0f, 0.22f);
            Material violet = VisualStyleBuilder.GetMaterial("Void_Violet", new Color(0.62f, 0.30f, 1f), 0f, 0.54f, true);
            Material green = VisualStyleBuilder.GetMaterial("Void_Green", new Color(0.20f, 0.88f, 0.56f), 0f, 0.52f, true);
            Material water = VisualStyleBuilder.GetMaterial("Void_Water", new Color(0.24f, 0.72f, 0.82f), 0.02f, 0.76f, true);
            Material gold = VisualStyleBuilder.GetMaterial("Void_Gold", new Color(0.92f, 0.74f, 0.30f), 0.58f, 0.56f);

            GameObject ground = GameObject.Find("Mission Ground");
            if (ground != null)
            {
                Renderer renderer = ground.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = moss;
            }

            CreateGardenRoom("GardenApproach", new Vector3(0f, 0f, -28f), 18f, 12f, floor, pale, moss, green, 0);
            CreateConnector(new Vector3(0f, 0f, -20.5f), 6f, 3f, floor, pale, green);

            CreateGardenRoom("MoonTerrace", new Vector3(0f, 0f, -13f), 18f, 12f, floor, pale, moss, violet, 1);
            CreateGardenRoom("MirrorGrove", new Vector3(0f, 0f, 1f), 22f, 16f, floor, wall, moss, green, 2);

            CreateReflectingPool(new Vector3(-8.0f, 0.02f, 1f), 5f, 10f, water, pale);
            CreateReflectingPool(new Vector3(8.0f, 0.02f, 1f), 5f, 10f, water, pale);

            CreateConnector(new Vector3(0f, 0f, 10f), 6f, 2f, floor, wall, violet);
            CreateGardenRoom("StarShrine", new Vector3(0f, 0f, 18f), 18f, 14f, floor, pale, moss, violet, 3);
            CreateSideGarden(new Vector3(12f, 0f, 18f), floor, pale, green);

            CreateConnector(new Vector3(0f, 0f, 26f), 6f, 2f, floor, wall, violet);
            CreateGardenRoom("AstralCourt", new Vector3(0f, 0f, 34f), 20f, 14f, floor, pale, moss, green, 4);
            CreateConnector(new Vector3(0f, 0f, 41.5f), 7f, 1f, floor, wall, violet);

            CreateBossArena(new Vector3(0f, 0f, 52f), 26f, 20f, floor, pale, moss, violet, green, gold);
            CreateAtmosphere();
            WorldPostProcessingBuilder.ApplyVoid();
        }

        private static void CreateGardenRoom(
            string name,
            Vector3 center,
            float width,
            float depth,
            Material floor,
            Material wall,
            Material moss,
            Material glow,
            int variant)
        {
            GameObject root = new GameObject(name);
            root.transform.position = center;

            CreateSlab(root.transform, width, depth, floor);
            CreateBlock(root.transform, "Wall_L", new Vector3(-width * 0.5f, 1.4f, 0f), new Vector3(0.7f, 2.8f, depth), wall);
            CreateBlock(root.transform, "Wall_R", new Vector3(width * 0.5f, 1.4f, 0f), new Vector3(0.7f, 2.8f, depth), wall);

            for (int i = -1; i <= 1; i += 2)
            {
                CreateBlock(root.transform, "MossBed", new Vector3(i * width * 0.33f, 0.10f, 0f), new Vector3(2.8f, 0.10f, depth * 0.55f), moss, false);
                CreateCrystalFlower(root.transform, new Vector3(i * width * 0.34f, 0.55f, -depth * 0.18f), glow, 0.9f + variant * 0.05f);
            }

            CreateArch(root.transform, new Vector3(0f, 0f, depth * 0.5f - 0.35f), wall, glow, width > 20f ? 5.8f : 5.0f);
            CreatePointLight(root.transform, new Vector3(0f, 3.2f, 0f), glow.color, 2.0f + variant * 0.18f, Mathf.Max(width, depth) * 0.60f);
        }

        private static void CreateConnector(Vector3 center, float width, float depth, Material floor, Material wall, Material glow)
        {
            GameObject root = new GameObject("VoidConnector");
            root.transform.position = center;
            CreateSlab(root.transform, width, depth, floor);
            CreateBlock(root.transform, "Wall_L", new Vector3(-width * 0.5f, 1.2f, 0f), new Vector3(0.65f, 2.4f, depth), wall);
            CreateBlock(root.transform, "Wall_R", new Vector3(width * 0.5f, 1.2f, 0f), new Vector3(0.65f, 2.4f, depth), wall);
            CreateBlock(root.transform, "Guide", new Vector3(0f, 0.11f, 0f), new Vector3(1.1f, 0.05f, depth * 0.75f), glow, false);
        }

        private static void CreateReflectingPool(Vector3 center, float width, float depth, Material water, Material stone)
        {
            GameObject root = new GameObject("ReflectingPool");
            root.transform.position = center;
            CreateBlock(root.transform, "Water", new Vector3(0f, 0f, 0f), new Vector3(width, 0.06f, depth), water, false);
            CreateBlock(root.transform, "Edge_L", new Vector3(-width * 0.5f, 0.22f, 0f), new Vector3(0.35f, 0.44f, depth), stone);
            CreateBlock(root.transform, "Edge_R", new Vector3(width * 0.5f, 0.22f, 0f), new Vector3(0.35f, 0.44f, depth), stone);
        }

        private static void CreateSideGarden(Vector3 center, Material floor, Material wall, Material glow)
        {
            GameObject root = new GameObject("SideGarden");
            root.transform.position = center;
            CreateSlab(root.transform, 7f, 8f, floor);
            CreateBlock(root.transform, "BackWall", new Vector3(0f, 1.6f, 4f), new Vector3(7f, 3.2f, 0.6f), wall);
            CreateCrystalFlower(root.transform, new Vector3(0f, 0.7f, 1.4f), glow, 1.3f);
            CreatePointLight(root.transform, new Vector3(0f, 2.8f, 0f), glow.color, 2.4f, 7f);
        }

        private static void CreateBossArena(
            Vector3 center,
            float width,
            float depth,
            Material floor,
            Material wall,
            Material moss,
            Material violet,
            Material green,
            Material gold)
        {
            GameObject root = new GameObject("VoidBossArena");
            root.transform.position = center;

            CreateSlab(root.transform, width, depth, floor);
            CreateBlock(root.transform, "Wall_L", new Vector3(-width * 0.5f, 2.2f, 0f), new Vector3(1f, 4.4f, depth), wall);
            CreateBlock(root.transform, "Wall_R", new Vector3(width * 0.5f, 2.2f, 0f), new Vector3(1f, 4.4f, depth), wall);
            CreateBlock(root.transform, "Wall_N", new Vector3(0f, 2.2f, depth * 0.5f), new Vector3(width, 4.4f, 1f), wall);

            CreateBlock(root.transform, "GardenSigil", new Vector3(0f, 0.11f, 2f), new Vector3(8f, 0.09f, 8f), violet, false);
            CreateBlock(root.transform, "MossRing", new Vector3(0f, 0.16f, 2f), new Vector3(5.8f, 0.05f, 5.8f), moss, false);

            CreateCrystalFlower(root.transform, new Vector3(-9f, 0.9f, 3f), green, 1.6f);
            CreateCrystalFlower(root.transform, new Vector3(9f, 0.9f, 3f), violet, 1.6f);

            for (int i = -2; i <= 2; i++)
                CreateBlock(root.transform, "MoonPillar", new Vector3(i * 4.2f, 2.1f, depth * 0.36f), new Vector3(0.8f, 4.2f, 0.8f), i % 2 == 0 ? gold : wall);

            CreatePointLight(root.transform, new Vector3(0f, 4f, 1f), violet.color, 3.4f, 14f);
        }

        private static void CreateArch(Transform parent, Vector3 pos, Material wall, Material glow, float gap)
        {
            CreateBlock(parent, "Arch_L", pos + new Vector3(-gap * 0.5f, 1.8f, 0f), new Vector3(0.72f, 3.6f, 0.82f), wall);
            CreateBlock(parent, "Arch_R", pos + new Vector3(gap * 0.5f, 1.8f, 0f), new Vector3(0.72f, 3.6f, 0.82f), wall);
            CreateBlock(parent, "ArchTop", pos + new Vector3(0f, 3.55f, 0f), new Vector3(gap + 1.44f, 0.58f, 0.82f), glow, false);
        }

        private static void CreateCrystalFlower(Transform parent, Vector3 pos, Material glow, float scale)
        {
            for (int i = 0; i < 5; i++)
            {
                float angle = i / 5f * Mathf.PI * 2f;
                Vector3 offset = new Vector3(Mathf.Cos(angle) * 0.32f, 0f, Mathf.Sin(angle) * 0.32f);
                GameObject petal = CreateBlock(parent, "Petal", pos + offset, new Vector3(0.18f, 0.72f, 0.18f) * scale, glow, false);
                petal.transform.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 24f);
            }
            CreateBlock(parent, "FlowerCore", pos + new Vector3(0f, 0.18f, 0f), new Vector3(0.30f, 0.30f, 0.30f) * scale, glow, false);
        }

        private static void CreateSlab(Transform parent, float width, float depth, Material material)
        {
            CreateBlock(parent, "FloorSlab", new Vector3(0f, 0.055f, 0f), new Vector3(width - 0.12f, 0.08f, depth - 0.12f), material, false);
        }

        private static GameObject CreateBlock(Transform parent, string name, Vector3 localPosition, Vector3 scale, Material material, bool keepCollider = true)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
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

        private static void CreatePointLight(Transform parent, Vector3 pos, Color color, float intensity, float range)
        {
            GameObject go = new GameObject("GardenLight");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;

            Light light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private static void CreateAtmosphere()
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.62f, 0.70f, 0.76f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0055f;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.72f, 0.76f, 0.86f);
            RenderSettings.ambientEquatorColor = new Color(0.46f, 0.56f, 0.56f);
            RenderSettings.ambientGroundColor = new Color(0.24f, 0.30f, 0.26f);

            Light key = Object.FindFirstObjectByType<Light>();
            if (key != null && key.type == LightType.Directional)
            {
                key.color = new Color(0.88f, 0.90f, 1f);
                key.intensity = 1.28f;
                key.shadows = LightShadows.Soft;
            }
        }
    }
}
#endif
