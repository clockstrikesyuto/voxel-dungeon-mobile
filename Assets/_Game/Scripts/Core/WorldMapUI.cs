using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VoxelDungeon.Core
{
    public sealed class WorldMapUI : MonoBehaviour
    {
        private RectTransform root;
        private Font font;

        private readonly Color dark = new Color(0.025f, 0.035f, 0.045f, 0.98f);
        private readonly Color crypt = new Color(0.18f, 0.72f, 0.78f, 1f);
        private readonly Color forge = new Color(0.92f, 0.46f, 0.16f, 1f);
        private readonly Color voidColor = new Color(0.48f, 0.34f, 0.82f, 1f);

        public static void Show()
        {
            GameObject old = GameObject.Find("WorldMapUI");
            if (old != null)
                Destroy(old);

            GameObject go = new GameObject("WorldMapUI");
            WorldMapUI map = go.AddComponent<WorldMapUI>();
            map.Build();
            HubServiceUI.SetExternalOpen(true);
        }

        private void Build()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(transform, false);
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 105;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject safe = new GameObject("SafeArea");
            safe.transform.SetParent(canvasGo.transform, false);
            root = safe.AddComponent<RectTransform>();
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;
            safe.AddComponent<VoxelDungeon.UI.SafeAreaFitter>();

            Image bg = root.gameObject.AddComponent<Image>();
            bg.color = new Color(0.055f, 0.075f, 0.085f, 1f);

            BuildTerrain();
            BuildRoutes();
            BuildNodes();
            BuildHeader();
            BuildClose();
        }

        private void BuildTerrain()
        {
            CreateRegion(
                "CrystalRegion",
                new Vector2(0.05f, 0.16f),
                new Vector2(0.37f, 0.78f),
                new Color(0.18f, 0.42f, 0.43f, 1f),
                new Color(0.24f, 0.62f, 0.60f, 0.92f),
                -5f);

            CreateRegion(
                "ForgeRegion",
                new Vector2(0.31f, 0.23f),
                new Vector2(0.70f, 0.84f),
                new Color(0.43f, 0.26f, 0.15f, 1f),
                new Color(0.72f, 0.38f, 0.18f, 0.95f),
                3f);

            CreateRegion(
                "VoidRegion",
                new Vector2(0.64f, 0.12f),
                new Vector2(0.95f, 0.76f),
                new Color(0.34f, 0.31f, 0.42f, 1f),
                new Color(0.48f, 0.44f, 0.62f, 0.95f),
                -2f);

            for (int i = 0; i < 18; i++)
            {
                float x = 0.08f + (i % 6) * 0.045f;
                float y = 0.20f + (i / 6) * 0.16f + (i % 2) * 0.035f;
                CreateDecor(new Vector2(x, y), new Vector2(54f, 88f), crypt, 45f);
            }

            for (int i = 0; i < 16; i++)
            {
                float x = 0.39f + (i % 5) * 0.055f;
                float y = 0.29f + (i / 5) * 0.17f + (i % 2) * 0.04f;
                CreateDecor(new Vector2(x, y), new Vector2(70f, 105f), forge, i % 2 == 0 ? 0f : 45f);
            }

            for (int i = 0; i < 18; i++)
            {
                float x = 0.68f + (i % 6) * 0.042f;
                float y = 0.19f + (i / 6) * 0.15f + (i % 3) * 0.025f;
                CreateDecor(new Vector2(x, y), new Vector2(48f, 78f), voidColor, 45f);
            }
        }

        private void BuildRoutes()
        {
            CreateRoute(new Vector2(0.27f, 0.46f), new Vector2(0.45f, 0.52f), crypt, forge);
            CreateRoute(new Vector2(0.57f, 0.52f), new Vector2(0.72f, 0.43f), forge, voidColor);
        }

        private void BuildNodes()
        {
            CreateStageNode(
                "stage.crypt",
                new Vector2(0.23f, 0.46f),
                crypt,
                true,
                ProfileProgress.CrystalCryptCleared,
                "LV 1+",
                Localization.IsJapanese ? "クリスタルの欠片 / 古代遺物" : "Crystal Shard / Ancient Relic",
                Localization.IsJapanese ? "ストーンウォーデン" : "Stone Warden",
                "Mission_Test");

            bool forgeOpen = ProfileProgress.AshenForgeUnlocked;
            CreateStageNode(
                "stage.ashen",
                new Vector2(0.53f, 0.53f),
                forge,
                forgeOpen,
                ProfileProgress.AshenForgeCleared,
                "LV 3+",
                Localization.IsJapanese ? "鉄鉱石 / 炎の核" : "Iron Ore / Ember Core",
                Localization.IsJapanese ? "フォージコロッサス" : "Forge Colossus",
                "Mission_Ashen");

            bool voidOpen = ProfileProgress.VoidGardenUnlocked;
            CreateStageNode(
                "stage.void",
                new Vector2(0.77f, 0.42f),
                voidColor,
                voidOpen,
                ProfileProgress.VoidGardenCleared,
                "LV 5+",
                Localization.IsJapanese ? "月光花 / 虚空の欠片" : "Moon Bloom / Void Fragment",
                Localization.IsJapanese ? "アストラルウォーデン" : "Astral Warden",
                "Mission_Void");
        }

        private void BuildHeader()
        {
            AddText(
                Localization.T("world_map"),
                new Vector2(0.5f, 0.93f),
                new Vector2(1000f, 70f),
                42,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);

            AddText(
                Localization.T("choose_expedition"),
                new Vector2(0.5f, 0.875f),
                new Vector2(1000f, 42f),
                19,
                FontStyle.Normal,
                TextAnchor.MiddleCenter,
                new Color(0.82f, 0.86f, 0.88f));
        }

        private void BuildClose()
        {
            CreateButton(
                Localization.T("back"),
                new Vector2(0.08f, 0.08f),
                new Vector2(180f, 62f),
                new Color(0.72f, 0.78f, 0.82f),
                Close);
        }

        private void CreateStageNode(
            string stageId,
            Vector2 anchor,
            Color accent,
            bool unlocked,
            bool cleared,
            string recommended,
            string materials,
            string boss,
            string sceneName)
        {
            GameObject node = CreatePanel(
                "StageNode",
                anchor,
                new Vector2(370f, 190f),
                new Color(0.025f, 0.035f, 0.045f, unlocked ? 0.97f : 0.82f));

            Outline outline = node.AddComponent<Outline>();
            outline.effectColor = unlocked ? accent : new Color(0.36f, 0.39f, 0.42f);
            outline.effectDistance = new Vector2(3f, -3f);

            AddChildText(
                node.transform,
                Localization.StageName(stageId),
                new Vector2(18f, -12f),
                new Vector2(330f, 46f),
                25,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                unlocked ? Color.white : new Color(0.54f, 0.57f, 0.60f));

            string state = cleared
                ? Localization.T("cleared")
                : unlocked
                    ? Localization.T("unlocked")
                    : Localization.T("sealed");

            AddChildText(
                node.transform,
                state,
                new Vector2(18f, -56f),
                new Vector2(330f, 30f),
                15,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                unlocked ? accent : new Color(0.48f, 0.50f, 0.53f));

            string details =
                $"{Localization.T("recommended")}: {recommended}\n" +
                $"{Localization.T("materials")}: {materials}\n" +
                $"{Localization.T("boss")}: {boss}";

            AddChildText(
                node.transform,
                details,
                new Vector2(18f, -88f),
                new Vector2(330f, 84f),
                14,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.76f, 0.80f, 0.84f));

            if (unlocked)
            {
                Button button = node.AddComponent<Button>();
                button.targetGraphic = node.GetComponent<Image>();
                button.onClick.AddListener(() =>
                {
                    GameFlowState.Mode = PlayModeKind.Solo;
                    GameFlowState.SelectedStageId = stageId;
                    GameFlowState.SelectedStageName = Localization.StageName(stageId);
                    SceneManager.LoadScene(sceneName);
                });
            }
        }

        private void CreateRegion(string name, Vector2 min, Vector2 max, Color colorA, Color colorB, float rotation)
        {
            GameObject a = new GameObject(name);
            a.transform.SetParent(root, false);
            RectTransform rect = a.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localRotation = Quaternion.Euler(0f, 0f, rotation);

            Image image = a.AddComponent<Image>();
            image.color = colorA;
            image.raycastTarget = false;

            GameObject inner = new GameObject("InnerTerrain");
            inner.transform.SetParent(a.transform, false);
            RectTransform innerRect = inner.AddComponent<RectTransform>();
            innerRect.anchorMin = new Vector2(0.04f, 0.06f);
            innerRect.anchorMax = new Vector2(0.96f, 0.94f);
            innerRect.offsetMin = Vector2.zero;
            innerRect.offsetMax = Vector2.zero;

            Image innerImage = inner.AddComponent<Image>();
            innerImage.color = colorB;
            innerImage.raycastTarget = false;
        }

        private void CreateDecor(Vector2 anchor, Vector2 size, Color color, float rotation)
        {
            GameObject go = new GameObject("TerrainDetail");
            go.transform.SetParent(root, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.sizeDelta = size;
            rect.localRotation = Quaternion.Euler(0f, 0f, rotation);
            Image image = go.AddComponent<Image>();
            image.color = new Color(color.r, color.g, color.b, 0.25f);
            image.raycastTarget = false;
        }

        private void CreateRoute(Vector2 a, Vector2 b, Color from, Color to)
        {
            Vector2 delta = b - a;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            float length = delta.magnitude * 1920f;

            GameObject route = new GameObject("Route");
            route.transform.SetParent(root, false);
            RectTransform rect = route.AddComponent<RectTransform>();
            rect.anchorMin = (a + b) * 0.5f;
            rect.anchorMax = rect.anchorMin;
            rect.sizeDelta = new Vector2(length, 10f);
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);

            Image image = route.AddComponent<Image>();
            image.color = Color.Lerp(from, to, 0.5f);
            image.raycastTarget = false;
        }

        private GameObject CreatePanel(string name, Vector2 anchor, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(root, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;

            Image image = go.AddComponent<Image>();
            image.color = color;
            return go;
        }

        private void CreateButton(string label, Vector2 anchor, Vector2 size, Color accent, Action action)
        {
            GameObject go = CreatePanel(label + "Button", anchor, size, new Color(0.08f, 0.10f, 0.12f, 0.97f));
            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = accent;
            outline.effectDistance = new Vector2(2f, -2f);

            Button button = go.AddComponent<Button>();
            button.targetGraphic = go.GetComponent<Image>();
            if (action != null)
                button.onClick.AddListener(() => action());

            AddChildText(go.transform, label, Vector2.zero, size, 18, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
        }

        private Text AddText(string value, Vector2 anchor, Vector2 size, int fontSize, FontStyle style, TextAnchor alignment, Color color)
        {
            GameObject go = new GameObject("Text");
            go.transform.SetParent(root, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;

            Text t = go.AddComponent<Text>();
            t.font = font;
            t.text = value;
            t.fontSize = fontSize;
            t.fontStyle = style;
            t.alignment = alignment;
            t.color = color;
            t.raycastTarget = false;
            return t;
        }

        private Text AddChildText(Transform parent, string value, Vector2 anchoredPosition, Vector2 size, int fontSize, FontStyle style, TextAnchor alignment, Color color)
        {
            GameObject go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Text t = go.AddComponent<Text>();
            t.font = font;
            t.text = value;
            t.fontSize = fontSize;
            t.fontStyle = style;
            t.alignment = alignment;
            t.color = color;
            t.raycastTarget = false;
            return t;
        }

        private void Close()
        {
            HubServiceUI.SetExternalOpen(false);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            HubServiceUI.SetExternalOpen(false);
        }
    }
}
