#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace VoxelDungeon.EditorTools
{
    public static class PackageBootstrap
    {
        private static readonly Queue<string> Packages = new(new[]
        {
            "com.unity.inputsystem",
            "com.unity.netcode.gameobjects",
            "com.unity.services.multiplayer",
            "com.unity.multiplayer.playmode",
            "com.unity.multiplayer.tools"
        });

        private static AddRequest request;

        [MenuItem("Tools/Voxel Dungeon/Install Required Packages")]
        public static void Install()
        {
            if (request != null) return;
            Debug.Log("[VoxelDungeon] Installing required compatible packages...");
            AddNext();
        }

        private static void AddNext()
        {
            if (Packages.Count == 0)
            {
                EditorApplication.update -= Poll;
                request = null;
                Debug.Log("[VoxelDungeon] Package installation queue completed. Review Package Manager for any compatibility warnings.");
                return;
            }

            request = Client.Add(Packages.Dequeue());
            EditorApplication.update -= Poll;
            EditorApplication.update += Poll;
        }

        private static void Poll()
        {
            if (request == null || !request.IsCompleted) return;

            if (request.Status == StatusCode.Failure)
                Debug.LogError($"[VoxelDungeon] Package install failed: {request.Error?.message}");
            else
                Debug.Log($"[VoxelDungeon] Installed {request.Result?.packageId}");

            request = null;
            AddNext();
        }
    }
}
#endif
