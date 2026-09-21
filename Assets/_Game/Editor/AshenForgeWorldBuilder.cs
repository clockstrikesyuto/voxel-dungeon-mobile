#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;

namespace VoxelDungeon.EditorTools
{
    public static class AshenForgeWorldBuilder
    {
        public static void Build()
        {
            Material floor = VisualStyleBuilder.GetMaterial("Ashen_Floor", new Color(0.22f, 0.24f, 0.25f), 0.30f, 0.40f);
            Material floorWarm = VisualStyleBuilder.GetMaterial("Ashen_FloorWarm", new Color(0.30f, 0.27f, 0.24f), 0.28f, 0.38f);
            Material wall = VisualStyleBuilder.GetMaterial("Ashen_Wall", new Color(0.16f, 0.17f, 0.18f), 0.18f, 0.28f);
            Material metal = VisualStyleBuilder.GetMaterial("Ashen_Metal", new Color(0.34f, 0.35f, 0.36f), 0.72f, 0.52f);
            Material trim = VisualStyleBuilder.GetMaterial("Ashen_Trim", new Color(0.50f, 0.42f, 0.32f), 0.48f, 0.44f);
            Material lava = VisualStyleBuilder.GetMaterial("Ashen_Lava", new Color(1f, 0.20f, 0.025f), 0f, 0.62f, true);
            Material ember = VisualStyleBuilder.GetMaterial("Ashen_Ember", new Color(1f, 0.55f, 0.08f), 0f, 0.58f, true);
            Material core = VisualStyleBuilder.GetMaterial("Ashen_Core", new Color(1f, 0.78f, 0.18f), 0f, 0.58f, true);

            GameObject ground = GameObject.Find("Mission Ground");
            if (ground != null)
            {
                Renderer renderer = ground.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.sharedMaterial = wall;
            }

            CreateRoom("ForgeGate", new Vector3(0f, 0f, -28f), 18f, 12f, floorWarm, wall, metal, ember, 0);
            CreateConnector(new Vector3(0f, 0f, -20.5f), 6f, 3f, floor, wall, ember);

            CreateRoom("SmelterRow", new Vector3(0f, 0f, -13f), 18f, 12f, floor, wall, metal, lava, 1);
            CreateRoom("ConveyorCross", new Vector3(0f, 0f, 1f), 22f, 16f, floorWarm, wall, trim, ember, 2);

            CreateLavaCanal(new Vector3(-8.5f, 0.0f, 1f), 3.5f, 14f, lava, metal);
            CreateLavaCanal(new Vector3(8.5f, 0.0f, 1f), 3.5f, 14f, lava, metal);

            CreateConnector(new Vector3(0f, 0f, 10f), 6f, 2f, floor, wall, ember);
            CreateRoom("FurnaceHall", new Vector3(0f, 0f, 18f), 18f, 14f, floor, wall, metal, lava, 3);

            CreateSideFoundry(new Vector3(-12f, 0f, 18f), floorWarm, wall, metal, lava);
            CreateConnector(new Vector3(0f, 0f, 26f), 6f, 2f, floor, wall, core);

            CreateRoom("CoreWorks", new Vector3(0f, 0f, 34f), 20f, 14f, floorWarm, wall, metal, core, 4);
            CreateConnector(new Vector3(0f, 0f, 41.5f), 7f, 1f, floor, wall, core);

            CreateBossArena(new Vector3(0f, 0f, 52f), 26f, 20f, floor, wall, metal, lava, core);

            CreateAtmosphere();
            WorldPostProcessingBuilder.ApplyAshen();
        }

