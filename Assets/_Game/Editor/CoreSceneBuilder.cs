#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using VoxelDungeon.Combat;
using VoxelDungeon.AI;
using VoxelDungeon.Core;
using VoxelDungeon.Player;
using VoxelDungeon.Presentation;
using VoxelDungeon.UI;

namespace VoxelDungeon.EditorTools
{
    public static class CoreSceneBuilder
    {
        private const string SceneFolder = "Assets/_Game/Scenes";
        private const string BootPath = SceneFolder + "/Boot.unity";
        private const string CampPath = SceneFolder + "/Camp.unity";
        private const string HubPath = SceneFolder + "/Hub.unity";
        private const string MissionPath = SceneFolder + "/Mission_Test.unity";
        private const string AshenMissionPath = SceneFolder + "/Mission_Ashen.unity";
        private const string VoidMissionPath = SceneFolder + "/Mission_Void.unity";

        [MenuItem("Tools/Voxel Dungeon/Create Core Scenes")]
        public static void CreateCoreScenes()
        {
            EnsureFolder();
            CreateBoot();
            CreateCamp();
            CreateHub();
            CreateMission();
            CreateAshenMission();
            CreateVoidMission();
            ConfigureBuildScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorSceneManager.OpenScene(BootPath);
            Debug.Log("[VoxelDungeon] Created Boot, Hub, Camp, Crystal Crypt, Ashen Forge and Void Garden scenes and added them to build settings.");
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

            GameObject menu = new GameObject("MainMenu");
            menu.AddComponent<MainMenuController>();

            Camera camera = CreateCamera("Main Camera", new Vector3(0f, 4f, -8f), new Vector3(18f, 0f, 0f));
            camera.backgroundColor = new Color(0.012f, 0.02f, 0.035f);
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


        private static void CreateHub()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            CreateGround("Hub Ground", new Vector3(0f, -0.5f, 0f), new Vector3(64f, 1f, 64f));

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player_Hub";
            player.transform.position = new Vector3(0f, 1f, -3f);

            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0f, 1f, 0f);

            TopDownPlayerMotor motor = player.AddComponent<TopDownPlayerMotor>();
            HubPlayerInput input = player.AddComponent<HubPlayerInput>();
            VisualStyleBuilder.ApplyPlayerVisual(player);
            player.AddComponent<LoadoutVisualController>();

            Camera camera = CreateCamera("Main Camera", new Vector3(0f, 12f, -10f), Vector3.zero);
            camera.backgroundColor = new Color(0.58f, 0.78f, 0.90f);
            TopDownCameraFollow follow = camera.gameObject.AddComponent<TopDownCameraFollow>();
            follow.SetTarget(player.transform);
            follow.ConfigureView(new Vector3(0f, 12f, -10f), new Vector3(0f, 1f, 0f));
            camera.transform.LookAt(player.transform.position + Vector3.up);

            CreateDirectionalLight();
            HubWorldBuilder.PopulateHub(player);
            CreateHubMobileHud(input);

            EditorSceneManager.SaveScene(scene, HubPath);
        }

        private static void CreateMission()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject stageIdentity = new GameObject("StageIdentity");
            stageIdentity.AddComponent<StageSceneIdentity>().Configure("stage.crypt", "CRYSTAL CRYPT");

