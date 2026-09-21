using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelDungeon.Items
{
    public enum EquipmentSlot
    {
        Melee,
        Ranged
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

        public EquipmentRecord(
            string id,
            string name,
            EquipmentSlot slot,
            ItemRarity rarity,
            int power,
            float cooldownMultiplier,
            string trait)
        {
            Id = id;
            Name = name;
            Slot = slot;
            Rarity = rarity;
            Power = power;
            CooldownMultiplier = cooldownMultiplier;
            Trait = trait;
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
                ItemRarity.Rare, 8, 0.92f, "Fast crystal-forged blade"),

            ["warden_cleaver"] = new EquipmentRecord(
                "warden_cleaver", "WARDEN CLEAVER", EquipmentSlot.Melee,
                ItemRarity.Epic, 17, 1.12f, "Heavy strike with high power"),

            ["ember_axe"] = new EquipmentRecord(
                "ember_axe", "EMBER AXE", EquipmentSlot.Melee,
                ItemRarity.Rare, 13, 1.05f, "Forged in the Ashen foundry"),

            ["colossus_maul"] = new EquipmentRecord(
                "colossus_maul", "COLOSSUS MAUL", EquipmentSlot.Melee,
                ItemRarity.Legendary, 28, 1.22f, "Massive boss-forged weapon"),

            ["moonblade"] = new EquipmentRecord(
                "moonblade", "MOONBLADE", EquipmentSlot.Melee,
                ItemRarity.Epic, 19, 0.88f, "Light and fast lunar blade"),

            ["void_edge"] = new EquipmentRecord(
                "void_edge", "VOID EDGE", EquipmentSlot.Melee,
                ItemRarity.Legendary, 34, 0.96f, "Blade carried by the garden guardian"),

            ["field_bow"] = new EquipmentRecord(
                "field_bow", "FIELD BOW", EquipmentSlot.Ranged,
                ItemRarity.Common, 0, 1f, "Reliable starter bow"),

            ["crystal_bow"] = new EquipmentRecord(
                "crystal_bow", "CRYSTAL BOW", EquipmentSlot.Ranged,
                ItemRarity.Rare, 7, 0.90f, "Quick luminous shots"),

            ["ember_repeater"] = new EquipmentRecord(
                "ember_repeater", "EMBER REPEATER", EquipmentSlot.Ranged,
                ItemRarity.Epic, 15, 0.80f, "Rapid forged launcher"),

            ["void_staff"] = new EquipmentRecord(
                "void_staff", "VOID STAFF", EquipmentSlot.Ranged,
                ItemRarity.Legendary, 26, 1.05f, "Focused arcane projectile"),

            ["starbow"] = new EquipmentRecord(
                "starbow", "STAR BOW", EquipmentSlot.Ranged,
                ItemRarity.Epic, 18, 0.82f, "Rapid shots infused with garden light")
        };

        public static EquipmentRecord Get(string id)
        {
            if (!string.IsNullOrEmpty(id) && Items.TryGetValue(id, out EquipmentRecord item))
                return item;

            return Items["rustblade"];
        }

        public static EquipmentRecord GetStarterRanged() => Items["field_bow"];

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
