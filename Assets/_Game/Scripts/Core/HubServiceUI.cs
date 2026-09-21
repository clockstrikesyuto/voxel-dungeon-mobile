using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Items;

namespace VoxelDungeon.Core
{
    public sealed class HubServiceUI : MonoBehaviour
    {
        public static bool IsOpen { get; private set; }

        public static void SetExternalOpen(bool value)
        {
            IsOpen = value;
        }

        private Canvas canvas;
        private RectTransform panel;
        private Font font;
        private readonly Color dark = new Color(0.015f, 0.028f, 0.045f, 0.98f);
        private readonly Color cyan = new Color(0.12f, 0.82f, 0.95f);
        private readonly Color violet = new Color(0.62f, 0.30f, 1f);
        private readonly Color warm = new Color(1f, 0.48f, 0.12f);

        public static void ShowEquipment()
        {
            AdventureInventoryUI.Show();
        }

        public static void ShowMerchant()
        {
            Create().BuildMerchant();
        }

        public static void ShowArchivist()
        {
            Create().BuildArchivist();
        }

        private static HubServiceUI Create()
        {
            GameObject old = GameObject.Find("HubServiceUI");
            if (old != null)
                Destroy(old);

            GameObject go = new GameObject("HubServiceUI");
            HubServiceUI ui = go.AddComponent<HubServiceUI>();
            ui.Initialize();
            IsOpen = true;
            return ui;
        }

        private void Initialize()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(transform, false);
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 95;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject blocker = new GameObject("Blocker");
            blocker.transform.SetParent(canvasGo.transform, false);
            RectTransform blockerRect = blocker.AddComponent<RectTransform>();
            blockerRect.anchorMin = Vector2.zero;
            blockerRect.anchorMax = Vector2.one;
            blockerRect.offsetMin = Vector2.zero;
            blockerRect.offsetMax = Vector2.zero;
            Image blockerImage = blocker.AddComponent<Image>();
            blockerImage.color = new Color(0f, 0f, 0f, 0.62f);

            GameObject panelGo = new GameObject("Panel");
            panelGo.transform.SetParent(canvasGo.transform, false);
            panel = panelGo.AddComponent<RectTransform>();
            panel.anchorMin = new Vector2(0.17f, 0.10f);
            panel.anchorMax = new Vector2(0.83f, 0.90f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;

            Image panelImage = panelGo.AddComponent<Image>();
            panelImage.color = dark;

            Outline outline = panelGo.AddComponent<Outline>();
            outline.effectColor = new Color(0.22f, 0.50f, 0.65f, 0.85f);
            outline.effectDistance = new Vector2(2f, -2f);
        }

        private void BuildEquipment()
        {
            AddHeader("ARSENAL", "Equip weapons collected during expeditions.");

            AddSectionTitle("MELEE", 0.72f);
            BuildEquipmentRows(ProfileProgress.GetOwnedEquipment(EquipmentSlot.Melee), 0.65f);

            AddSectionTitle("RANGED", 0.40f);
            BuildEquipmentRows(ProfileProgress.GetOwnedEquipment(EquipmentSlot.Ranged), 0.33f);

            AddClose();
        }

        private void BuildEquipmentRows(List<EquipmentRecord> items, float top)
        {
            int max = Mathf.Min(4, items.Count);

            for (int i = 0; i < max; i++)
            {
                EquipmentRecord item = items[i];
                float y = top - i * 0.072f;

                GameObject row = AddPanel(
                    new Vector2(0.08f, y - 0.058f),
                    new Vector2(0.92f, y),
                    new Color(0.035f, 0.055f, 0.075f, 0.96f));

                Color rarity = EquipmentCatalog.GetRarityColor(item.Rarity);

                AddText(row.transform, item.Name, new Vector2(0.03f, 0.50f), new Vector2(0.46f, 0.95f), 22, FontStyle.Bold, TextAnchor.MiddleLeft, rarity);
                AddText(row.transform, $"POWER +{item.Power}   {item.Trait}", new Vector2(0.03f, 0.05f), new Vector2(0.62f, 0.50f), 15, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.68f, 0.75f, 0.82f));

                bool equipped = item.Slot == EquipmentSlot.Melee
                    ? ProfileProgress.EquippedMeleeId == item.Id
                    : ProfileProgress.EquippedRangedId == item.Id;

                CreateButton(
                    row.transform,
                    equipped ? "EQUIPPED" : "EQUIP",
                    new Vector2(0.72f, 0.18f),
                    new Vector2(0.95f, 0.82f),
                    equipped ? new Color(0.28f, 0.48f, 0.36f) : rarity,
                    () =>
                    {
                        ProfileProgress.Equip(item.Id);
                        Rebuild(BuildEquipment);
                    },
                    !equipped);
            }
        }

        private void BuildMerchant()
        {
            AddHeader("FRONTIER MERCHANT", $"Gold {ProfileProgress.Gold}  •  Curated frontier equipment");

            BuildShopItem("crystal_saber", 55, 0.66f);
            BuildShopItem("crystal_bow", 55, 0.54f);

            if (ProfileProgress.AshenForgeUnlocked)
            {
                BuildShopItem("ember_axe", 95, 0.42f);
                BuildShopItem("ember_repeater", 115, 0.30f);
            }

            AddText(
                panel,
                "Boss-only equipment cannot be purchased.",
                new Vector2(0.12f, 0.15f),
                new Vector2(0.88f, 0.21f),
                17,
                FontStyle.Normal,
                TextAnchor.MiddleCenter,
                new Color(0.58f, 0.65f, 0.72f));

            AddClose();
        }

