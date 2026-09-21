#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VoxelDungeon.EditorTools
{
    public static class PixelTextureFactory
    {
        private const string Folder = "Assets/_Game/Art/PixelTextures";
        private const int TextureSize = 128;

        public static Texture2D GetOrCreate(string stableName, Color baseColor, bool emissive = false)
        {
            EnsureFolder();

            string safeName = stableName.Replace(" ", "_");
            string path = $"{Folder}/{safeName}.png";

            Texture2D existing = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (existing != null && existing.width >= TextureSize)
                return existing;

            if (existing != null)
                AssetDatabase.DeleteAsset(path);

            Texture2D texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
            texture.name = safeName;
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;

            int seed = stableName.GetHashCode();
            System.Random random = new System.Random(seed);

            Color deep = Color.Lerp(baseColor, Color.black, emissive ? 0.10f : 0.30f);
            Color shadow = Color.Lerp(baseColor, Color.black, emissive ? 0.05f : 0.18f);
            Color light = Color.Lerp(baseColor, Color.white, emissive ? 0.38f : 0.22f);
            Color highlight = Color.Lerp(baseColor, Color.white, emissive ? 0.58f : 0.34f);

            for (int y = 0; y < TextureSize; y++)
            {
                for (int x = 0; x < TextureSize; x++)
                {
                    int macroX = x / 16;
                    int macroY = y / 16;
                    int microX = x / 4;
                    int microY = y / 4;

                    int macroHash = (macroX * 73856093) ^ (macroY * 19349663) ^ seed;
                    int microHash = (microX * 83492791) ^ (microY * 297657976) ^ (seed * 31);

                    float macro = (Mathf.Abs(macroHash) % 100) / 100f;
                    float micro = (Mathf.Abs(microHash) % 100) / 100f;

                    Color c = baseColor;

                    if (macro < 0.15f) c = Color.Lerp(c, deep, 0.48f);
                    else if (macro < 0.34f) c = Color.Lerp(c, shadow, 0.46f);
                    else if (macro > 0.87f) c = Color.Lerp(c, highlight, 0.45f);
                    else if (macro > 0.68f) c = Color.Lerp(c, light, 0.38f);

                    if (micro < 0.16f) c = Color.Lerp(c, shadow, 0.24f);
                    else if (micro > 0.84f) c = Color.Lerp(c, light, 0.20f);

                    bool majorSeamX = x % 32 == 0 || x % 32 == 31;
                    bool majorSeamY = y % 32 == 0 || y % 32 == 31;
                    bool minorSeam = (x + y + seed) % 47 == 0;

                    if (!emissive && (majorSeamX || majorSeamY))
                        c = Color.Lerp(c, deep, 0.34f);

                    if (!emissive && minorSeam)
                        c = Color.Lerp(c, shadow, 0.22f);

                    double grain = random.NextDouble();
                    if (grain < 0.022)
                        c = Color.Lerp(c, deep, 0.52f);
                    else if (grain > 0.982)
                        c = Color.Lerp(c, highlight, 0.46f);

                    if (emissive)
                    {
                        float pulseBand = Mathf.Abs(Mathf.Sin((x + y * 0.65f + seed * 0.001f) * 0.10f));
                        if (pulseBand > 0.92f)
                            c = Color.Lerp(c, highlight, 0.32f);
                    }

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
                importer.maxTextureSize = TextureSize;
                importer.anisoLevel = 4;
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
