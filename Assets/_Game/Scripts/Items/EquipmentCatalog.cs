using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelDungeon.Items
{
    public enum EquipmentSlot
    {
        Melee,
        Ranged,
        Head,
        Body,
        Boots,
        Accessory
    }

    [Serializable]
    public readonly struct EquipmentRecord
    {
        public readonly string Id;
        public readonly string Name;
        public readonly EquipmentSlot Slot;
        public readonly ItemRarity Rarity;
        public readonly int Power;
        public readonly float CooldownMultiplier;
        public readonly string Trait;
        public readonly int Defense;
        public readonly int Vitality;
        public readonly float CritChance;
        public readonly float MoveSpeedBonus;

        public EquipmentRecord(
            string id,
            string name,
            EquipmentSlot slot,
            ItemRarity rarity,
            int power,
            float cooldownMultiplier,
            string trait,
            int defense = 0,
            int vitality = 0,
            float critChance = 0f,
            float moveSpeedBonus = 0f)
        {
            Id = id;
            Name = name;
            Slot = slot;
            Rarity = rarity;
            Power = power;
            CooldownMultiplier = cooldownMultiplier;
            Trait = trait;
            Defense = defense;
            Vitality = vitality;
            CritChance = critChance;
            MoveSpeedBonus = moveSpeedBonus;
        }
    }

    public static class EquipmentCatalog
    {
        private static readonly Dictionary<string, EquipmentRecord> Items = new()
        {
            ["rustblade"] = new EquipmentRecord(
                "rustblade", "FRONTIER BLADE", EquipmentSlot.Melee,
                ItemRarity.Common, 0, 1f, "Balanced starter sword"),

            ["crystal_saber"] = new EquipmentRecord(
                "crystal_saber", "CRYSTAL SABER", EquipmentSlot.Melee,
                ItemRarity.Rare, 8, 0.92f, "Fast crystal-forged blade", critChance: 0.03f),

            ["warden_cleaver"] = new EquipmentRecord(
                "warden_cleaver", "WARDEN CLEAVER", EquipmentSlot.Melee,
                ItemRarity.Epic, 17, 1.12f, "Heavy strike with high power", defense: 2),

            ["ember_axe"] = new EquipmentRecord(
                "ember_axe", "EMBER AXE", EquipmentSlot.Melee,
                ItemRarity.Rare, 13, 1.05f, "Forged in the Ashen foundry"),

            ["colossus_maul"] = new EquipmentRecord(
                "colossus_maul", "COLOSSUS MAUL", EquipmentSlot.Melee,
                ItemRarity.Legendary, 28, 1.22f, "Massive boss-forged weapon", vitality: 12),

            ["moonblade"] = new EquipmentRecord(
                "moonblade", "MOONBLADE", EquipmentSlot.Melee,
                ItemRarity.Epic, 19, 0.88f, "Light and fast lunar blade", critChance: 0.05f, moveSpeedBonus: 0.03f),

            ["void_edge"] = new EquipmentRecord(
                "void_edge", "VOID EDGE", EquipmentSlot.Melee,
                ItemRarity.Legendary, 34, 0.96f, "Blade carried by the garden guardian", critChance: 0.07f),

            ["field_bow"] = new EquipmentRecord(
                "field_bow", "FIELD BOW", EquipmentSlot.Ranged,
                ItemRarity.Common, 0, 1f, "Reliable starter bow"),

            ["crystal_bow"] = new EquipmentRecord(
                "crystal_bow", "CRYSTAL BOW", EquipmentSlot.Ranged,
                ItemRarity.Rare, 7, 0.90f, "Quick luminous shots", critChance: 0.03f),

            ["ember_repeater"] = new EquipmentRecord(
                "ember_repeater", "EMBER REPEATER", EquipmentSlot.Ranged,
                ItemRarity.Epic, 15, 0.80f, "Rapid forged launcher"),

            ["void_staff"] = new EquipmentRecord(
                "void_staff", "VOID STAFF", EquipmentSlot.Ranged,
                ItemRarity.Legendary, 26, 1.05f, "Focused arcane projectile", vitality: 8, critChance: 0.04f),

            ["starbow"] = new EquipmentRecord(
                "starbow", "STAR BOW", EquipmentSlot.Ranged,
                ItemRarity.Epic, 18, 0.82f, "Rapid shots infused with garden light", critChance: 0.05f),

            ["frontier_cap"] = new EquipmentRecord(
                "frontier_cap", "FRONTIER CAP", EquipmentSlot.Head,
                ItemRarity.Common, 0, 1f, "Simple frontier protection", defense: 2, vitality: 4),

            ["crystal_hood"] = new EquipmentRecord(
                "crystal_hood", "CRYSTAL HOOD", EquipmentSlot.Head,
                ItemRarity.Rare, 0, 1f, "Resonates with crystal light", defense: 5, vitality: 8, critChance: 0.02f),

            ["forge_helm"] = new EquipmentRecord(
                "forge_helm", "FORGE HELM", EquipmentSlot.Head,
                ItemRarity.Epic, 0, 1f, "Heat-tempered plated helm", defense: 9, vitality: 12),

            ["astral_crown"] = new EquipmentRecord(
                "astral_crown", "ASTRAL CROWN", EquipmentSlot.Head,
                ItemRarity.Legendary, 0, 1f, "Crown of the garden sentinel", defense: 8, vitality: 18, critChance: 0.05f),

            ["frontier_vest"] = new EquipmentRecord(
                "frontier_vest", "FRONTIER VEST", EquipmentSlot.Body,
                ItemRarity.Common, 0, 1f, "Light expedition armor", defense: 4, vitality: 8),

            ["crystal_mail"] = new EquipmentRecord(
                "crystal_mail", "CRYSTAL MAIL", EquipmentSlot.Body,
                ItemRarity.Rare, 0, 1f, "Layered crystal-scale armor", defense: 10, vitality: 15),

            ["ember_plate"] = new EquipmentRecord(
                "ember_plate", "EMBER PLATE", EquipmentSlot.Body,
                ItemRarity.Epic, 0, 1f, "Heavy foundry plate", defense: 16, vitality: 24),

            ["void_mantle"] = new EquipmentRecord(
                "void_mantle", "VOID MANTLE", EquipmentSlot.Body,
                ItemRarity.Legendary, 0, 1f, "Weightless astral mantle", defense: 14, vitality: 30, moveSpeedBonus: 0.03f),

            ["trail_boots"] = new EquipmentRecord(
                "trail_boots", "TRAIL BOOTS", EquipmentSlot.Boots,
                ItemRarity.Common, 0, 1f, "Reliable expedition boots", defense: 1, moveSpeedBonus: 0.02f),

            ["crystal_steps"] = new EquipmentRecord(
                "crystal_steps", "CRYSTAL STEPS", EquipmentSlot.Boots,
                ItemRarity.Rare, 0, 1f, "Light-footed cavern boots", defense: 3, moveSpeedBonus: 0.05f),

            ["ember_greaves"] = new EquipmentRecord(
                "ember_greaves", "EMBER GREAVES", EquipmentSlot.Boots,
                ItemRarity.Epic, 0, 1f, "Stable boots for molten ground", defense: 7, vitality: 8, moveSpeedBonus: 0.03f),

            ["moonstep_boots"] = new EquipmentRecord(
                "moonstep_boots", "MOONSTEP BOOTS", EquipmentSlot.Boots,
                ItemRarity.Legendary, 0, 1f, "Almost weightless astral steps", defense: 6, moveSpeedBonus: 0.09f, critChance: 0.02f),

            ["scout_charm"] = new EquipmentRecord(
                "scout_charm", "SCOUT CHARM", EquipmentSlot.Accessory,
                ItemRarity.Common, 0, 1f, "Small lucky charm", critChance: 0.01f),

            ["crystal_charm"] = new EquipmentRecord(
                "crystal_charm", "CRYSTAL CHARM", EquipmentSlot.Accessory,
                ItemRarity.Rare, 3, 1f, "Sharpens crystal resonance", critChance: 0.04f),

            ["forge_emblem"] = new EquipmentRecord(
                "forge_emblem", "FORGE EMBLEM", EquipmentSlot.Accessory,
                ItemRarity.Epic, 5, 1f, "Forged emblem of endurance", defense: 4, vitality: 14),

            ["void_talisman"] = new EquipmentRecord(
                "void_talisman", "VOID TALISMAN", EquipmentSlot.Accessory,
                ItemRarity.Legendary, 8, 1f, "Unstable astral focus", vitality: 12, critChance: 0.08f, moveSpeedBonus: 0.03f)
        };

        public static EquipmentRecord Get(string id)
        {
            if (!string.IsNullOrEmpty(id) && Items.TryGetValue(id, out EquipmentRecord item))
                return item;

            return Items["rustblade"];
        }

        public static EquipmentRecord GetStarterRanged() => Items["field_bow"];

        public static EquipmentRecord GetStarter(EquipmentSlot slot)
        {
            return slot switch
            {
                EquipmentSlot.Melee => Items["rustblade"],
                EquipmentSlot.Ranged => Items["field_bow"],
                EquipmentSlot.Head => Items["frontier_cap"],
                EquipmentSlot.Body => Items["frontier_vest"],
                EquipmentSlot.Boots => Items["trail_boots"],
                EquipmentSlot.Accessory => Items["scout_charm"],
                _ => Items["rustblade"]
            };
        }

        public static IReadOnlyCollection<EquipmentRecord> All => Items.Values;

        public static Color GetRarityColor(ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => new Color(0.78f, 0.82f, 0.86f),
                ItemRarity.Rare => new Color(0.18f, 0.66f, 1f),
                ItemRarity.Epic => new Color(0.68f, 0.28f, 1f),
                ItemRarity.Legendary => new Color(1f, 0.62f, 0.10f),
                _ => Color.white
            };
        }
    }
}
