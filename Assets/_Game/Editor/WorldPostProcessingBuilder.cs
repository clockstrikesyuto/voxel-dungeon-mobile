#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VoxelDungeon.EditorTools
{
    public static class WorldPostProcessingBuilder
    {
        private const string ArtFolder = "Assets/_Game/Art";
        private const string HubProfilePath = ArtFolder + "/HubBrightLook.asset";
        private const string CryptProfilePath = ArtFolder + "/CryptBrightLook.asset";

        public static void ApplyHub()
        {
            VolumeProfile profile = GetOrCreate(HubProfilePath);

            Bloom bloom = GetOrAdd<Bloom>(profile);
            bloom.active = true;
            bloom.intensity.Override(0.20f);
            bloom.threshold.Override(1.05f);
            bloom.scatter.Override(0.55f);

            ColorAdjustments color = GetOrAdd<ColorAdjustments>(profile);
            color.active = true;
            color.postExposure.Override(0.34f);
            color.contrast.Override(5f);
            color.saturation.Override(10f);

            WhiteBalance balance = GetOrAdd<WhiteBalance>(profile);
            balance.active = true;
            balance.temperature.Override(6f);
            balance.tint.Override(1f);

            Vignette vignette = GetOrAdd<Vignette>(profile);
            vignette.active = true;
            vignette.intensity.Override(0.06f);
            vignette.smoothness.Override(0.75f);

            Tonemapping tone = GetOrAdd<Tonemapping>(profile);
            tone.active = true;
            tone.mode.Override(TonemappingMode.ACES);

            CreateVolume("HubPostProcessing", profile);
        }

        public static void ApplyCrypt()
        {
            VolumeProfile profile = GetOrCreate(CryptProfilePath);

            Bloom bloom = GetOrAdd<Bloom>(profile);
            bloom.active = true;
            bloom.intensity.Override(0.38f);
            bloom.threshold.Override(0.88f);
            bloom.scatter.Override(0.68f);

            ColorAdjustments color = GetOrAdd<ColorAdjustments>(profile);
            color.active = true;
            color.postExposure.Override(0.12f);
            color.contrast.Override(8f);
            color.saturation.Override(8f);

            WhiteBalance balance = GetOrAdd<WhiteBalance>(profile);
            balance.active = true;
            balance.temperature.Override(-4f);
            balance.tint.Override(3f);

            Vignette vignette = GetOrAdd<Vignette>(profile);
            vignette.active = true;
            vignette.intensity.Override(0.10f);
            vignette.smoothness.Override(0.72f);

            Tonemapping tone = GetOrAdd<Tonemapping>(profile);
            tone.active = true;
            tone.mode.Override(TonemappingMode.ACES);

            CreateVolume("CryptPostProcessing", profile);
        }

        private static VolumeProfile GetOrCreate(string path)
        {
            if (!AssetDatabase.IsValidFolder(ArtFolder))
                AssetDatabase.CreateFolder("Assets/_Game", "Art");

            VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(profile, path);
            }

            EditorUtility.SetDirty(profile);
            return profile;
        }

        private static T GetOrAdd<T>(VolumeProfile profile) where T : VolumeComponent
        {
            T component;
            if (!profile.TryGet(out component))
                component = profile.Add<T>(true);
            return component;
        }

        private static void CreateVolume(string name, VolumeProfile profile)
        {
            GameObject old = GameObject.Find(name);
            if (old != null)
                Object.DestroyImmediate(old);

            GameObject go = new GameObject(name);
            Volume volume = go.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 20f;
            volume.sharedProfile = profile;
        }
    }
}
#endif
