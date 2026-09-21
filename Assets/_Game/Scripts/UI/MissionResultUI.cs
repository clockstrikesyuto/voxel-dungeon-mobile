using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoxelDungeon.Core;
using VoxelDungeon.Player;

namespace VoxelDungeon.UI
{
    public static class MissionResultUI
    {
        public static void Show(PlayerProgress progress)
        {
            HideGameplayHud();

            int gold = progress != null ? progress.Gold : MissionRunStats.GoldCollected;
            int melee = progress != null ? progress.MeleePowerBonus : 0;
            int ranged = progress != null ? progress.RangedPowerBonus : 0;
            int xp = MissionRunStats.ExperienceGained;

            ProfileProgress.AddGold(gold);
            ProfileProgress.CompleteStage(GameFlowState.SelectedStageId);

            string retryScene = SceneManager.GetActiveScene().name;
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameObject canvasGo = new GameObject("MissionResultCanvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 120;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject background = CreatePanel(
                canvasGo.transform,
                "Background",
                Vector2.zero,
                Vector2.one,
                new Color(0.012f, 0.018f, 0.026f, 0.965f));

            GameObject card = CreatePanel(
                background.transform,
                "ResultCard",
                new Vector2(0.14f, 0.10f),
                new Vector2(0.86f, 0.90f),
                new Color(0.035f, 0.050f, 0.065f, 0.985f));

            AddText(
                card.transform,
                Localization.T("mission_clear"),
                new Vector2(0.06f, 0.83f),
                new Vector2(0.94f, 0.97f),
                58,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white,
                font);

            AddText(
                card.transform,
                Localization.StageName(GameFlowState.SelectedStageId),
                new Vector2(0.08f, 0.75f),
                new Vector2(0.92f, 0.84f),
                27,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(0.18f, 0.84f, 0.95f),
                font);

            CreateStatChip(
                card.transform,
                Localization.T("gold"),
                $"+{gold}",
                new Vector2(0.08f, 0.61f),
                new Vector2(0.31f, 0.72f),
                new Color(1f, 0.72f, 0.18f),
                font);

            CreateStatChip(
                card.transform,
                Localization.T("exp"),
                $"+{xp}",
                new Vector2(0.385f, 0.61f),
                new Vector2(0.615f, 0.72f),
                new Color(0.30f, 0.82f, 1f),
                font);

            CreateStatChip(
                card.transform,
                Localization.IsJapanese ? "レベル" : "LEVEL",
                ProfileProgress.Level.ToString(),
                new Vector2(0.69f, 0.61f),
                new Vector2(0.92f, 0.72f),
                new Color(0.60f, 0.42f, 1f),
                font);

            CreateRewardBox(
                card.transform,
                Localization.T("equipment"),
                MissionRunStats.BuildEquipmentSummary(),
                new Vector2(0.08f, 0.35f),
                new Vector2(0.46f, 0.57f),
                new Color(0.56f, 0.38f, 1f),
                font);

            CreateRewardBox(
                card.transform,
                Localization.T("materials"),
                MissionRunStats.BuildMaterialSummary(),
                new Vector2(0.54f, 0.35f),
                new Vector2(0.92f, 0.57f),
                new Color(0.18f, 0.76f, 0.70f),
                font);

            string unlockStage = GameFlowState.SelectedStageId switch
            {
                "stage.crypt" => "stage.ashen",
                "stage.ashen" => "stage.void",
                _ => null
            };

            if (!string.IsNullOrEmpty(unlockStage))
            {
                GameObject unlock = CreatePanel(
                    card.transform,
                    "Unlock",
                    new Vector2(0.20f, 0.245f),
                    new Vector2(0.80f, 0.33f),
                    new Color(0.16f, 0.10f, 0.24f, 0.98f));

                Outline outline = unlock.AddComponent<Outline>();
                outline.effectColor = new Color(0.72f, 0.46f, 1f, 0.9f);
                outline.effectDistance = new Vector2(2f, -2f);

                AddText(
                    unlock.transform,
                    $"{Localization.T("new_route")}   •   {Localization.StageName(unlockStage)}",
                    Vector2.zero,
                    Vector2.one,
                    24,
                    FontStyle.Bold,
                    TextAnchor.MiddleCenter,
                    Color.white,
                    font);
            }

            if (melee > 0 || ranged > 0)
            {
                string power = Localization.IsJapanese
                    ? $"探索強化  近接 +{melee}   遠距離 +{ranged}"
                    : $"RUN POWER  MELEE +{melee}   RANGED +{ranged}";

                AddText(
                    card.transform,
                    power,
                    new Vector2(0.20f, 0.185f),
                    new Vector2(0.80f, 0.235f),
                    17,
                    FontStyle.Normal,
                    TextAnchor.MiddleCenter,
                    new Color(0.72f, 0.78f, 0.84f),
                    font);
            }

            CreateButton(
                card.transform,
                Localization.T("return_hub"),
                new Vector2(0.08f, 0.065f),
                new Vector2(0.36f, 0.155f),
                new Color(0.12f, 0.78f, 0.90f),
                font,
                () => SceneManager.LoadScene("Hub"));

            CreateButton(
                card.transform,
                Localization.T("retry"),
                new Vector2(0.39f, 0.065f),
                new Vector2(0.61f, 0.155f),
                new Color(0.20f, 0.55f, 0.72f),
                font,
                () => SceneManager.LoadScene(retryScene));

            CreateButton(
                card.transform,
                Localization.T("title"),
                new Vector2(0.64f, 0.065f),
                new Vector2(0.92f, 0.155f),
                new Color(0.50f, 0.28f, 0.90f),
                font,
                () =>
                {
                    GameFlowState.ResetToTitle();
                    SceneManager.LoadScene("Boot");
                });
        }

