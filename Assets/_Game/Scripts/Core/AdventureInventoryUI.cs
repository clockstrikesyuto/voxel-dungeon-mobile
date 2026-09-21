using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Items;

namespace VoxelDungeon.Core
{
    public sealed class AdventureInventoryUI : MonoBehaviour
    {
        private RectTransform panel;
        private Font font;

        private readonly Color dark = new Color(0.012f, 0.024f, 0.038f, 0.985f);
        private readonly Color cyan = new Color(0.12f, 0.82f, 0.95f);
        private readonly Color warm = new Color(1f, 0.48f, 0.12f);
        private readonly Color violet = new Color(0.62f, 0.30f, 1f);

        public static void Show()
        {
            GameObject old = GameObject.Find("AdventureInventoryUI");
            if (old != null)
                Destroy(old);

            GameObject go = new GameObject("AdventureInventoryUI");
            AdventureInventoryUI ui = go.AddComponent<AdventureInventoryUI>();
            ui.Build();
            HubServiceUI.SetExternalOpen(true);
        }

        private void Build()
        {
            ProfileProgress.EnsureStarterEquipment();
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(transform, false);
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 98;

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
            blockerImage.color = new Color(0f, 0f, 0f, 0.70f);

            GameObject panelGo = new GameObject("Panel");
            panelGo.transform.SetParent(canvasGo.transform, false);
            panel = panelGo.AddComponent<RectTransform>();
            panel.anchorMin = new Vector2(0.05f, 0.055f);
            panel.anchorMax = new Vector2(0.95f, 0.945f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;
            panelGo.AddComponent<Image>().color = dark;

            AddText(
                panel,
                "LOADOUT & INVENTORY",
                new Vector2(0.04f, 0.905f),
                new Vector2(0.60f, 0.98f),
                34,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                Color.white);

            AddText(
                panel,
                $"LV {ProfileProgress.Level}   EXP {ProfileProgress.Experience}/{ProfileProgress.ExperienceToNextLevel}   GOLD {ProfileProgress.Gold}   SKILL {ProfileProgress.SkillPoints}",
                new Vector2(0.04f, 0.855f),
                new Vector2(0.72f, 0.91f),
                20,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.72f, 0.80f, 0.88f));

            EquipmentSlot[] slots =
            {
                EquipmentSlot.Melee,
                EquipmentSlot.Ranged,
                EquipmentSlot.Head,
                EquipmentSlot.Body,
                EquipmentSlot.Boots,
                EquipmentSlot.Accessory
            };

            for (int i = 0; i < slots.Length; i++)
                BuildSlotRow(slots[i], 0.80f - i * 0.112f);

            BuildRightPanel();
            BuildClose();
        }

        private void BuildSlotRow(EquipmentSlot slot, float top)
        {
            EquipmentRecord item = ProfileProgress.GetEquipped(slot);
            EquipmentRoll roll = ProfileProgress.GetEquipmentRoll(item.Id);

            GameObject row = CreatePanel(
                panel,
                new Vector2(0.04f, top - 0.093f),
                new Vector2(0.60f, top),
                new Color(0.035f, 0.052f, 0.071f, 0.98f));

            Color rarity = EquipmentCatalog.GetRarityColor(item.Rarity);

            AddText(
                row.GetComponent<RectTransform>(),
                SlotLabel(slot),
                new Vector2(0.025f, 0.52f),
                new Vector2(0.18f, 0.94f),
                16,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.52f, 0.62f, 0.72f));

            AddText(
                row.GetComponent<RectTransform>(),
                item.Name,
                new Vector2(0.18f, 0.50f),
                new Vector2(0.68f, 0.96f),
                22,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                rarity);

            string stats =
                $"PWR {item.Power + roll.PowerBonus}   DEF {item.Defense + roll.DefenseBonus}   HP +{item.Vitality + roll.VitalityBonus}   CRIT +{Mathf.RoundToInt((item.CritChance + roll.CritBonus) * 100f)}%";

