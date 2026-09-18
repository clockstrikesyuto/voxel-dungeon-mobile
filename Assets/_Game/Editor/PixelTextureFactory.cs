#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VoxelDungeon.EditorTools
{
    public static class PixelTextureFactory
    {
        private const string Folder = "Assets/_Game/Art/PixelTextures";

        public static Texture2D GetOrCreate(string stableName, Color baseColor, bool emissive = false)
        {
            EnsureFolder();

            string safeName = stableName.Replace(" ", "_");
            string path = $"{Folder}/{safeName}.png";

            Texture2D existing = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (existing != null)
                return existing;

            const int size = 64;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = safeName;
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;

            int seed = stableName.GetHashCode();
            System.Random random = new System.Random(seed);

            Color shadow = Color.Lerp(baseColor, Color.black, emissive ? 0.08f : 0.24f);
            Color light = Color.Lerp(baseColor, Color.white, emissive ? 0.32f : 0.18f);
            Color midDark = Color.Lerp(baseColor, shadow, 0.45f);
            Color midLight = Color.Lerp(baseColor, light, 0.45f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int cellX = x / 4;
                    int cellY = y / 4;
                    int hash = (cellX * 73856093) ^ (cellY * 19349663) ^ seed;
                    int mode = Mathf.Abs(hash) % 10;

                    Color c = baseColor;
                    if (mode <= 1) c = midDark;
                    else if (mode == 2) c = shadow;
                    else if (mode == 3) c = midLight;
                    else if (mode == 4) c = light;

                    bool seamX = (x % 16 == 0 || x % 16 == 15);
                    bool seamY = (y % 16 == 0 || y % 16 == 15);

                    if (!emissive && (seamX || seamY))
                        c = Color.Lerp(c, shadow, 0.35f);

                    double noise = random.NextDouble();
                    if (noise < 0.035)
                        c = Color.Lerp(c, shadow, 0.48f);
                    else if (noise > 0.972)
                        c = Color.Lerp(c, light, 0.40f);

                    texture.SetPixel(x, y, c);
                }
            }

            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.filterMode = FilterMode.Point;
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.mipmapEnabled = true;
                importer.textureCompression = TextureImporterCompression.CompressedHQ;
                importer.maxTextureSize = 64;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Game/Art"))
                AssetDatabase.CreateFolder("Assets/_Game", "Art");
            if (!AssetDatabase.IsValidFolder(Folder))
                AssetDatabase.CreateFolder("Assets/_Game/Art", "PixelTextures");
        }
    }
}
#endif