        private void BuildShopItem(string id, int cost, float top)
        {
            EquipmentRecord item = EquipmentCatalog.Get(id);
            GameObject row = AddPanel(
                new Vector2(0.10f, top - 0.095f),
                new Vector2(0.90f, top),
                new Color(0.04f, 0.06f, 0.08f, 0.96f));

            Color rarity = EquipmentCatalog.GetRarityColor(item.Rarity);
            AddText(row.transform, item.Name, new Vector2(0.04f, 0.50f), new Vector2(0.58f, 0.94f), 25, FontStyle.Bold, TextAnchor.MiddleLeft, rarity);
            AddText(row.transform, $"POWER +{item.Power}  •  {item.Trait}", new Vector2(0.04f, 0.08f), new Vector2(0.64f, 0.50f), 16, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.68f, 0.74f, 0.82f));

            bool owned = ProfileProgress.OwnsEquipment(id);
            bool affordable = ProfileProgress.Gold >= cost;

            CreateButton(
                row.transform,
                owned ? "OWNED" : $"{cost} GOLD",
                new Vector2(0.70f, 0.18f),
                new Vector2(0.95f, 0.82f),
                owned ? new Color(0.28f, 0.48f, 0.36f) : warm,
                () =>
                {
                    if (ProfileProgress.BuyEquipment(id, cost))
                        Rebuild(BuildMerchant);
                },
                !owned && affordable);
        }

        private void BuildArchivist()
        {
            string title = "ARCHIVIST LUMA";
            string body;
            string buttonLabel = null;
            Action action = null;

            if (!ProfileProgress.CrystalCryptCleared)
            {
                body = "The crystals beneath the frontier are reacting to something deeper.\nClear CRYSTAL CRYPT and return to me.";
            }
            else if (!ProfileProgress.AshenForgeCleared)
            {
                body = "You found the first resonance. The forge is amplifying it.\nClear ASHEN FORGE and bring back its core reading.";
            }
            else if (!ProfileProgress.ArchivistRewardClaimed)
            {
                body = "You mapped both resonance points. Take this prototype before entering the Void route.\nReward: 80 GOLD + VOID STAFF.";
                buttonLabel = "CLAIM REWARD";
                action = () =>
                {
                    ProfileProgress.ClaimArchivistReward();
                    Rebuild(BuildArchivist);
                };
            }
            else
            {
                body = "The Void Garden route is stable enough to enter.\nYour next expedition waits beyond the violet gate.";
            }

            AddHeader(title, "Frontier research quest");
            AddText(panel, body, new Vector2(0.13f, 0.38f), new Vector2(0.87f, 0.68f), 27, FontStyle.Normal, TextAnchor.MiddleCenter, Color.white);

            if (buttonLabel != null)
            {
                CreateButton(
                    panel,
                    buttonLabel,
                    new Vector2(0.34f, 0.22f),
                    new Vector2(0.66f, 0.31f),
                    violet,
                    action,
                    true);
            }

            AddClose();
        }

        private void AddHeader(string title, string subtitle)
        {
            AddText(panel, title, new Vector2(0.08f, 0.84f), new Vector2(0.92f, 0.96f), 40, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            AddText(panel, subtitle, new Vector2(0.10f, 0.78f), new Vector2(0.90f, 0.85f), 19, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.65f, 0.73f, 0.82f));
        }

        private void AddSectionTitle(string value, float y)
        {
            AddText(panel, value, new Vector2(0.08f, y), new Vector2(0.30f, y + 0.05f), 19, FontStyle.Bold, TextAnchor.MiddleLeft, cyan);
        }

        private void AddClose()
        {
            CreateButton(
                panel,
                "CLOSE",
                new Vector2(0.39f, 0.035f),
                new Vector2(0.61f, 0.105f),
                new Color(0.36f, 0.44f, 0.52f),
                Close,
                true);
        }

        private GameObject AddPanel(Vector2 min, Vector2 max, Color color)
        {
            GameObject go = new GameObject("Row");
            go.transform.SetParent(panel, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image image = go.AddComponent<Image>();
            image.color = color;
            return go;
        }

        private Text AddText(Transform parent, string value, Vector2 min, Vector2 max, int size, FontStyle style, TextAnchor alignment, Color color)
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
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private Button CreateButton(Transform parent, string label, Vector2 min, Vector2 max, Color accent, Action action, bool interactable)
        {
            GameObject go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = go.AddComponent<Image>();
            image.color = new Color(accent.r * 0.30f, accent.g * 0.30f, accent.b * 0.30f, 0.98f);

            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = accent;
            outline.effectDistance = new Vector2(2f, -2f);

            Button button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.interactable = interactable;
            if (action != null)
                button.onClick.AddListener(() => action());

            AddText(go.transform, label, Vector2.zero, Vector2.one, 19, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            return button;
        }

        private void Rebuild(Action builder)
        {
            for (int i = panel.childCount - 1; i >= 0; i--)
                Destroy(panel.GetChild(i).gameObject);

            builder();
        }

        private void Close()
        {
            IsOpen = false;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            IsOpen = false;
        }
    }
}
