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
            GameObject canvasGo = new GameObject("MissionResultCanvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(canvasGo.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image image = panel.AddComponent<Image>();
            image.color = new Color(0.015f, 0.025f, 0.04f, 0.9f);

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            Text title = CreateText(
                panel.transform,
                "MISSION CLEAR",
                new Vector2(0.5f, 0.74f),
                new Vector2(1000f, 120f),
                64,
                FontStyle.Bold,
                Color.white,
                font);

            CreateText(
                panel.transform,
                GameFlowState.SelectedStageName,
                new Vector2(0.5f, 0.655f),
                new Vector2(800f, 55f),
                24,
                FontStyle.Bold,
                new Color(0.12f, 0.82f, 0.95f),
                font);

            int gold = progress != null ? progress.Gold : 0;
            int loot = progress != null ? progress.LootCount : 0;
            int melee = progress != null ? progress.MeleePowerBonus : 0;
            int ranged = progress != null ? progress.RangedPowerBonus : 0;

            ProfileProgress.AddGold(gold);
            ProfileProgress.CompleteStage(GameFlowState.SelectedStageId);

            string unlockLine = GameFlowState.SelectedStageId == "stage.crypt"
                ? "\nNEW ROUTE  ASHEN FORGE UNLOCKED"
                : string.Empty;

            string body =
                $"GOLD   {gold}\n" +
                $"LOOT   {loot}\n" +
                $"MELEE POWER   +{melee}\n" +
                $"RANGED POWER  +{ranged}" +
                unlockLine;

            CreateText(
                panel.transform,
                body,
                new Vector2(0.5f, 0.47f),
                new Vector2(760f, 260f),
                38,
                FontStyle.Normal,
                Color.white,
                font);

            CreateButton(
                panel.transform,
                "RETURN HUB",
                new Vector2(0.34f, 0.16f),
                new Vector2(300f, 78f),
                new Color(0.12f, 0.78f, 0.90f),
                font,
                () => SceneManager.LoadScene("Hub"));

            CreateButton(
                panel.transform,
                "RETRY",
                new Vector2(0.50f, 0.16f),
                new Vector2(260f, 78f),
                new Color(0.2f, 0.55f, 0.72f),
                font,
                () => SceneManager.LoadScene("Mission_Test"));

            CreateButton(
                panel.transform,
                "TITLE",
                new Vector2(0.66f, 0.16f),
                new Vector2(260f, 78f),
                new Color(0.5f, 0.28f, 0.9f),
                font,
                () =>
                {
                    GameFlowState.ResetToTitle();
                    SceneManager.LoadScene("Boot");
                });
        }

        private static Text CreateText(
            Transform parent,
            string value,
            Vector2 anchor,
            Vector2 size,
            int fontSize,
            FontStyle style,
            Color color,
            Font font)
        {
            GameObject go = new GameObject("Text");
            go.transform.SetParent(parent, false);

            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;

            Text text = go.AddComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private static void CreateButton(
            Transform parent,
            string label,
            Vector2 anchor,
            Vector2 size,
            Color accent,
            Font font,
            UnityEngine.Events.UnityAction action)
        {
            GameObject go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);

            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;

            Image bg = go.AddComponent<Image>();
            bg.color = new Color(accent.r * 0.3f, accent.g * 0.3f, accent.b * 0.3f, 0.96f);

            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = accent;
            outline.effectDistance = new Vector2(2f, -2f);

            Button button = go.AddComponent<Button>();
            button.targetGraphic = bg;
            button.onClick.AddListener(action);

            CreateText(
                go.transform,
                label,
                new Vector2(0.5f, 0.5f),
                size,
                25,
                FontStyle.Bold,
                Color.white,
                font);
        }
    }
}
