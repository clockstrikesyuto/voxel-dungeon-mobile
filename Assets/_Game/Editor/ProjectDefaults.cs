#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VoxelDungeon.EditorTools
{
    public static class ProjectDefaults
    {
        [MenuItem("Tools/Voxel Dungeon/Apply Mobile Defaults")]
        public static void Apply()
        {
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.runInBackground = true;
            PlayerSettings.companyName = "ClockStrikesYuto";
            PlayerSettings.productName = "Voxel Dungeon Mobile";
            PlayerSettings.bundleVersion = "0.0.1";
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
            AssetDatabase.SaveAssets();
            Debug.Log("[VoxelDungeon] Applied mobile-first defaults. Bundle identifiers and signing remain platform-specific setup items.");
        }
    }
}
#endif