        private static void CreateStatChip(
            Transform parent,
            string label,
            string value,
            Vector2 min,
            Vector2 max,
            Color accent,
            Font font)
        {
            GameObject box = CreatePanel(
                parent,
                "Stat",
                min,
                max,
                new Color(0.055f, 0.07f, 0.085f, 0.98f));

            Outline outline = box.AddComponent<Outline>();
            outline.effectColor = new Color(accent.r, accent.g, accent.b, 0.65f);
            outline.effectDistance = new Vector2(2f, -2f);

            AddText(
                box.transform,
                label,
                new Vector2(0.06f, 0.52f),
                new Vector2(0.94f, 0.90f),
                16,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(0.70f, 0.76f, 0.82f),
                font);

            AddText(
                box.transform,
                value,
                new Vector2(0.06f, 0.08f),
                new Vector2(0.94f, 0.58f),
                30,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                accent,
                font);
        }

        private static void CreateRewardBox(
            Transform parent,
            string title,
            string body,
            Vector2 min,
            Vector2 max,
            Color accent,
            Font font)
        {
            GameObject box = CreatePanel(
                parent,
                "Reward",
                min,
                max,
                new Color(0.045f, 0.060f, 0.075f, 0.98f));

            Outline outline = box.AddComponent<Outline>();
            outline.effectColor = new Color(accent.r, accent.g, accent.b, 0.5f);
            outline.effectDistance = new Vector2(2f, -2f);

            AddText(
                box.transform,
                title,
                new Vector2(0.07f, 0.72f),
                new Vector2(0.93f, 0.94f),
                19,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                accent,
                font);

            AddText(
                box.transform,
                body,
                new Vector2(0.07f, 0.08f),
                new Vector2(0.93f, 0.70f),
                17,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                Color.white,
                font);
        }

        private static void HideGameplayHud()
        {
            string[] names =
            {
                "MobileHUD",
                "MissionHeader",
                "ProgressText",
                "JourneyBanner",
                "BossHealthUI"
            };

            foreach (string name in names)
            {
                GameObject go = GameObject.Find(name);
                if (go != null)
                    go.SetActive(false);
            }
        }

        private static GameObject CreatePanel(
            Transform parent,
            string name,
            Vector2 min,
            Vector2 max,
            Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image image = go.AddComponent<Image>();
            image.color = color;
            return go;
        }

        private static Text AddText(
            Transform parent,
            string value,
            Vector2 min,
            Vector2 max,
            int fontSize,
            FontStyle style,
            TextAnchor alignment,
            Color color,
            Font font)
        {
            GameObject go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Text text = go.AddComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private static void CreateButton(
            Transform parent,
            string label,
            Vector2 min,
            Vector2 max,
            Color accent,
            Font font,
            UnityEngine.Events.UnityAction action)
        {
            GameObject go = CreatePanel(
                parent,
                label + "Button",
                min,
                max,
                new Color(accent.r * 0.28f, accent.g * 0.28f, accent.b * 0.28f, 0.98f));

            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = accent;
            outline.effectDistance = new Vector2(2f, -2f);

            Button button = go.AddComponent<Button>();
            button.targetGraphic = go.GetComponent<Image>();
            button.onClick.AddListener(action);

            AddText(
                go.transform,
                label,
                Vector2.zero,
                Vector2.one,
                21,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white,
                font);
        }
    }
}
