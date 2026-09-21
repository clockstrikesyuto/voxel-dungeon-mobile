#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;

namespace VoxelDungeon.EditorTools
{
    public static class CrystalCryptWorldBuilder
    {
        public static void Build()
        {
            Material baseFloor = VisualStyleBuilder.GetMaterial("Crypt_Floor", new Color(0.50f, 0.58f, 0.62f), 0.04f, 0.34f);
            Material floorLight = VisualStyleBuilder.GetMaterial("Crypt_FloorLight", new Color(0.68f, 0.75f, 0.76f), 0.03f, 0.38f);
            Material wall = VisualStyleBuilder.GetMaterial("Crypt_Wall", new Color(0.39f, 0.46f, 0.50f), 0.06f, 0.30f);
            Material wallDark = VisualStyleBuilder.GetMaterial("Crypt_WallDark", new Color(0.22f, 0.27f, 0.32f), 0.10f, 0.26f);
            Material trim = VisualStyleBuilder.GetMaterial("Crypt_Trim", new Color(0.76f, 0.81f, 0.80f), 0.18f, 0.42f);
            Material cyan = VisualStyleBuilder.GetMaterial("Crypt_Cyan", new Color(0.12f, 0.88f, 1f), 0f, 0.55f, true);
            Material blue = VisualStyleBuilder.GetMaterial("Crypt_Blue", new Color(0.18f, 0.50f, 1f), 0f, 0.50f, true);
            Material violet = VisualStyleBuilder.GetMaterial("Crypt_Violet", new Color(0.63f, 0.30f, 1f), 0f, 0.52f, true);
            Material bossStone = VisualStyleBuilder.GetMaterial("Crypt_BossStone", new Color(0.15f, 0.18f, 0.24f), 0.18f, 0.28f);
            Material gold = VisualStyleBuilder.GetMaterial("Crypt_Gold", new Color(0.92f, 0.68f, 0.20f), 0.58f, 0.55f);

            GameObject ground = GameObject.Find("Mission Ground");
            if (ground != null)
            {
                Renderer renderer = ground.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = baseFloor;
            }

            CreateRoom("EntryCourt", new Vector3(0f, 0f, -28f), 18f, 12f, floorLight, wall, trim, cyan, 0);
            CreateCorridor("EntryPassage", new Vector3(0f, 0f, -20.5f), 6f, 3f, baseFloor, wall, cyan);

            CreateRoom("CrystalHall", new Vector3(0f, 0f, -13f), 16f, 12f, baseFloor, wall, trim, cyan, 1);
            // Crystal Hall and Crossing touch directly; no overlapping floor strip is needed.

            CreateRoom("Crossing", new Vector3(0f, 0f, 1f), 22f, 16f, floorLight, wall, trim, blue, 2);
            CreateSideAlcove(new Vector3(-13f, 0f, 1f), new Vector3(7f, 0.08f, 8f), floorLight, wall, cyan);
            CreateCorridor("CrossingPassage", new Vector3(0f, 0f, 10f), 6f, 2f, baseFloor, wallDark, blue);

            CreateRoom("CrystalGallery", new Vector3(0f, 0f, 18f), 18f, 14f, baseFloor, wallDark, trim, blue, 3);
            CreateSideAlcove(new Vector3(12f, 0f, 18f), new Vector3(7f, 0.08f, 8f), baseFloor, wallDark, violet);
            CreateCorridor("DeepPassage", new Vector3(0f, 0f, 26f), 6f, 2f, baseFloor, wallDark, violet);

            CreateRoom("InnerSanctum", new Vector3(0f, 0f, 34f), 20f, 14f, baseFloor, wallDark, trim, violet, 4);
            CreateCorridor("BossApproach", new Vector3(0f, 0f, 41.5f), 7f, 1f, bossStone, bossStone, violet);

            CreateBossArena(new Vector3(0f, 0f, 52f), 26f, 20f, bossStone, wallDark, gold, violet);

            CreateBreadcrumbCrystals(cyan, blue, violet);
            CreateAtmosphere();
            WorldPostProcessingBuilder.ApplyCrypt();
        }

