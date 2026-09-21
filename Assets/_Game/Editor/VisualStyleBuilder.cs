#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VoxelDungeon.EditorTools
{
    public static class VisualStyleBuilder
    {
        private const string ArtFolder = "Assets/_Game/Art";
        private const string MatFolder = "Assets/_Game/Art/Materials";
        private const string UiFolder = "Assets/_Game/Art/UI";
        private const string RoundSpritePath = "Assets/_Game/Art/UI/soft_circle.png";
        private const string VolumeProfilePath = "Assets/_Game/Art/MissionLook.asset";

        public static void EnsureArtFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Game/Art"))
                AssetDatabase.CreateFolder("Assets/_Game", "Art");
            if (!AssetDatabase.IsValidFolder(MatFolder))
                AssetDatabase.CreateFolder("Assets/_Game/Art", "Materials");
            if (!AssetDatabase.IsValidFolder(UiFolder))
                AssetDatabase.CreateFolder("Assets/_Game/Art", "UI");
        }

        public static Material GetMaterial(string name, Color color, float metallic = 0f, float smoothness = 0.35f, bool emissive = false)
        {
            EnsureArtFolders();
            string path = $"{MatFolder}/{name}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null)
                    shader = Shader.Find("Standard");

                mat = new Material(shader);
                mat.name = name;
                AssetDatabase.CreateAsset(mat, path);
            }

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);

            Texture2D pixelTexture = PixelTextureFactory.GetOrCreate(name, color, emissive);
            if (mat.HasProperty("_BaseMap"))
            {
                mat.SetTexture("_BaseMap", pixelTexture);
                mat.SetTextureScale("_BaseMap", GetTextureTiling(name));
            }
            else if (mat.HasProperty("_MainTex"))
            {
                mat.SetTexture("_MainTex", pixelTexture);
                mat.SetTextureScale("_MainTex", GetTextureTiling(name));
            }

            if (mat.HasProperty("_Metallic"))
                mat.SetFloat("_Metallic", metallic);
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", smoothness);

            if (emissive && mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", color * 2.2f);
            }

            EditorUtility.SetDirty(mat);
            return mat;
        }

        private static Vector2 GetTextureTiling(string name)
        {
            if (name.Contains("Floor") || name.Contains("Grass") || name.Contains("Path"))
                return new Vector2(5f, 5f);

            if (name.Contains("Stone") || name.Contains("Wood"))
                return new Vector2(2.5f, 2.5f);

            return Vector2.one;
        }

        public static Sprite GetRoundSprite()
        {
            EnsureArtFolders();

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(RoundSpritePath);
            if (sprite != null)
                return sprite;

            const int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.name = "SoftCircle";

            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.47f;
            float soft = size * 0.035f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = 1f - Mathf.SmoothStep(radius - soft, radius + soft, d);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            tex.Apply();
            File.WriteAllBytes(RoundSpritePath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(RoundSpritePath, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(RoundSpritePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(RoundSpritePath);
        }

        public static void ApplyPlayerVisual(GameObject root)
        {
            HidePrimitiveRenderer(root);

            Material skin = GetMaterial("Player_Skin", new Color(0.93f, 0.72f, 0.54f), 0f, 0.25f);
            Material coat = GetMaterial("Player_Coat", new Color(0.10f, 0.34f, 0.42f), 0.04f, 0.42f);
            Material cloth = GetMaterial("Player_Cloth", new Color(0.16f, 0.46f, 0.52f), 0f, 0.32f);
            Material trim = GetMaterial("Player_Trim", new Color(0.16f, 0.84f, 0.88f), 0.05f, 0.52f, true);
            Material dark = GetMaterial("Player_Dark", new Color(0.05f, 0.07f, 0.09f), 0.15f, 0.38f);
            Material leather = GetMaterial("Player_Leather", new Color(0.28f, 0.14f, 0.07f), 0.02f, 0.30f);
            Material steel = GetMaterial("Player_Sword", new Color(0.76f, 0.88f, 0.92f), 0.78f, 0.72f);
            Material gold = GetMaterial("Player_Gold", new Color(0.92f, 0.68f, 0.20f), 0.62f, 0.58f);
            Material hair = GetMaterial("Player_Hair", new Color(0.10f, 0.075f, 0.06f), 0f, 0.28f);
            Material eye = GetMaterial("Player_Eye", new Color(0.06f, 0.12f, 0.15f), 0f, 0.32f);

            GameObject visual = NewVisualRoot(root, "PlayerVisual");

            CreateCube(visual.transform, "Body", new Vector3(0f, 0.95f, 0f), new Vector3(0.80f, 0.88f, 0.54f), coat);
            CreateCube(visual.transform, "ChestPanel", new Vector3(0f, 1.06f, 0.292f), new Vector3(0.56f, 0.40f, 0.05f), cloth);
            CreateCube(visual.transform, "ChestTrim", new Vector3(0f, 1.15f, 0.323f), new Vector3(0.60f, 0.09f, 0.04f), trim);
            CreateCube(visual.transform, "Belt", new Vector3(0f, 0.63f, 0.01f), new Vector3(0.86f, 0.13f, 0.58f), leather);
            CreateCube(visual.transform, "Buckle", new Vector3(0f, 0.63f, 0.315f), new Vector3(0.15f, 0.15f, 0.05f), gold);

            CreateCube(visual.transform, "Head", new Vector3(0f, 1.64f, 0f), new Vector3(0.60f, 0.60f, 0.60f), skin);
            CreateCube(visual.transform, "HairTop", new Vector3(0f, 1.96f, -0.02f), new Vector3(0.64f, 0.17f, 0.64f), hair);
            CreateCube(visual.transform, "HairFront_L", new Vector3(-0.17f, 1.83f, 0.315f), new Vector3(0.18f, 0.20f, 0.05f), hair);
            CreateCube(visual.transform, "HairFront_R", new Vector3(0.10f, 1.86f, 0.315f), new Vector3(0.16f, 0.16f, 0.05f), hair);
            CreateCube(visual.transform, "Eye_L", new Vector3(-0.15f, 1.66f, 0.315f), new Vector3(0.07f, 0.07f, 0.04f), eye);
            CreateCube(visual.transform, "Eye_R", new Vector3(0.15f, 1.66f, 0.315f), new Vector3(0.07f, 0.07f, 0.04f), eye);
            CreateCube(visual.transform, "Mouth", new Vector3(0f, 1.51f, 0.315f), new Vector3(0.12f, 0.035f, 0.035f), leather);

            CreateCube(visual.transform, "Shoulder_L", new Vector3(-0.52f, 1.23f, 0f), new Vector3(0.30f, 0.24f, 0.38f), steel);
            CreateCube(visual.transform, "Shoulder_R", new Vector3(0.52f, 1.23f, 0f), new Vector3(0.30f, 0.24f, 0.38f), steel);
            CreateCube(visual.transform, "Arm_L", new Vector3(-0.52f, 0.93f, 0f), new Vector3(0.22f, 0.64f, 0.24f), coat);
            CreateCube(visual.transform, "Arm_R", new Vector3(0.52f, 0.93f, 0f), new Vector3(0.22f, 0.64f, 0.24f), coat);
            CreateCube(visual.transform, "Glove_L", new Vector3(-0.52f, 0.58f, 0.02f), new Vector3(0.24f, 0.20f, 0.26f), leather);
            CreateCube(visual.transform, "Glove_R", new Vector3(0.52f, 0.58f, 0.02f), new Vector3(0.24f, 0.20f, 0.26f), leather);

            CreateCube(visual.transform, "Leg_L", new Vector3(-0.22f, 0.38f, 0f), new Vector3(0.28f, 0.62f, 0.32f), dark);
            CreateCube(visual.transform, "Leg_R", new Vector3(0.22f, 0.38f, 0f), new Vector3(0.28f, 0.62f, 0.32f), dark);
            CreateCube(visual.transform, "Boot_L", new Vector3(-0.22f, 0.13f, 0.08f), new Vector3(0.31f, 0.20f, 0.46f), leather);
            CreateCube(visual.transform, "Boot_R", new Vector3(0.22f, 0.13f, 0.08f), new Vector3(0.31f, 0.20f, 0.46f), leather);

            CreateCube(visual.transform, "Cape", new Vector3(0f, 0.98f, -0.33f), new Vector3(0.66f, 0.90f, 0.08f), cloth);

            GameObject sword = CreateCube(visual.transform, "SwordBlade", new Vector3(0.72f, 1.06f, 0.30f), new Vector3(0.13f, 0.88f, 0.10f), steel);
            sword.transform.localRotation = Quaternion.Euler(-18f, 0f, -18f);
            CreateCube(visual.transform, "SwordGuard", new Vector3(0.58f, 0.69f, 0.24f), new Vector3(0.38f, 0.10f, 0.12f), gold);
            CreateCube(visual.transform, "SwordGrip", new Vector3(0.52f, 0.55f, 0.20f), new Vector3(0.15f, 0.24f, 0.16f), leather);

            root.AddComponent<SimpleVisualBob>().Configure(visual.transform, 0.040f, 6f);
        }

        public static void ApplyEnemyVisual(GameObject root, string archetype)
        {
            HidePrimitiveRenderer(root);
            GameObject visual = NewVisualRoot(root, "EnemyVisual");

            Material dark = GetMaterial("Enemy_Dark", new Color(0.05f, 0.06f, 0.08f), 0.18f, 0.38f);

            if (archetype == "Scout")
            {
                Material body = GetMaterial("Enemy_Scout", new Color(0.10f, 0.58f, 0.66f), 0f, 0.30f);
                Material cloth = GetMaterial("Enemy_ScoutCloth", new Color(0.08f, 0.30f, 0.36f), 0f, 0.24f);
                Material glow = GetMaterial("Enemy_ScoutGlow", new Color(0.15f, 0.95f, 1f), 0f, 0.46f, true);
                Material blade = GetMaterial("Enemy_ScoutBlade", new Color(0.58f, 0.92f, 0.94f), 0.68f, 0.66f);

                CreateCube(visual.transform, "Body", new Vector3(0f, 0.90f, 0f), new Vector3(0.62f, 0.74f, 0.46f), body);
                CreateCube(visual.transform, "Sash", new Vector3(0f, 0.95f, 0.25f), new Vector3(0.52f, 0.13f, 0.05f), glow);
                CreateCube(visual.transform, "Hood", new Vector3(0f, 1.56f, -0.02f), new Vector3(0.66f, 0.56f, 0.58f), cloth);
                CreateCube(visual.transform, "FaceVoid", new Vector3(0f, 1.53f, 0.30f), new Vector3(0.40f, 0.24f, 0.05f), dark);
                CreateCube(visual.transform, "Eye_L", new Vector3(-0.10f, 1.57f, 0.335f), new Vector3(0.06f, 0.055f, 0.03f), glow);
                CreateCube(visual.transform, "Eye_R", new Vector3(0.10f, 1.57f, 0.335f), new Vector3(0.06f, 0.055f, 0.03f), glow);
                CreateCube(visual.transform, "Arm_L", new Vector3(-0.46f, 0.90f, 0f), new Vector3(0.17f, 0.62f, 0.20f), cloth);
                CreateCube(visual.transform, "Arm_R", new Vector3(0.46f, 0.90f, 0f), new Vector3(0.17f, 0.62f, 0.20f), cloth);
                CreateCube(visual.transform, "Leg_L", new Vector3(-0.18f, 0.28f, 0f), new Vector3(0.22f, 0.54f, 0.26f), dark);
                CreateCube(visual.transform, "Leg_R", new Vector3(0.18f, 0.28f, 0f), new Vector3(0.22f, 0.54f, 0.26f), dark);

                GameObject bladeL = CreateCube(visual.transform, "Blade_L", new Vector3(-0.58f, 0.83f, 0.20f), new Vector3(0.10f, 0.72f, 0.08f), blade);
                bladeL.transform.localRotation = Quaternion.Euler(-10f, 0f, 12f);
                GameObject bladeR = CreateCube(visual.transform, "Blade_R", new Vector3(0.58f, 0.83f, 0.20f), new Vector3(0.10f, 0.72f, 0.08f), blade);
                bladeR.transform.localRotation = Quaternion.Euler(-10f, 0f, -12f);
            }
            else if (archetype == "Raider")
            {
                Material body = GetMaterial("Enemy_Raider", new Color(0.58f, 0.18f, 0.08f), 0.08f, 0.30f);
                Material accent = GetMaterial("Enemy_RaiderAccent", new Color(1f, 0.46f, 0.12f), 0.05f, 0.38f, true);
                Material metal = GetMaterial("Enemy_RaiderMetal", new Color(0.30f, 0.32f, 0.34f), 0.60f, 0.46f);
                Material leather = GetMaterial("Enemy_RaiderLeather", new Color(0.25f, 0.11f, 0.05f), 0f, 0.26f);

                CreateCube(visual.transform, "Body", new Vector3(0f, 0.95f, 0f), new Vector3(0.88f, 0.92f, 0.64f), body);
                CreateCube(visual.transform, "ChestArmor", new Vector3(0f, 1.05f, 0.35f), new Vector3(0.62f, 0.48f, 0.10f), metal);
                CreateCube(visual.transform, "Belt", new Vector3(0f, 0.62f, 0f), new Vector3(0.90f, 0.13f, 0.66f), leather);
                CreateCube(visual.transform, "Helmet", new Vector3(0f, 1.66f, 0f), new Vector3(0.72f, 0.58f, 0.64f), metal);
                CreateCube(visual.transform, "Visor", new Vector3(0f, 1.66f, 0.335f), new Vector3(0.44f, 0.12f, 0.05f), accent);
                CreateCube(visual.transform, "Shoulder_L", new Vector3(-0.60f, 1.18f, 0f), new Vector3(0.34f, 0.30f, 0.44f), metal);
                CreateCube(visual.transform, "Shoulder_R", new Vector3(0.60f, 1.18f, 0f), new Vector3(0.34f, 0.30f, 0.44f), metal);
                CreateCube(visual.transform, "Arm_L", new Vector3(-0.57f, 0.86f, 0f), new Vector3(0.22f, 0.60f, 0.24f), body);
                CreateCube(visual.transform, "Arm_R", new Vector3(0.57f, 0.86f, 0f), new Vector3(0.22f, 0.60f, 0.24f), body);
                CreateCube(visual.transform, "Leg_L", new Vector3(-0.22f, 0.28f, 0f), new Vector3(0.28f, 0.56f, 0.32f), dark);
                CreateCube(visual.transform, "Leg_R", new Vector3(0.22f, 0.28f, 0f), new Vector3(0.28f, 0.56f, 0.32f), dark);

                GameObject axeHandle = CreateCube(visual.transform, "AxeHandle", new Vector3(0.72f, 0.90f, 0.20f), new Vector3(0.10f, 0.86f, 0.10f), leather);
                axeHandle.transform.localRotation = Quaternion.Euler(0f, 0f, -18f);
                CreateCube(visual.transform, "AxeHead", new Vector3(0.90f, 1.28f, 0.20f), new Vector3(0.34f, 0.30f, 0.10f), metal);
            }
            else
            {
                Material body = GetMaterial("Enemy_Brute", new Color(0.30f, 0.14f, 0.44f), 0.12f, 0.28f);
                Material crystal = GetMaterial("Enemy_BruteCrystal", new Color(0.74f, 0.30f, 1f), 0f, 0.50f, true);
                Material stone = GetMaterial("Enemy_Stone", new Color(0.20f, 0.21f, 0.23f), 0.06f, 0.24f);

                CreateCube(visual.transform, "Body", new Vector3(0f, 0.92f, 0f), new Vector3(1.04f, 1.02f, 0.82f), stone);
                CreateCube(visual.transform, "ChestPlate", new Vector3(0f, 1.04f, 0.43f), new Vector3(0.62f, 0.54f, 0.11f), body);
                CreateCube(visual.transform, "Head", new Vector3(0f, 1.76f, 0f), new Vector3(0.76f, 0.64f, 0.68f), stone);
                CreateCube(visual.transform, "Jaw", new Vector3(0f, 1.51f, 0.33f), new Vector3(0.44f, 0.18f, 0.10f), body);
                CreateCube(visual.transform, "Core", new Vector3(0f, 1.05f, 0.51f), new Vector3(0.28f, 0.28f, 0.08f), crystal);
                CreateCube(visual.transform, "Shoulder_L", new Vector3(-0.70f, 1.24f, 0f), new Vector3(0.46f, 0.42f, 0.55f), stone);
                CreateCube(visual.transform, "Shoulder_R", new Vector3(0.70f, 1.24f, 0f), new Vector3(0.46f, 0.42f, 0.55f), stone);
                CreateCube(visual.transform, "Fist_L", new Vector3(-0.82f, 0.72f, 0.08f), new Vector3(0.54f, 0.54f, 0.54f), stone);
                CreateCube(visual.transform, "Fist_R", new Vector3(0.82f, 0.72f, 0.08f), new Vector3(0.54f, 0.54f, 0.54f), stone);
                CreateCube(visual.transform, "Leg_L", new Vector3(-0.28f, 0.25f, 0f), new Vector3(0.34f, 0.50f, 0.38f), stone);
                CreateCube(visual.transform, "Leg_R", new Vector3(0.28f, 0.25f, 0f), new Vector3(0.34f, 0.50f, 0.38f), stone);

                GameObject shardL = CreateCube(visual.transform, "CrystalShoulder_L", new Vector3(-0.72f, 1.58f, -0.08f), new Vector3(0.16f, 0.70f, 0.16f), crystal);
                shardL.transform.localRotation = Quaternion.Euler(0f, 0f, -22f);
                GameObject shardR = CreateCube(visual.transform, "CrystalShoulder_R", new Vector3(0.72f, 1.58f, -0.08f), new Vector3(0.16f, 0.70f, 0.16f), crystal);
                shardR.transform.localRotation = Quaternion.Euler(0f, 0f, 22f);
                CreateCube(visual.transform, "BackCrystal", new Vector3(0f, 1.55f, -0.48f), new Vector3(0.22f, 0.95f, 0.22f), crystal);
            }

            root.AddComponent<SimpleVisualBob>().Configure(
                visual.transform,
                archetype == "Scout" ? 0.065f : 0.032f,
                archetype == "Scout" ? 8f : 4f);
        }

        public static void ApplyBossVisual(GameObject root)
        {
            HidePrimitiveRenderer(root);
            GameObject visual = NewVisualRoot(root, "BossVisual");

            Material stone = GetMaterial("Boss_Stone", new Color(0.13f, 0.15f, 0.19f), 0.22f, 0.28f);
            Material stoneLight = GetMaterial("Boss_StoneLight", new Color(0.28f, 0.31f, 0.35f), 0.18f, 0.34f);
            Material metal = GetMaterial("Boss_Metal", new Color(0.32f, 0.31f, 0.35f), 0.68f, 0.48f);
            Material glow = GetMaterial("Boss_Core", new Color(1f, 0.12f, 0.07f), 0f, 0.54f, true);
            Material violet = GetMaterial("Boss_VioletCore", new Color(0.62f, 0.26f, 1f), 0f, 0.50f, true);

            CreateCube(visual.transform, "Torso", new Vector3(0f, 1.02f, 0f), new Vector3(1.38f, 1.30f, 1.02f), stone);
            CreateCube(visual.transform, "ChestPlate", new Vector3(0f, 1.14f, 0.55f), new Vector3(0.92f, 0.80f, 0.13f), metal);
            CreateCube(visual.transform, "ChestInset", new Vector3(0f, 1.18f, 0.64f), new Vector3(0.52f, 0.46f, 0.08f), stoneLight);
            CreateCube(visual.transform, "Core", new Vector3(0f, 1.18f, 0.70f), new Vector3(0.34f, 0.34f, 0.07f), glow);

            CreateCube(visual.transform, "Head", new Vector3(0f, 1.98f, 0f), new Vector3(1.0f, 0.78f, 0.84f), stone);
            CreateCube(visual.transform, "Brow", new Vector3(0f, 2.06f, 0.45f), new Vector3(0.72f, 0.16f, 0.08f), stoneLight);
            CreateCube(visual.transform, "Eye", new Vector3(0f, 1.96f, 0.47f), new Vector3(0.48f, 0.11f, 0.06f), glow);
            CreateCube(visual.transform, "Jaw", new Vector3(0f, 1.66f, 0.34f), new Vector3(0.62f, 0.22f, 0.16f), metal);

            GameObject hornL = CreateCube(visual.transform, "Horn_L", new Vector3(-0.55f, 2.47f, 0f), new Vector3(0.20f, 0.70f, 0.20f), metal);
            hornL.transform.localRotation = Quaternion.Euler(0f, 0f, -28f);
            GameObject hornR = CreateCube(visual.transform, "Horn_R", new Vector3(0.55f, 2.47f, 0f), new Vector3(0.20f, 0.70f, 0.20f), metal);
            hornR.transform.localRotation = Quaternion.Euler(0f, 0f, 28f);

            CreateCube(visual.transform, "Shoulder_L", new Vector3(-0.92f, 1.35f, 0f), new Vector3(0.68f, 0.56f, 0.76f), metal);
            CreateCube(visual.transform, "Shoulder_R", new Vector3(0.92f, 1.35f, 0f), new Vector3(0.68f, 0.56f, 0.76f), metal);
            CreateCube(visual.transform, "Arm_L", new Vector3(-1.02f, 0.90f, 0f), new Vector3(0.52f, 0.98f, 0.62f), stone);
            CreateCube(visual.transform, "Arm_R", new Vector3(1.02f, 0.90f, 0f), new Vector3(0.52f, 0.98f, 0.62f), stone);
            CreateCube(visual.transform, "Gauntlet_L", new Vector3(-1.05f, 0.40f, 0.10f), new Vector3(0.62f, 0.42f, 0.68f), metal);
            CreateCube(visual.transform, "Gauntlet_R", new Vector3(1.05f, 0.40f, 0.10f), new Vector3(0.62f, 0.42f, 0.68f), metal);

            CreateCube(visual.transform, "Leg_L", new Vector3(-0.38f, 0.12f, 0f), new Vector3(0.46f, 0.60f, 0.52f), stone);
            CreateCube(visual.transform, "Leg_R", new Vector3(0.38f, 0.12f, 0f), new Vector3(0.46f, 0.60f, 0.52f), stone);

            GameObject backL = CreateCube(visual.transform, "BackCrystal_L", new Vector3(-0.42f, 1.80f, -0.62f), new Vector3(0.20f, 1.15f, 0.20f), violet);
            backL.transform.localRotation = Quaternion.Euler(12f, 0f, -18f);
            GameObject backR = CreateCube(visual.transform, "BackCrystal_R", new Vector3(0.42f, 1.80f, -0.62f), new Vector3(0.20f, 1.15f, 0.20f), violet);
            backR.transform.localRotation = Quaternion.Euler(12f, 0f, 18f);

            root.AddComponent<SimpleVisualBob>().Configure(visual.transform, 0.020f, 2.2f);
        }

        public static void DecorateMission()
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.035f, 0.055f, 0.07f);
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.018f;
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.09f, 0.12f, 0.16f);

            Material floor = GetMaterial("Floor_Stone", new Color(0.075f, 0.10f, 0.12f), 0.05f, 0.28f);
            Material tile = GetMaterial("Floor_Tile", new Color(0.105f, 0.15f, 0.17f), 0.05f, 0.35f);
            Material pillar = GetMaterial("Pillar_Stone", new Color(0.11f, 0.12f, 0.145f), 0.10f, 0.25f);
            Material crystal = GetMaterial("Crystal_Cyan", new Color(0.10f, 0.82f, 0.95f), 0f, 0.45f, true);
            Material crystal2 = GetMaterial("Crystal_Violet", new Color(0.56f, 0.23f, 1f), 0f, 0.45f, true);

            GameObject ground = GameObject.Find("Mission Ground");
            if (ground != null)
            {
                Renderer r = ground.GetComponent<Renderer>();
                if (r != null) r.sharedMaterial = floor;
            }

            for (int x = -6; x <= 6; x += 3)
            {
                for (int z = -6; z <= 6; z += 3)
                {
                    GameObject plate = CreateCube(null, $"FloorPlate_{x}_{z}", new Vector3(x, 0.015f, z), new Vector3(2.72f, 0.03f, 2.72f), tile);
                    plate.transform.position = new Vector3(x, 0.015f, z);
                    Object.DestroyImmediate(plate.GetComponent<Collider>());
                }
            }

            Vector3[] pillarPositions =
            {
                new Vector3(-9f, 1.6f, -9f), new Vector3(9f, 1.6f, -9f),
                new Vector3(-9f, 1.6f, 9f), new Vector3(9f, 1.6f, 9f),
                new Vector3(-9f, 1.6f, 0f), new Vector3(9f, 1.6f, 0f)
            };

            for (int i = 0; i < pillarPositions.Length; i++)
            {
                GameObject p = CreateCube(null, $"RuinedPillar_{i}", pillarPositions[i], new Vector3(0.9f, 3.2f + (i % 2) * 0.8f, 0.9f), pillar);
                p.transform.position = pillarPositions[i];
            }

            CreateCrystalCluster(new Vector3(-7.2f, 0.45f, 5.4f), crystal);
            CreateCrystalCluster(new Vector3(7.2f, 0.45f, 5.4f), crystal2);
            CreateCrystalCluster(new Vector3(-6.4f, 0.45f, -6.0f), crystal2);
            CreateCrystalCluster(new Vector3(6.4f, 0.45f, -6.0f), crystal);

            Light key = Object.FindFirstObjectByType<Light>();
            if (key != null)
            {
                key.color = new Color(0.72f, 0.82f, 1f);
                key.intensity = 1.05f;
                key.shadows = LightShadows.Soft;
            }

            CreatePointLight("CyanGlow", new Vector3(-7f, 2f, 5f), new Color(0.1f, 0.8f, 1f), 3.2f, 7f);
            CreatePointLight("VioletGlow", new Vector3(7f, 2f, 5f), new Color(0.6f, 0.2f, 1f), 3.0f, 7f);
            CreatePostProcessing();
        }

        private static void CreatePostProcessing()
        {
            VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, VolumeProfilePath);
            }

            Bloom bloom;
            if (!profile.TryGet(out bloom))
                bloom = profile.Add<Bloom>(true);
            bloom.active = true;
            bloom.intensity.Override(0.42f);
            bloom.threshold.Override(0.9f);
            bloom.scatter.Override(0.65f);

            Vignette vignette;
            if (!profile.TryGet(out vignette))
                vignette = profile.Add<Vignette>(true);
            vignette.active = true;
            vignette.intensity.Override(0.18f);
            vignette.smoothness.Override(0.7f);

            ColorAdjustments color;
            if (!profile.TryGet(out color))
                color = profile.Add<ColorAdjustments>(true);
            color.active = true;
            color.contrast.Override(10f);
            color.saturation.Override(6f);
            color.postExposure.Override(-0.05f);

            EditorUtility.SetDirty(profile);

            GameObject go = new GameObject("GlobalPostProcessing");
            Volume volume = go.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 10f;
            volume.sharedProfile = profile;
        }

        private static void CreateCrystalCluster(Vector3 pos, Material mat)
        {
            GameObject a = CreateCube(null, "Crystal", pos, new Vector3(0.28f, 1.15f, 0.28f), mat);
            a.transform.position = pos;
            a.transform.rotation = Quaternion.Euler(0f, 20f, 12f);

            GameObject b = CreateCube(null, "Crystal", pos + new Vector3(0.35f, -0.1f, 0.18f), new Vector3(0.20f, 0.78f, 0.20f), mat);
            b.transform.position = pos + new Vector3(0.35f, -0.1f, 0.18f);
            b.transform.rotation = Quaternion.Euler(0f, -30f, -10f);
        }

        private static void CreatePointLight(string name, Vector3 pos, Color color, float intensity, float range)
        {
            GameObject go = new GameObject(name);
            go.transform.position = pos;
            Light l = go.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = color;
            l.intensity = intensity;
            l.range = range;
            l.shadows = LightShadows.None;
        }

        private static GameObject NewVisualRoot(GameObject root, string name)
        {
            GameObject old = GameObject.Find($"{root.name}/{name}");
            if (old != null)
                Object.DestroyImmediate(old);

            GameObject visual = new GameObject(name);
            visual.transform.SetParent(root.transform, false);
            // CharacterController transform is already the actor's ground origin.
            // Keep visual geometry on the same origin so feet do not sink below floors.
            visual.transform.localPosition = Vector3.zero;
            return visual;
        }

        private static void HidePrimitiveRenderer(GameObject root)
        {
            Renderer renderer = root.GetComponent<Renderer>();
            if (renderer != null)
                renderer.enabled = false;
        }

        private static GameObject CreateCube(Transform parent, string name, Vector3 localPos, Vector3 scale, Material material)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;

            if (parent != null)
            {
                go.transform.SetParent(parent, false);
                go.transform.localPosition = localPos;
            }
            else
            {
                go.transform.position = localPos;
            }

            go.transform.localScale = scale;

            Collider c = go.GetComponent<Collider>();
            if (c != null)
                Object.DestroyImmediate(c);

            Renderer r = go.GetComponent<Renderer>();
            if (r != null)
                r.sharedMaterial = material;

            return go;
        }
    }
}
#endif