            CreateGround("Mission Ground", new Vector3(0f, -0.5f, 10f), new Vector3(34f, 1f, 88f));

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player_Debug";
            player.transform.position = new Vector3(0f, 1f, -31f);

            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0f, 1f, 0f);

            Health playerHealth = player.AddComponent<Health>();
            player.AddComponent<PlayerProgress>();
            TopDownPlayerMotor motor = player.AddComponent<TopDownPlayerMotor>();
            player.AddComponent<KnockbackMotor>();
            player.AddComponent<HealthDamageReceiver>();
            PlayerCombat combat = player.AddComponent<PlayerCombat>();
            DebugPlayerInput input = player.AddComponent<DebugPlayerInput>();
            VisualStyleBuilder.ApplyPlayerVisual(player);
            player.AddComponent<LoadoutVisualController>();

            Camera camera = CreateCamera("Main Camera", new Vector3(0f, 10f, -8f), Vector3.zero);
            camera.backgroundColor = new Color(0.18f, 0.28f, 0.34f);
            TopDownCameraFollow follow = camera.gameObject.AddComponent<TopDownCameraFollow>();
            follow.SetTarget(player.transform);
            camera.transform.LookAt(player.transform.position + Vector3.up);

            CreateDirectionalLight();
            CrystalCryptWorldBuilder.Build();
            CreateMobileHud(motor, combat, input, playerHealth);
            player.AddComponent<MissionHeaderUI>();
            player.AddComponent<StageJourneyUI>();

            VerticalSliceBuilder.PopulateMission(player);

            EditorSceneManager.SaveScene(scene, MissionPath);
        }



        private static void CreateAshenMission()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject stageIdentity = new GameObject("StageIdentity");
            stageIdentity.AddComponent<StageSceneIdentity>().Configure("stage.ashen", "ASHEN FORGE");

            CreateGround("Mission Ground", new Vector3(0f, -0.5f, 10f), new Vector3(34f, 1f, 88f));

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player_Debug";
            player.transform.position = new Vector3(0f, 1f, -31f);

            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0f, 1f, 0f);

            Health playerHealth = player.AddComponent<Health>();
            player.AddComponent<PlayerProgress>();
            TopDownPlayerMotor motor = player.AddComponent<TopDownPlayerMotor>();
            player.AddComponent<KnockbackMotor>();
            player.AddComponent<HealthDamageReceiver>();
            PlayerCombat combat = player.AddComponent<PlayerCombat>();
            DebugPlayerInput input = player.AddComponent<DebugPlayerInput>();
            VisualStyleBuilder.ApplyPlayerVisual(player);
            player.AddComponent<LoadoutVisualController>();

            Camera camera = CreateCamera("Main Camera", new Vector3(0f, 10f, -8f), Vector3.zero);
            camera.backgroundColor = new Color(0.30f, 0.20f, 0.15f);
            TopDownCameraFollow follow = camera.gameObject.AddComponent<TopDownCameraFollow>();
            follow.SetTarget(player.transform);
            camera.transform.LookAt(player.transform.position + Vector3.up);

            CreateDirectionalLight();
            AshenForgeWorldBuilder.Build();
            CreateMobileHud(motor, combat, input, playerHealth);
            player.AddComponent<MissionHeaderUI>();
            player.AddComponent<StageJourneyUI>();

            AshenForgeSliceBuilder.PopulateMission(player);

            EditorSceneManager.SaveScene(scene, AshenMissionPath);
        }


        private static void CreateVoidMission()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject stageIdentity = new GameObject("StageIdentity");
            stageIdentity.AddComponent<StageSceneIdentity>().Configure("stage.void", "VOID GARDEN");

            CreateGround("Mission Ground", new Vector3(0f, -0.5f, 10f), new Vector3(34f, 1f, 88f));

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player_Debug";
            player.transform.position = new Vector3(0f, 1f, -31f);

            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0f, 1f, 0f);

            Health playerHealth = player.AddComponent<Health>();
            player.AddComponent<PlayerProgress>();
            TopDownPlayerMotor motor = player.AddComponent<TopDownPlayerMotor>();
            player.AddComponent<KnockbackMotor>();
            player.AddComponent<HealthDamageReceiver>();
            PlayerCombat combat = player.AddComponent<PlayerCombat>();
            DebugPlayerInput input = player.AddComponent<DebugPlayerInput>();
            VisualStyleBuilder.ApplyPlayerVisual(player);
            player.AddComponent<LoadoutVisualController>();

            Camera camera = CreateCamera("Main Camera", new Vector3(0f, 10f, -8f), Vector3.zero);
            camera.backgroundColor = new Color(0.58f, 0.66f, 0.74f);
            TopDownCameraFollow follow = camera.gameObject.AddComponent<TopDownCameraFollow>();
            follow.SetTarget(player.transform);
            camera.transform.LookAt(player.transform.position + Vector3.up);

            CreateDirectionalLight();
            VoidGardenWorldBuilder.Build();
            CreateMobileHud(motor, combat, input, playerHealth);
            player.AddComponent<MissionHeaderUI>();
            player.AddComponent<StageJourneyUI>();

            VoidGardenSliceBuilder.PopulateMission(player);

            EditorSceneManager.SaveScene(scene, VoidMissionPath);
        }


        private static void CreateHubMobileHud(HubPlayerInput input)
        {
            GameObject canvasGo = new GameObject("HubControls");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject safeRoot = new GameObject("SafeArea_HubControls");
            safeRoot.transform.SetParent(canvasGo.transform, false);
            RectTransform safeRect = safeRoot.AddComponent<RectTransform>();
            safeRect.anchorMin = Vector2.zero;
            safeRect.anchorMax = Vector2.one;
            safeRect.offsetMin = Vector2.zero;
            safeRect.offsetMax = Vector2.zero;
            safeRoot.AddComponent<SafeAreaFitter>();

            GameObject joystickBg = CreateUiBlock(
                "HubMoveJoystick",
                safeRect,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(190f, 190f),
                new Vector2(230f, 230f),
                new Color(0.08f, 0.1f, 0.14f, 0.48f));

            GameObject joystickHandle = CreateUiBlock(
                "Handle",
                joystickBg.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(95f, 95f),
                new Color(0.85f, 0.9f, 1f, 0.82f));

            MobileJoystick joystick = joystickBg.AddComponent<MobileJoystick>();
            joystick.Configure(
                joystickBg.GetComponent<RectTransform>(),
                joystickHandle.GetComponent<RectTransform>());
            input.SetMobileJoystick(joystick);

            GameObject eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<EventSystem>();
            eventSystemGo.AddComponent<InputSystemUIInputModule>();
        }

        private static void CreateMobileHud(
            TopDownPlayerMotor motor,
            PlayerCombat combat,
            DebugPlayerInput input,
            Health playerHealth)
        {
            GameObject canvasGo = new GameObject("MobileHUD");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject safeRoot = new GameObject("SafeArea");
            safeRoot.transform.SetParent(canvasGo.transform, false);
            RectTransform safeRect = safeRoot.AddComponent<RectTransform>();
            safeRect.anchorMin = Vector2.zero;
            safeRect.anchorMax = Vector2.one;
            safeRect.offsetMin = Vector2.zero;
            safeRect.offsetMax = Vector2.zero;
            safeRoot.AddComponent<SafeAreaFitter>();

            GameObject joystickBg = CreateUiBlock(
                "MoveJoystick",
                safeRect,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(190f, 190f),
                new Vector2(230f, 230f),
                new Color(0.08f, 0.1f, 0.14f, 0.55f));

            GameObject joystickHandle = CreateUiBlock(
                "Handle",
                joystickBg.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(95f, 95f),
                new Color(0.85f, 0.9f, 1f, 0.85f));

            MobileJoystick joystick = joystickBg.AddComponent<MobileJoystick>();
            joystick.Configure(
                joystickBg.GetComponent<RectTransform>(),
                joystickHandle.GetComponent<RectTransform>());
            input.SetMobileJoystick(joystick);

            GameObject attack = CreateUiBlock(
                "AttackButton",
                safeRect,
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-155f, 185f),
                new Vector2(165f, 165f),
                new Color(1f, 0.42f, 0.15f, 0.8f));
            attack.AddComponent<MobileActionButton>().Configure(
                MobileActionButton.ActionKind.Attack,
                motor,
                combat);
            AddButtonLabel(attack, "ATK", 34);

            GameObject dodge = CreateUiBlock(
                "DodgeButton",
                safeRect,
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-345f, 115f),
                new Vector2(125f, 125f),
                new Color(0.2f, 0.65f, 1f, 0.8f));
            dodge.AddComponent<MobileActionButton>().Configure(
                MobileActionButton.ActionKind.Dodge,
                motor,
                combat);
            AddButtonLabel(dodge, "DODGE", 22);

            GameObject ranged = CreateUiBlock(
                "RangedButton",
                safeRect,
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-335f, 305f),
                new Vector2(135f, 135f),
                new Color(0.55f, 0.32f, 1f, 0.82f));
            ranged.AddComponent<MobileActionButton>().Configure(
                MobileActionButton.ActionKind.Ranged,
                motor,
                combat);
            AddButtonLabel(ranged, "RNG", 28);

            GameObject potion = CreateUiBlock(
                "PotionButton",
                safeRect,
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-520f, 205f),
                new Vector2(115f, 115f),
                new Color(0.2f, 0.85f, 0.4f, 0.82f));
            potion.AddComponent<MobileActionButton>().Configure(
                MobileActionButton.ActionKind.Potion,
                motor,
                combat);
            AddButtonLabel(potion, "+", 46);

            GameObject hpBg = CreateUiBlock(
                "HealthBar",
                safeRect,
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(200f, -75f),
                new Vector2(330f, 38f),
                new Color(0.08f, 0.08f, 0.08f, 0.85f));

            GameObject hpFill = CreateUiBlock(
                "Fill",
                hpBg.GetComponent<RectTransform>(),
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.15f, 0.9f, 0.28f, 0.95f));
            RectTransform fillRect = hpFill.GetComponent<RectTransform>();
            fillRect.offsetMin = new Vector2(4f, 4f);
            fillRect.offsetMax = new Vector2(-4f, -4f);
            hpBg.AddComponent<HealthBarUI>().Configure(playerHealth, fillRect);

            GameObject eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<EventSystem>();
            eventSystemGo.AddComponent<InputSystemUIInputModule>();
        }

        private static GameObject CreateUiBlock(
            string name,
            RectTransform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 size,
            Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);

            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            if (anchorMin == anchorMax)
                rect.sizeDelta = size;
            else
            {
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }

            Image image = go.AddComponent<Image>();
            image.color = color;

            bool round = name.Contains("Button") || name.Contains("Joystick") || name == "Handle";
            if (round)
            {
                image.sprite = VisualStyleBuilder.GetRoundSprite();
                image.preserveAspect = true;
            }

            return go;
        }

        private static void AddButtonLabel(GameObject button, string labelText, int fontSize)
        {
            GameObject labelGo = new GameObject("Label");
            labelGo.transform.SetParent(button.transform, false);

            RectTransform rect = labelGo.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Text label = labelGo.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = labelText;
            label.fontSize = fontSize;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.raycastTarget = false;

            Shadow shadow = labelGo.AddComponent<Shadow>();
            shadow.effectDistance = new Vector2(2f, -2f);
            shadow.effectColor = new Color(0f, 0f, 0f, 0.65f);
        }

        private static Camera CreateCamera(string name, Vector3 position, Vector3 euler)
        {
            GameObject go = new GameObject(name);
            go.tag = "MainCamera";
            go.transform.position = position;
            go.transform.eulerAngles = euler;

            Camera camera = go.AddComponent<Camera>();
            camera.fieldOfView = 46f;
            camera.allowHDR = true;
            camera.allowMSAA = true;
            camera.backgroundColor = new Color(0.018f, 0.03f, 0.045f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 250f;
            camera.GetUniversalAdditionalCameraData().renderPostProcessing = true;
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
                new EditorBuildSettingsScene(HubPath, true),
                new EditorBuildSettingsScene(CampPath, true),
                new EditorBuildSettingsScene(MissionPath, true),
                new EditorBuildSettingsScene(AshenMissionPath, true),
                new EditorBuildSettingsScene(VoidMissionPath, true)
            };
        }
    }
}
#endif