        private static void CreateRoom(
            string name,
            Vector3 center,
            float width,
            float depth,
            Material floor,
            Material wall,
            Material trim,
            Material glow,
            int variant)
        {
            GameObject root = new GameObject(name);
            root.transform.position = center;

            CreateFloorTiles(root.transform, width, depth, floor, variant);
            if (variant == 2)
                CreateSideWallWithGap(root.transform, -width * 0.5f, depth, wall, "Wall_L");
            else
                CreateBlockChild(root.transform, "Wall_L", new Vector3(-width * 0.5f, 1.5f, 0f), new Vector3(0.7f, 3.0f, depth), wall);

            if (variant == 3)
                CreateSideWallWithGap(root.transform, width * 0.5f, depth, wall, "Wall_R");
            else
                CreateBlockChild(root.transform, "Wall_R", new Vector3(width * 0.5f, 1.5f, 0f), new Vector3(0.7f, 3.0f, depth), wall);

            CreatePillar(root.transform, new Vector3(-width * 0.5f + 0.8f, 1.9f, -depth * 0.5f + 1.0f), wall, trim);
            CreatePillar(root.transform, new Vector3(width * 0.5f - 0.8f, 1.9f, -depth * 0.5f + 1.0f), wall, trim);
            CreatePillar(root.transform, new Vector3(-width * 0.5f + 0.8f, 1.9f, depth * 0.5f - 1.0f), wall, trim);
            CreatePillar(root.transform, new Vector3(width * 0.5f - 0.8f, 1.9f, depth * 0.5f - 1.0f), wall, trim);

            CreateArch(root.transform, new Vector3(0f, 0f, depth * 0.5f - 0.3f), wall, trim, width > 20f ? 5.8f : 5.0f);
            CreateCrystalCluster(root.transform, new Vector3(-width * 0.38f, 0.65f, 0f), glow, 1.0f + variant * 0.06f);
            CreateCrystalCluster(root.transform, new Vector3(width * 0.38f, 0.65f, depth * 0.18f), glow, 0.85f + variant * 0.05f);
            CreateRoomLight(root.transform, new Vector3(0f, 3.2f, 0f), glow.color, 2.2f + variant * 0.2f, Mathf.Max(width, depth) * 0.65f);
        }

        private static void CreateSideWallWithGap(
            Transform parent,
            float x,
            float depth,
            Material wall,
            string prefix)
        {
            float gap = 4.2f;
            float segment = Mathf.Max(1f, (depth - gap) * 0.5f);

            CreateBlockChild(
                parent,
                prefix + "_A",
                new Vector3(x, 1.5f, -(gap * 0.5f + segment * 0.5f)),
                new Vector3(0.7f, 3.0f, segment),
                wall);

            CreateBlockChild(
                parent,
                prefix + "_B",
                new Vector3(x, 1.5f, gap * 0.5f + segment * 0.5f),
                new Vector3(0.7f, 3.0f, segment),
                wall);
        }

        private static void CreateCorridor(
            string name,
            Vector3 center,
            float width,
            float depth,
            Material floor,
            Material wall,
            Material glow)
        {
            GameObject root = new GameObject(name);
            root.transform.position = center;

            CreateFloorTiles(root.transform, width, depth, floor, 0);
            CreateBlockChild(root.transform, "Wall_L", new Vector3(-width * 0.5f, 1.5f, 0f), new Vector3(0.7f, 3f, depth), wall);
            CreateBlockChild(root.transform, "Wall_R", new Vector3(width * 0.5f, 1.5f, 0f), new Vector3(0.7f, 3f, depth), wall);

            CreateCrystalCluster(root.transform, new Vector3(-width * 0.38f, 0.55f, 0f), glow, 0.65f);
            CreateCrystalCluster(root.transform, new Vector3(width * 0.38f, 0.55f, 1.6f), glow, 0.52f);
        }