        private static void CreateRoom(
            string name,
            Vector3 center,
            float width,
            float depth,
            Material floor,
            Material wall,
            Material metal,
            Material glow,
            int variant)
        {
            GameObject root = new GameObject(name);
            root.transform.position = center;

            CreateSlab(root.transform, width, depth, floor);

            CreateBlock(root.transform, "Wall_L", new Vector3(-width * 0.5f, 1.6f, 0f), new Vector3(0.8f, 3.2f, depth), wall);
            CreateBlock(root.transform, "Wall_R", new Vector3(width * 0.5f, 1.6f, 0f), new Vector3(0.8f, 3.2f, depth), wall);

            for (int i = -1; i <= 1; i += 2)
            {
                CreateBlock(root.transform, "Support", new Vector3(i * width * 0.35f, 1.8f, -depth * 0.30f), new Vector3(0.7f, 3.6f, 0.7f), metal);
                CreateBlock(root.transform, "Support", new Vector3(i * width * 0.35f, 1.8f, depth * 0.30f), new Vector3(0.7f, 3.6f, 0.7f), metal);
            }

            CreateMachine(root.transform, new Vector3(-width * 0.34f, 0.8f, 0f), metal, glow, 0.95f);
            CreateMachine(root.transform, new Vector3(width * 0.34f, 0.8f, depth * 0.20f), metal, glow, 0.85f);

            if (variant >= 2)
            {
                CreateBlock(root.transform, "OverheadBeam", new Vector3(0f, 3.8f, 0f), new Vector3(width * 0.70f, 0.45f, 0.55f), metal);
            }

            CreatePointLight(root.transform, new Vector3(0f, 3f, 0f), glow.color, 2.0f + variant * 0.22f, Mathf.Max(width, depth) * 0.62f);
        }

        private static void CreateConnector(Vector3 center, float width, float depth, Material floor, Material wall, Material glow)
        {
            GameObject root = new GameObject("ForgeConnector");
            root.transform.position = center;

            CreateSlab(root.transform, width, depth, floor);
            CreateBlock(root.transform, "Wall_L", new Vector3(-width * 0.5f, 1.4f, 0f), new Vector3(0.75f, 2.8f, depth), wall);
            CreateBlock(root.transform, "Wall_R", new Vector3(width * 0.5f, 1.4f, 0f), new Vector3(0.75f, 2.8f, depth), wall);

            CreateBlock(root.transform, "Guide_L", new Vector3(-1.6f, 0.14f, 0f), new Vector3(0.16f, 0.12f, depth * 0.8f), glow, false);
            CreateBlock(root.transform, "Guide_R", new Vector3(1.6f, 0.14f, 0f), new Vector3(0.16f, 0.12f, depth * 0.8f), glow, false);
        }

        private static void CreateSideFoundry(Vector3 center, Material floor, Material wall, Material metal, Material lava)
        {
            GameObject root = new GameObject("SideFoundry");
            root.transform.position = center;

            CreateSlab(root.transform, 7f, 8f, floor);
            CreateBlock(root.transform, "BackWall", new Vector3(0f, 1.7f, 4f), new Vector3(7f, 3.4f, 0.8f), wall);
            CreateMachine(root.transform, new Vector3(0f, 0.9f, 1.4f), metal, lava, 1.35f);
            CreatePointLight(root.transform, new Vector3(0f, 2.8f, 0f), lava.color, 2.8f, 7f);
        }

        private static void CreateLavaCanal(Vector3 center, float width, float depth, Material lava, Material metal)
        {
            GameObject root = new GameObject("LavaCanal");
            root.transform.position = center;

            CreateBlock(root.transform, "Lava", new Vector3(0f, -0.10f, 0f), new Vector3(width, 0.10f, depth), lava, false);
            CreateBlock(root.transform, "Rail_A", new Vector3(-width * 0.5f, 0.30f, 0f), new Vector3(0.28f, 0.6f, depth), metal);
            CreateBlock(root.transform, "Rail_B", new Vector3(width * 0.5f, 0.30f, 0f), new Vector3(0.28f, 0.6f, depth), metal);
        }