            AddText(
                row.GetComponent<RectTransform>(),
                stats + "   •   " + item.Trait,
                new Vector2(0.18f, 0.06f),
                new Vector2(0.80f, 0.50f),
                14,
                FontStyle.Normal,
                TextAnchor.MiddleLeft,
                new Color(0.68f, 0.75f, 0.82f));

            CreateButton(
                row.transform,
                "NEXT",
                new Vector2(0.82f, 0.17f),
                new Vector2(0.97f, 0.83f),
                rarity,
                () => EquipNext(slot));
        }

        private void EquipNext(EquipmentSlot slot)
        {
            List<EquipmentRecord> owned = ProfileProgress.GetOwnedEquipment(slot);
            if (owned.Count <= 1)
                return;

            string current = ProfileProgress.GetEquippedId(slot);
            int index = owned.FindIndex(x => x.Id == current);
            int next = (index + 1 + owned.Count) % owned.Count;

            ProfileProgress.Equip(owned[next].Id);
            Rebuild();
        }

        private void BuildRightPanel()
        {
            GameObject info = CreatePanel(
                panel,
                new Vector2(0.635f, 0.40f),
                new Vector2(0.96f, 0.84f),
                new Color(0.028f, 0.044f, 0.061f, 0.98f));

            AddText(
                info.GetComponent<RectTransform>(),
                "EXPEDITION BAG",
                new Vector2(0.06f, 0.84f),
                new Vector2(0.94f, 0.97f),
                22,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                cyan);

            string inventory =
                $"Crystal Shard   {ProfileProgress.GetMaterial("crystal_shard")}\n" +
                $"Iron Ore        {ProfileProgress.GetMaterial("iron_ore")}\n" +
                $"Ember Core      {ProfileProgress.GetMaterial("ember_core")}\n" +
                $"Moon Bloom      {ProfileProgress.GetMaterial("moon_bloom")}\n" +
                $"Void Fragment   {ProfileProgress.GetMaterial("void_fragment")}\n" +
                $"Ancient Relic   {ProfileProgress.GetMaterial("ancient_relic")}\n\n" +
                $"Healing Potion  {ProfileProgress.GetConsumable("healing_potion")}";

            AddText(
                info.GetComponent<RectTransform>(),
                inventory,
                new Vector2(0.06f, 0.08f),
                new Vector2(0.94f, 0.82f),
                18,
                FontStyle.Normal,
                TextAnchor.UpperLeft,
                new Color(0.82f, 0.86f, 0.90f));

            GameObject skills = CreatePanel(
                panel,
                new Vector2(0.635f, 0.17f),
                new Vector2(0.96f, 0.38f),
                new Color(0.028f, 0.044f, 0.061f, 0.98f));

            AddText(
                skills.GetComponent<RectTransform>(),
                $"SKILLS   POINTS {ProfileProgress.SkillPoints}",
                new Vector2(0.06f, 0.72f),
                new Vector2(0.94f, 0.96f),
                20,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                violet);

            CreateButton(
                skills.transform,
                $"MIGHT {ProfileProgress.MightRank}",
                new Vector2(0.05f, 0.12f),
                new Vector2(0.31f, 0.62f),
                warm,
                () => UpgradeSkill("might"));

            CreateButton(
                skills.transform,
                $"GUARD {ProfileProgress.GuardRank}",
                new Vector2(0.37f, 0.12f),
                new Vector2(0.63f, 0.62f),
                cyan,
                () => UpgradeSkill("guard"));

            CreateButton(
                skills.transform,
                $"AGILITY {ProfileProgress.AgilityRank}",
                new Vector2(0.69f, 0.12f),
                new Vector2(0.95f, 0.62f),
                violet,
                () => UpgradeSkill("agility"));

            GameObject craft = CreatePanel(
                panel,
                new Vector2(0.635f, 0.04f),
                new Vector2(0.96f, 0.15f),
                new Color(0.028f, 0.044f, 0.061f, 0.98f));

            string craftId = ProfileProgress.VoidGardenUnlocked
                ? "void_talisman"
                : ProfileProgress.AshenForgeUnlocked
                    ? "ember_plate"
                    : "crystal_mail";

            string craftLabel = craftId switch
            {
                "void_talisman" => "CRAFT VOID TALISMAN",
                "ember_plate" => "CRAFT EMBER PLATE",
                _ => "CRAFT CRYSTAL MAIL"
            };

            CreateButton(
                craft.transform,
                craftLabel,
                new Vector2(0.04f, 0.12f),
                new Vector2(0.96f, 0.88f),
                EquipmentCatalog.GetRarityColor(EquipmentCatalog.Get(craftId).Rarity),
                () => TryCraft(craftId));
        }