        private static void CreateSideAlcove(Vector3 center, Vector3 size, Material floor, Material wall, Material glow)
        {
            GameObject root = new GameObject("SideAlcove");
            root.transform.position = center;

            CreateBlockChild(root.transform, "Floor", new Vector3(0f, 0.04f, 0f), size, floor, false);
            CreateBlockChild(root.transform, "BackWall", new Vector3(0f, 1.6f, size.z * 0.5f), new Vector3(size.x, 3.2f, 0.6f), wall);
            CreateCrystalCluster(root.transform, new Vector3(0f, 0.65f, 1.4f), glow, 1.15f);
            CreateRoomLight(root.transform, new Vector3(0f, 2.8f, 0f), glow.color, 2.0f, 7f);
        }

        private static void CreateBossArena(
            Vector3 center,
            float width,
            float depth,
            Material floor,
            Material wall,
            Material gold,
            Material glow)
        {
            GameObject root = new GameObject("BossArena");
            root.transform.position = center;

            CreateFloorTiles(root.transform, width, depth, floor, 5);
            CreateBlockChild(root.transform, "Wall_L", new Vector3(-width * 0.5f, 2.2f, 0f), new Vector3(1f, 4.4f, depth), wall);
            CreateBlockChild(root.transform, "Wall_R", new Vector3(width * 0.5f, 2.2f, 0f), new Vector3(1f, 4.4f, depth), wall);
            CreateBlockChild(root.transform, "Wall_N", new Vector3(0f, 2.2f, depth * 0.5f), new Vector3(width, 4.4f, 1f), wall);

            for (int i = -2; i <= 2; i++)
            {
                CreatePillar(root.transform, new Vector3(i * 4.5f, 2.4f, depth * 0.38f), wall, gold);
            }

            CreateBlockChild(root.transform, "BossSigil", new Vector3(0f, 0.08f, 2f), new Vector3(7f, 0.10f, 7f), glow, false);
            CreateCrystalCluster(root.transform, new Vector3(-9f, 0.8f, 4f), glow, 1.6f);
            CreateCrystalCluster(root.transform, new Vector3(9f, 0.8f, 4f), glow, 1.6f);
            CreateRoomLight(root.transform, new Vector3(0f, 4f, 1f), glow.color, 3.6f, 14f);
        }

        private static void CreateFloorTiles(Transform parent, float width, float depth, Material material, int variant)
        {
            CreateBlockChild(
                parent,
                "FloorSlab",
                new Vector3(0f, 0.055f, 0f),
                new Vector3(width - 0.12f, 0.08f, depth - 0.12f),
                material,
                false);

            // Sparse decorative inlays sit clearly above the slab, avoiding coplanar overlap.
            if (width >= 12f && depth >= 10f)
            {
                Material inlay = VisualStyleBuilder.GetMaterial(
                    "Crypt_FloorInlay",
                    new Color(0.42f, 0.52f, 0.56f),
                    0.08f,
                    0.38f);

                int count = Mathf.Clamp(Mathf.FloorToInt(width / 6f), 2, 4);
                for (int i = 0; i < count; i++)
                {
                    float x = Mathf.Lerp(-width * 0.28f, width * 0.28f, count == 1 ? 0.5f : i / (float)(count - 1));
                    float z = ((i + variant) % 2 == 0) ? -depth * 0.20f : depth * 0.20f;

                    CreateBlockChild(
                        parent,
                        "FloorInlay",
                        new Vector3(x, 0.105f, z),
                        new Vector3(1.25f, 0.018f, 1.25f),
                        inlay,
                        false);
                }
            }
        }

