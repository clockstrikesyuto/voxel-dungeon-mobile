using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Core;
using VoxelDungeon.Player;

namespace VoxelDungeon.UI
{
    [RequireComponent(typeof(PlayerProgress))]
    public sealed class ProgressHudUI : MonoBehaviour
    {
        private PlayerProgress progress;
        private Text progressionText;
        private Text inventoryText;
        private Text loadoutText;
        private float nextRefreshTime;

        private void Start()
        {
            progress = GetComponent<PlayerProgress>();
            Build();
            progress.Changed += Refresh;
            Refresh();
        }

        private void Update()
        {
            if (Time.time < nextRefreshTime)
                return;

            nextRefreshTime = Time.time + 0.25f;
            Refresh();
        }

        private void OnDestroy()
        {
            if (progress != null)
                progress.Changed -= Refresh;
        }

        private void Build()
        {
            GameObject safe = GameObject.Find("SafeArea");
            if (safe == null)
                return;

            Font font = UiFontProvider.Get();

            GameObject panel = new GameObject("ProgressText");
            panel.transform.SetParent(safe.transform, false);

            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(1f, 1f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.pivot = new Vector2(1f, 1f);
            panelRect.anchoredPosition = new Vector2(-34f, -26f);
            panelRect.sizeDelta = new Vector2(440f, 150f);

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.018f, 0.028f, 0.040f, 0.76f);

            Outline outline = panel.AddComponent<Outline>();
            outline.effectColor = new Color(0.24f, 0.48f, 0.62f, 0.55f);
            outline.effectDistance = new Vector2(2f, -2f);

            progressionText = CreateText(
                panel.transform,
                new Vector2(0.05f, 0.66f),
                new Vector2(0.95f, 0.95f),
                17,
                FontStyle.Bold,
                font,
                TextAnchor.MiddleRight,
                Color.white);

            inventoryText = CreateText(
                panel.transform,
                new Vector2(0.05f, 0.37f),
                new Vector2(0.95f, 0.66f),
                15,
                FontStyle.Bold,
                font,
                TextAnchor.MiddleRight,
                new Color(0.80f, 0.86f, 0.90f));

            loadoutText = CreateText(
                panel.transform,
                new Vector2(0.05f, 0.06f),
                new Vector2(0.95f, 0.37f),
                14,
                FontStyle.Normal,
                font,
                TextAnchor.MiddleRight,
                new Color(0.62f, 0.78f, 0.86f));
        }

        private static Text CreateText(
            Transform parent,
            Vector2 min,
            Vector2 max,
            int size,
            FontStyle style,
            Font font,
            TextAnchor anchor,
            Color color)
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
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = anchor;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private void Refresh()
        {
            if (progressionText == null || progress == null)
                return;

            var melee = ProfileProgress.EquippedMelee;
            var ranged = ProfileProgress.EquippedRanged;

            progressionText.text = Localization.IsJapanese
                ? $"LV {ProfileProgress.Level}   EXP {ProfileProgress.Experience}/{ProfileProgress.ExperienceToNextLevel}   G {progress.Gold}"
                : $"LV {ProfileProgress.Level}   EXP {ProfileProgress.Experience}/{ProfileProgress.ExperienceToNextLevel}   GOLD {progress.Gold}";

            inventoryText.text = Localization.IsJapanese
                ? $"回復 {ProfileProgress.GetConsumable("healing_potion")}   火炎 {ProfileProgress.GetConsumable("fire_bomb")}   氷結 {ProfileProgress.GetConsumable("frost_flask")}"
                : $"POTION {ProfileProgress.GetConsumable("healing_potion")}   FIRE {ProfileProgress.GetConsumable("fire_bomb")}   ICE {ProfileProgress.GetConsumable("frost_flask")}";

            loadoutText.text =
                $"{Localization.EquipmentName(melee.Id, melee.Name)}  •  {Localization.EquipmentName(ranged.Id, ranged.Name)}";
        }
    }
}
