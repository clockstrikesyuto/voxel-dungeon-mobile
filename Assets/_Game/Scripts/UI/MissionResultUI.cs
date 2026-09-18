using UnityEngine;
using UnityEngine.UI;
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
            image.color = new Color(0.02f, 0.03f, 0.05f, 0.82f);

            GameObject textGo = new GameObject("ResultText");
            textGo.transform.SetParent(panel.transform, false);
            RectTransform textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.2f, 0.25f);
            textRect.anchorMax = new Vector2(0.8f, 0.75f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text text = textGo.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 54;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            int gold = progress != null ? progress.Gold : 0;
            int loot = progress != null ? progress.LootCount : 0;
            int melee = progress != null ? progress.MeleePowerBonus : 0;
            int ranged = progress != null ? progress.RangedPowerBonus : 0;

            text.text =
                "MISSION CLEAR\n\n" +
                $"GOLD  {gold}\n" +
                $"LOOT  {loot}\n" +
                $"MELEE POWER  +{melee}\n" +
                $"RANGED POWER +{ranged}";
        }
    }
}
