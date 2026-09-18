using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace VoxelDungeon.Core
{
    public sealed class HubController : MonoBehaviour
    {
        [SerializeField] private Transform player;

        private HubPoint[] points;
        private HubPoint nearest;
        private Canvas canvas;
        private RectTransform safeRoot;
        private GameObject promptRoot;
        private GameObject overlayRoot;
        private Text promptTitle;
        private Text promptSubtitle;
        private Text profileText;
        private Button interactButton;
        private Font font;

        private readonly Color dark = new Color(0.02f, 0.035f, 0.055f, 0.96f);
        private readonly Color cyan = new Color(0.10f, 0.82f, 0.95f, 1f);
        private readonly Color violet = new Color(0.55f, 0.28f, 1f, 1f);
        private readonly Color warm = new Color(1f, 0.48f, 0.16f, 1f);

        public void SetPlayer(Transform value) => player = value;

        private void Start()
        {
            ProfileProgress.EnsureStarterEquipment();
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            points = FindObjectsByType<HubPoint>(FindObjectsSortMode.None);
            BuildUi();
            RefreshProfile();
        }

        private void Update()
        {
            if (player == null)
            {
                GameObject go = GameObject.Find("Player_Hub");
                if (go != null)
                    player = go.transform;
            }

            if (player == null || overlayRoot.activeSelf || HubServiceUI.IsOpen)
                return;

            nearest = FindNearestPoint();
            UpdatePrompt();

#if ENABLE_INPUT_SYSTEM
            if (nearest != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                Interact(nearest);
#endif
        }

        private HubPoint FindNearestPoint()
        {
            HubPoint best = null;
            float bestDistance = float.MaxValue;

            foreach (HubPoint point in points)
            {
                if (point == null)
                    continue;

                Vector3 delta = point.transform.position - player.position;
                delta.y = 0f;
                float distance = delta.magnitude;

                if (distance <= point.InteractionRadius && distance < bestDistance)
                {
                    best = point;
                    bestDistance = distance;
                }
            }

            return best;
        }

        private void UpdatePrompt()
        {
            bool visible = nearest != null;
            promptRoot.SetActive(visible);

            if (!visible)
                return;

            bool unlocked = nearest.IsUnlocked();
            promptTitle.text = nearest.DisplayName;
            promptSubtitle.text = unlocked ? nearest.Subtitle : GetLockedText(nearest.StageId);
            interactButton.interactable = unlocked;
            interactButton.GetComponentInChildren<Text>().text = unlocked ? GetActionLabel(nearest.Kind) : "LOCKED";
        }

        private string GetActionLabel(HubPointKind kind)
        {
            return kind switch
            {
                HubPointKind.StageGate => "ENTER",
                HubPointKind.WorldMap => "OPEN MAP",
                HubPointKind.Forge => "USE FORGE",
                HubPointKind.Merchant => "TALK",
                HubPointKind.Training => "TRAIN",
                HubPointKind.Armory => "OPEN ARSENAL",
                HubPointKind.Quest => "TALK",
                _ => "OPEN"
            };
        }

        private string GetLockedText(string stageId)
        {
            if (stageId == "stage.ashen")
                return "Clear CRYSTAL CRYPT to unlock";
            if (stageId == "stage.void")
                return "Clear ASHEN FORGE to unlock";
            return "Locked";
        }

        private void Interact(HubPoint point)
        {
            if (point == null || !point.IsUnlocked())
                return;

            switch (point.Kind)
            {
                case HubPointKind.StageGate:
                    StartStage(point.StageId, point.DisplayName, point.SceneName);
                    break;

                case HubPointKind.WorldMap:
                    ShowWorldMap();
                    break;

                case HubPointKind.Forge:
                    ShowForge();
                    break;

                case HubPointKind.Merchant:
                    HubServiceUI.ShowMerchant();
                    break;

                case HubPointKind.Training:
                    ShowInfoPanel(
                        "TRAINING YARD",
                        "Movement and combat practice area.\nTarget dummies and weapon trials are being expanded.");
                    break;

                case HubPointKind.Armory:
                    HubServiceUI.ShowEquipment();
                    break;

                case HubPointKind.Quest:
                    HubServiceUI.ShowArchivist();
                    break;
            }
        }

        private void StartStage(string stageId, string stageName, string sceneName)
        {
            GameFlowState.Mode = PlayModeKind.Solo;
            GameFlowState.SelectedStageId = stageId;
            GameFlowState.SelectedStageName = stageName;
            SceneManager.LoadScene(sceneName);
        }

        private void BuildUi()
        {
            GameObject canvasGo = new GameObject("HubHUD");
            canvasGo.transform.SetParent(transform, false);

            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 30;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject safe = new GameObject("SafeArea_Hub");
            safe.transform.SetParent(canvasGo.transform, false);
            safeRoot = safe.AddComponent<RectTransform>();
            safeRoot.anchorMin = Vector2.zero;
            safeRoot.anchorMax = Vector2.one;
            safeRoot.offsetMin = Vector2.zero;
            safeRoot.offsetMax = Vector2.zero;
            safe.AddComponent<VoxelDungeon.UI.SafeAreaFitter>();

            profileText = CreateText(
                safeRoot,
                "Profile",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(40f, -38f),
                new Vector2(650f, 90f),
                24,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                Color.white);

            Text title = CreateText(
                safeRoot,
                "HubTitle",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -38f),
                new Vector2(760f, 80f),
                32,
                FontStyle.Bold,
                TextAnchor.UpperCenter,
                Color.white);
            title.text = "FRONTIER HAVEN";

            promptRoot = CreatePanel(
                safeRoot,
                "InteractionPrompt",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 120f),
                new Vector2(740f, 190f),
                dark);

            promptTitle = CreateText(
                promptRoot.GetComponent<RectTransform>(),
                "Title",
                new Vector2(0.04f, 0.80f),
                new Vector2(0.65f, 0.98f),
                Vector2.zero,
                Vector2.zero,
                30,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                Color.white);

            promptSubtitle = CreateText(
                promptRoot.GetComponent<RectTransform>(),
                "Subtitle",
                new Vector2(0.04f, 0.12f),
                new Vector2(0.67f, 0.73f),
                Vector2.zero,
                Vector2.zero,
                20,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.7f, 0.77f, 0.84f));

            interactButton = CreateButton(
                promptRoot.GetComponent<RectTransform>(),
                "InteractButton",
                "ENTER",
                new Vector2(0.72f, 0.20f),
                new Vector2(0.96f, 0.80f),
                cyan,
                () => Interact(nearest));

            promptRoot.SetActive(false);

            overlayRoot = CreatePanel(
                safeRoot,
                "Overlay",
                new Vector2(0.14f, 0.12f),
                new Vector2(0.86f, 0.88f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.015f, 0.028f, 0.045f, 0.97f));
            overlayRoot.SetActive(false);
        }

        private void ShowWorldMap()
        {
            ClearOverlay();

            CreateOverlayTitle("FRONTIER MAP", "Choose your next expedition.");

            CreateMapCard(
                "01  CRYSTAL CRYPT",
                "OPEN",
                "Ancient crystal tunnels beneath the frontier.",
                cyan,
                true,
                () => StartStage("stage.crypt", "CRYSTAL CRYPT", "Mission_Test"));

            bool ashen = ProfileProgress.AshenForgeUnlocked;
            CreateMapCard(
                "02  ASHEN FORGE",
                ashen ? "UNLOCKED" : "LOCKED",
                ashen
                    ? "Molten foundries, armored raiders and the Forge Colossus."
                    : "Clear Crystal Crypt first.",
                warm,
                ashen,
                () => StartStage("stage.ashen", "ASHEN FORGE", "Mission_Ashen"));

            bool voidOpen = ProfileProgress.VoidGardenUnlocked;
            CreateMapCard(
                "03  VOID GARDEN",
                voidOpen ? "UNLOCKED" : "SEALED",
                voidOpen
                    ? "White ruins, reflecting pools and the Astral Warden."
                    : "Clear Ashen Forge first.",
                violet,
                voidOpen,
                () => StartStage("stage.void", "VOID GARDEN", "Mission_Void"));

            CreateOverlayCloseButton();
            overlayRoot.SetActive(true);
        }

        private void ShowForge()
        {
            ClearOverlay();
            CreateOverlayTitle("EMBER FORGE", $"Gold {ProfileProgress.Gold}");

            CreateForgeRow(
                "MELEE CORE",
                $"+2 permanent melee power  •  Current +{ProfileProgress.MeleePower}",
                20,
                warm,
                () =>
                {
                    ProfileProgress.UpgradeMelee(20, 2);
                    RefreshProfile();
                    ShowForge();
                });

            CreateForgeRow(
                "RANGED CORE",
                $"+2 permanent ranged power  •  Current +{ProfileProgress.RangedPower}",
                20,
                violet,
                () =>
                {
                    ProfileProgress.UpgradeRanged(20, 2);
                    RefreshProfile();
                    ShowForge();
                });

            CreateOverlayCloseButton();
            overlayRoot.SetActive(true);
        }

        private void ShowInfoPanel(string title, string body)
        {
            ClearOverlay();
            CreateOverlayTitle(title, body);
            CreateOverlayCloseButton();
            overlayRoot.SetActive(true);
        }

        private void CreateOverlayTitle(string title, string subtitle)
        {
            RectTransform root = overlayRoot.GetComponent<RectTransform>();

            Text t = CreateText(
                root, "OverlayTitle",
                new Vector2(0.08f, 0.77f),
                new Vector2(0.92f, 0.94f),
                Vector2.zero, Vector2.zero,
                42, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            t.text = title;

            Text s = CreateText(
                root, "OverlaySubtitle",
                new Vector2(0.12f, 0.63f),
                new Vector2(0.88f, 0.78f),
                Vector2.zero, Vector2.zero,
                21, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.67f, 0.74f, 0.82f));
            s.text = subtitle;
        }

        private int mapCardIndex;
        private void CreateMapCard(string title, string state, string description, Color accent, bool enabled, Action action)
        {
            RectTransform root = overlayRoot.GetComponent<RectTransform>();
            float top = 0.58f - mapCardIndex * 0.19f;

            GameObject card = CreatePanel(
                root,
                "MapCard",
                new Vector2(0.12f, top - 0.13f),
                new Vector2(0.88f, top),
                Vector2.zero,
                Vector2.zero,
                new Color(0.035f, 0.055f, 0.075f, enabled ? 0.96f : 0.52f));

            Text a = CreateText(
                card.GetComponent<RectTransform>(),
                "Name",
                new Vector2(0.04f, 0.48f),
                new Vector2(0.67f, 0.94f),
                Vector2.zero, Vector2.zero,
                26, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            a.text = title;

            Text b = CreateText(
                card.GetComponent<RectTransform>(),
                "Desc",
                new Vector2(0.04f, 0.06f),
                new Vector2(0.68f, 0.5f),
                Vector2.zero, Vector2.zero,
                17, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.66f, 0.72f, 0.79f));
            b.text = description;

            Text stateText = CreateText(
                card.GetComponent<RectTransform>(),
                "State",
                new Vector2(0.70f, 0.15f),
                new Vector2(0.96f, 0.85f),
                Vector2.zero, Vector2.zero,
                19, FontStyle.Bold, TextAnchor.MiddleCenter, enabled ? accent : new Color(0.45f, 0.48f, 0.52f));
            stateText.text = state;

            if (enabled && action != null)
            {
                Button button = card.AddComponent<Button>();
                button.targetGraphic = card.GetComponent<Image>();
                button.onClick.AddListener(() => action());
            }

            mapCardIndex++;
        }

        private int forgeRowIndex;
        private void CreateForgeRow(string title, string description, int cost, Color accent, Action action)
        {
            RectTransform root = overlayRoot.GetComponent<RectTransform>();
            float top = 0.56f - forgeRowIndex * 0.22f;

            GameObject row = CreatePanel(
                root,
                "ForgeRow",
                new Vector2(0.14f, top - 0.15f),
                new Vector2(0.86f, top),
                Vector2.zero,
                Vector2.zero,
                new Color(0.04f, 0.06f, 0.08f, 0.96f));

            Text t = CreateText(
                row.GetComponent<RectTransform>(),
                "Name",
                new Vector2(0.04f, 0.45f),
                new Vector2(0.62f, 0.92f),
                Vector2.zero, Vector2.zero,
                27, FontStyle.Bold, TextAnchor.MiddleLeft, Color.white);
            t.text = title;

            Text d = CreateText(
                row.GetComponent<RectTransform>(),
                "Description",
                new Vector2(0.04f, 0.08f),
                new Vector2(0.67f, 0.47f),
                Vector2.zero, Vector2.zero,
                17, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.68f, 0.74f, 0.8f));
            d.text = description;

            Button button = CreateButton(
                row.GetComponent<RectTransform>(),
                "Buy",
                $"{cost} GOLD",
                new Vector2(0.72f, 0.20f),
                new Vector2(0.95f, 0.80f),
                accent,
                action);
            button.interactable = ProfileProgress.Gold >= cost;

            forgeRowIndex++;
        }

        private void CreateOverlayCloseButton()
        {
            RectTransform root = overlayRoot.GetComponent<RectTransform>();
            CreateButton(
                root,
                "Close",
                "CLOSE",
                new Vector2(0.38f, 0.035f),
                new Vector2(0.62f, 0.125f),
                new Color(0.35f, 0.42f, 0.5f, 1f),
                () => overlayRoot.SetActive(false));
        }

        private void ClearOverlay()
        {
            mapCardIndex = 0;
            forgeRowIndex = 0;

            RectTransform root = overlayRoot.GetComponent<RectTransform>();
            for (int i = root.childCount - 1; i >= 0; i--)
                Destroy(root.GetChild(i).gameObject);
        }

        private void RefreshProfile()
        {
            if (profileText == null)
                return;

            profileText.text =
                $"GOLD {ProfileProgress.Gold}\n" +
                $"MELEE +{ProfileProgress.MeleePower}   RANGE +{ProfileProgress.RangedPower}";
        }

        private GameObject CreatePanel(
            RectTransform parent,
            string name,
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
            return go;
        }

        private Text CreateText(
            RectTransform parent,
            string name,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 size,
            int fontSize,
            FontStyle style,
            TextAnchor alignment,
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

            Text text = go.AddComponent<Text>();
            text.font = font;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private Button CreateButton(
            RectTransform parent,
            string name,
            string label,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Color accent,
            Action action)
        {
            GameObject go = CreatePanel(
                parent,
                name,
                anchorMin,
                anchorMax,
                Vector2.zero,
                Vector2.zero,
                new Color(accent.r * 0.30f, accent.g * 0.30f, accent.b * 0.30f, 0.97f));

            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = accent;
            outline.effectDistance = new Vector2(2f, -2f);

            Button button = go.AddComponent<Button>();
            button.targetGraphic = go.GetComponent<Image>();

            if (action != null)
                button.onClick.AddListener(() => action());

            Text text = CreateText(
                go.GetComponent<RectTransform>(),
                "Label",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                21,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            text.text = label;

            return button;
        }
    }
}