        private static void CreatePillar(Transform parent, Vector3 localPos, Material stone, Material trim)
        {
            CreateBlockChild(parent, "PillarBase", localPos + new Vector3(0f, -1.35f, 0f), new Vector3(1.15f, 0.45f, 1.15f), trim);
            CreateBlockChild(parent, "Pillar", localPos, new Vector3(0.78f, 3.4f, 0.78f), stone);
            CreateBlockChild(parent, "PillarCap", localPos + new Vector3(0f, 1.8f, 0f), new Vector3(1.15f, 0.38f, 1.15f), trim);
        }

        private static void CreateArch(Transform parent, Vector3 localPos, Material stone, Material trim, float gapWidth)
        {
            CreateBlockChild(parent, "Arch_L", localPos + new Vector3(-gapWidth * 0.5f, 1.8f, 0f), new Vector3(0.75f, 3.6f, 0.85f), stone);
            CreateBlockChild(parent, "Arch_R", localPos + new Vector3(gapWidth * 0.5f, 1.8f, 0f), new Vector3(0.75f, 3.6f, 0.85f), stone);
            CreateBlockChild(parent, "ArchTop", localPos + new Vector3(0f, 3.55f, 0f), new Vector3(gapWidth + 1.5f, 0.60f, 0.85f), trim);
        }

        private static void CreateCrystalCluster(Transform parent, Vector3 localPos, Material glow, float scale)
        {
            GameObject a = CreateBlockChild(parent, "Crystal", localPos, new Vector3(0.34f, 1.6f, 0.34f) * scale, glow, false);
            a.transform.localRotation = Quaternion.Euler(0f, 25f, 12f);

            GameObject b = CreateBlockChild(parent, "Crystal", localPos + new Vector3(0.42f, -0.18f, 0.25f), new Vector3(0.24f, 1.05f, 0.24f) * scale, glow, false);
            b.transform.localRotation = Quaternion.Euler(0f, -22f, -10f);

            GameObject c = CreateBlockChild(parent, "Crystal", localPos + new Vector3(-0.38f, -0.25f, 0.15f), new Vector3(0.20f, 0.82f, 0.20f) * scale, glow, false);
            c.transform.localRotation = Quaternion.Euler(0f, 8f, 18f);
        }

        private static void CreateBreadcrumbCrystals(Material cyan, Material blue, Material violet)
        {
            Vector3[] positions =
            {
                new Vector3(-5.5f, 0.65f, -24f), new Vector3(5.5f, 0.65f, -16f),
                new Vector3(-7.5f, 0.65f, -2f), new Vector3(7f, 0.65f, 8f),
                new Vector3(-6.5f, 0.65f, 20f), new Vector3(6.5f, 0.65f, 30f),
                new Vector3(-8f, 0.65f, 42f), new Vector3(8f, 0.65f, 48f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                Material mat = i < 3 ? cyan : (i < 6 ? blue : violet);
                GameObject root = new GameObject("BreadcrumbCrystal");
                root.transform.position = positions[i];
                CreateCrystalCluster(root.transform, Vector3.zero, mat, 0.65f + (i % 3) * 0.1f);
            }
        }

        private static void CreateRoomLight(Transform parent, Vector3 localPos, Color color, float intensity, float range)
        {
            GameObject go = new GameObject("CrystalLight");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;

            Light light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private static GameObject CreateBlockChild(
            Transform parent,
            string name,
            Vector3 localPosition,
            Vector3 scale,
            Material material,
            bool keepCollider = true)
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

        private static void CreateAtmosphere()
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.20f, 0.31f, 0.38f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0038f;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.56f, 0.66f, 0.72f);
            RenderSettings.ambientEquatorColor = new Color(0.38f, 0.48f, 0.54f);
            RenderSettings.ambientGroundColor = new Color(0.24f, 0.29f, 0.34f);

            Light key = Object.FindFirstObjectByType<Light>();
            if (key != null && key.type == LightType.Directional)
            {
                key.color = new Color(0.74f, 0.86f, 1f);
                key.intensity = 1.58f;
                key.shadows = LightShadows.Soft;
            }
        }
    }
}
#endif