        private void UpgradeSkill(string id)
        {
            ProfileProgress.UpgradeSkill(id);
            Rebuild();
        }

        private void TryCraft(string id)
        {
            if (ProfileProgress.OwnsEquipment(id))
                return;

            string primary;
            int primaryCount;
            string secondary = null;
            int secondaryCount = 0;
            int gold;

            if (id == "void_talisman")
            {
                primary = "moon_bloom";
                primaryCount = 5;
                secondary = "void_fragment";
                secondaryCount = 4;
                gold = 120;
            }
            else if (id == "ember_plate")
            {
                primary = "iron_ore";
                primaryCount = 8;
                secondary = "ember_core";
                secondaryCount = 3;
                gold = 90;
            }
            else
            {
                primary = "crystal_shard";
                primaryCount = 7;
                gold = 45;
            }

            if (ProfileProgress.Gold < gold ||
                ProfileProgress.GetMaterial(primary) < primaryCount ||
                (!string.IsNullOrEmpty(secondary) &&
                 ProfileProgress.GetMaterial(secondary) < secondaryCount))
                return;

            ProfileProgress.SpendGold(gold);
            ProfileProgress.SpendMaterial(primary, primaryCount);

            if (!string.IsNullOrEmpty(secondary))
                ProfileProgress.SpendMaterial(secondary, secondaryCount);

            ProfileProgress.AddEquipment(id);
            ProfileProgress.Equip(id);
            Rebuild();
        }

        private void Rebuild()
        {
            Destroy(gameObject);
            Show();
        }

        private void BuildClose()
        {
            CreateButton(
                panel,
                "CLOSE",
                new Vector2(0.82f, 0.89f),
                new Vector2(0.96f, 0.965f),
                new Color(0.55f, 0.60f, 0.66f),
                Close);
        }

        private void Close()
        {
            HubServiceUI.SetExternalOpen(false);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (GameObject.Find("AdventureInventoryUI") == null ||
                GameObject.Find("AdventureInventoryUI") == gameObject)
            {
                HubServiceUI.SetExternalOpen(false);
            }
        }

        private static string SlotLabel(EquipmentSlot slot)
        {
            return slot switch
            {
                EquipmentSlot.Melee => "MELEE",
                EquipmentSlot.Ranged => "RANGED",
                EquipmentSlot.Head => "HEAD",
                EquipmentSlot.Body => "BODY",
                EquipmentSlot.Boots => "BOOTS",
                EquipmentSlot.Accessory => "ACCESSORY",
                _ => slot.ToString().ToUpperInvariant()
            };
        }

        private GameObject CreatePanel(RectTransform parent, Vector2 min, Vector2 max, Color color)
        {
            GameObject go = new GameObject("Panel");
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            go.AddComponent<Image>().color = color;
            return go;
        }

        private Text AddText(
            RectTransform parent,
            string value,
            Vector2 min,
            Vector2 max,
            int size,
            FontStyle style,
            TextAnchor alignment,
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
            text.text = value;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private Button CreateButton(
            Transform parent,
            string label,
            Vector2 min,
            Vector2 max,
            Color accent,
            Action action)
        {
            GameObject go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = go.AddComponent<Image>();
            image.color = new Color(accent.r * 0.28f, accent.g * 0.28f, accent.b * 0.28f, 0.98f);

            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = accent;
            outline.effectDistance = new Vector2(2f, -2f);

            Button button = go.AddComponent<Button>();
            button.targetGraphic = image;
            if (action != null)
                button.onClick.AddListener(() => action());

            AddText(
                go.GetComponent<RectTransform>(),
                label,
                Vector2.zero,
                Vector2.one,
                16,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);

            return button;
        }
    }
}