        private static void CreateBossArena(
            Vector3 center,
            float width,
            float depth,
            Material floor,
            Material wall,
            Material metal,
            Material lava,
            Material core)
        {
            GameObject root = new GameObject("ForgeBossArena");
            root.transform.position = center;

            CreateSlab(root.transform, width, depth, floor);
            CreateBlock(root.transform, "Wall_L", new Vector3(-width * 0.5f, 2.2f, 0f), new Vector3(1f, 4.4f, depth), wall);
            CreateBlock(root.transform, "Wall_R", new Vector3(width * 0.5f, 2.2f, 0f), new Vector3(1f, 4.4f, depth), wall);
            CreateBlock(root.transform, "Wall_N", new Vector3(0f, 2.2f, depth * 0.5f), new Vector3(width, 4.4f, 1f), wall);

            CreateLavaCanalWorld(root.transform, new Vector3(-9f, 0f, 0f), 3f, depth * 0.75f, lava, metal);
            CreateLavaCanalWorld(root.transform, new Vector3(9f, 0f, 0f), 3f, depth * 0.75f, lava, metal);

            CreateBlock(root.transform, "CoreSigil", new Vector3(0f, 0.12f, 2f), new Vector3(7f, 0.12f, 7f), core, false);

            for (int i = -2; i <= 2; i++)
            {
                CreateBlock(root.transform, "ArenaSupport", new Vector3(i * 4.2f, 2.2f, depth * 0.36f), new Vector3(0.85f, 4.4f, 0.85f), metal);
            }

            CreatePointLight(root.transform, new Vector3(0f, 4f, 1f), core.color, 3.6f, 14f);
        }

        private static void CreateLavaCanalWorld(Transform parent, Vector3 localCenter, float width, float depth, Material lava, Material metal)
        {
            CreateBlock(parent, "Lava", localCenter + new Vector3(0f, -0.10f, 0f), new Vector3(width, 0.10f, depth), lava, false);
            CreateBlock(parent, "Rail_A", localCenter + new Vector3(-width * 0.5f, 0.30f, 0f), new Vector3(0.26f, 0.6f, depth), metal);
            CreateBlock(parent, "Rail_B", localCenter + new Vector3(width * 0.5f, 0.30f, 0f), new Vector3(0.26f, 0.6f, depth), metal);
        }

        private static void CreateMachine(Transform parent, Vector3 localPos, Material metal, Material glow, float scale)
        {
            CreateBlock(parent, "MachineBase", localPos, new Vector3(1.8f, 1.5f, 1.4f) * scale, metal);
            CreateBlock(parent, "MachineCore", localPos + new Vector3(0f, 0.15f, 0.74f * scale), new Vector3(0.75f, 0.56f, 0.10f) * scale, glow, false);
            CreateBlock(parent, "Pipe_L", localPos + new Vector3(-0.72f * scale, 0.8f * scale, 0f), new Vector3(0.18f, 1.2f, 0.18f) * scale, metal);
            CreateBlock(parent, "Pipe_R", localPos + new Vector3(0.72f * scale, 0.8f * scale, 0f), new Vector3(0.18f, 1.2f, 0.18f) * scale, metal);
        }

        private static void CreateSlab(Transform parent, float width, float depth, Material material)
        {
            // Visible floor must always be walkable. Decorative overlays may remain non-collidable.
            CreateBlock(parent, "FloorSlab", new Vector3(0f, 0.055f, 0f), new Vector3(width - 0.12f, 0.08f, depth - 0.12f), material, true);
        }

        private static GameObject CreateBlock(
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

        private static void CreatePointLight(Transform parent, Vector3 localPos, Color color, float intensity, float range)
        {
            GameObject go = new GameObject("ForgeLight");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;

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
            RenderSettings.fogColor = new Color(0.26f, 0.20f, 0.17f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0042f;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.58f, 0.48f, 0.38f);
            RenderSettings.ambientEquatorColor = new Color(0.40f, 0.33f, 0.28f);
            RenderSettings.ambientGroundColor = new Color(0.25f, 0.21f, 0.19f);

            Light key = Object.FindFirstObjectByType<Light>();
            if (key != null && key.type == LightType.Directional)
            {
                key.color = new Color(1f, 0.70f, 0.42f);
                key.intensity = 1.62f;
                key.shadows = LightShadows.Soft;
            }
        }
    }
}
#endif
