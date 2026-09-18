#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoxelDungeon.Combat;
using VoxelDungeon.AI;
using VoxelDungeon.Core;
using VoxelDungeon.Player;
using VoxelDungeon.Presentation;

namespace VoxelDungeon.EditorTools
{
    public static class CoreSceneBuilder
    {
        private const string SceneFolder = "Assets/_Game/Scenes";
        private const string BootPath = SceneFolder + "/Boot.unity";
        private const string CampPath = SceneFolder + "/Camp.unity";
        private const string MissionPath = SceneFolder + "/Mission_Test.unity";

        [MenuItem("Tools/Voxel Dungeon/Create Core Scenes")]
        public static void CreateCoreScenes()
        {
            EnsureFolder();
            CreateBoot();
            CreateCamp();
            CreateMission();
            ConfigureBuildScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(BootPath);
            Debug.Log("[VoxelDungeon] Created Boot, Camp and Mission_Test scenes and added them to build settings.");
        }

        private static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/_Game"))
                AssetDatabase.CreateFolder("Assets", "_Game");
            if (!AssetDatabase.IsValidFolder(SceneFolder))
                AssetDatabase.CreateFolder("Assets/_Game", "Scenes");
        }

        private static void CreateBoot()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject bootstrap = new GameObject("GameBootstrap");
            bootstrap.AddComponent<GameBootstrap>();

            CreateCamera("Main Camera", new Vector3(0f, 4f, -8f), new Vector3(18f, 0f, 0f));
            CreateDirectionalLight();

            EditorSceneManager.SaveScene(scene, BootPath);
        }

        private static void CreateCamp()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateGround("Camp Ground", new Vector3(0f, -0.5f, 0f), new Vector3(18f, 1f, 18f));

            for (int x = -2; x <= 2; x += 2)
            {
                GameObject prop = GameObject.CreatePrimitive(PrimitiveType.Cube);
                prop.name = "Camp_Block_" + x;
                prop.transform.position = new Vector3(x, 0.5f, 3f);
                prop.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            Camera camera = CreateCamera("Main Camera", new Vector3(0f, 9f, -7f), Vector3.zero);
            camera.transform.LookAt(Vector3.up);
            CreateDirectionalLight();

            EditorSceneManager.SaveScene(scene, CampPath);
        }

        private static void CreateMission()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateGround("Mission Ground", new Vector3(0f, -0.5f, 0f), new Vector3(28f, 1f, 28f));

            for (int i = 0; i < 10; i++)
            {
                GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.name = "Dungeon_Block_" + i;
                float angle = i * Mathf.PI * 2f / 10f;
                block.transform.position = new Vector3(Mathf.Cos(angle) * 8f, 0.5f, Mathf.Sin(angle) * 8f);
                block.transform.localScale = new Vector3(1.2f, 1f + (i % 3) * 0.5f, 1.2f);
            }

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player_Debug";
            player.transform.position = new Vector3(0f, 1f, 0f);

            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0f, 1f, 0f);

            player.AddComponent<TopDownPlayerMotor>();
            player.AddComponent<PlayerCombat>();
            player.AddComponent<DebugPlayerInput>();
            player.AddComponent<Health>();
            player.AddComponent<HealthDamageReceiver>();

            Camera camera = CreateCamera("Main Camera", new Vector3(0f, 9f, -7f), Vector3.zero);
            TopDownCameraFollow follow = camera.gameObject.AddComponent<TopDownCameraFollow>();
            follow.SetTarget(player.transform);
            camera.transform.LookAt(player.transform.position + Vector3.up);

            CreateDirectionalLight();

            Vector3[] enemyPositions =
            {
                new Vector3(4f, 1f, 2f),
                new Vector3(-4f, 1f, 3f),
                new Vector3(2f, 1f, 6f)
            };

            for (int i = 0; i < enemyPositions.Length; i++)
            {
                GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                enemy.name = "Enemy_Debug_" + i;
                enemy.transform.position = enemyPositions[i];

                Object.DestroyImmediate(enemy.GetComponent<CapsuleCollider>());
                CharacterController enemyController = enemy.AddComponent<CharacterController>();
                enemyController.height = 2f;
                enemyController.radius = 0.5f;
                enemyController.center = new Vector3(0f, 1f, 0f);

                enemy.AddComponent<Health>();
                enemy.AddComponent<HealthDamageReceiver>();
                SimpleEnemyBrain brain = enemy.AddComponent<SimpleEnemyBrain>();
                brain.SetTarget(player.transform);
            }

            EditorSceneManager.SaveScene(scene, MissionPath);
        }

        private static Camera CreateCamera(string name, Vector3 position, Vector3 euler)
        {
            GameObject go = new GameObject(name);
            go.tag = "MainCamera";
            go.transform.position = position;
            go.transform.eulerAngles = euler;

            Camera camera = go.AddComponent<Camera>();
            camera.fieldOfView = 48f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 250f;
            go.AddComponent<AudioListener>();
            return camera;
        }

        private static void CreateDirectionalLight()
        {
            GameObject go = new GameObject("Directional Light");
            Light light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            go.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        }

        private static GameObject CreateGround(string name, Vector3 position, Vector3 scale)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = name;
            ground.transform.position = position;
            ground.transform.localScale = scale;
            return ground;
        }

        private static void ConfigureBuildScenes()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(BootPath, true),
                new EditorBuildSettingsScene(CampPath, true),
                new EditorBuildSettingsScene(MissionPath, true)
            };
        }
    }
}
#endif
