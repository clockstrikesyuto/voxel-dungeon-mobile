using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelDungeon.Items;

namespace VoxelDungeon.Core
{
    public static class ProfileProgress
    {
        private const string GoldKey = "profile.gold";
        private const string MeleeKey = "profile.melee";
        private const string RangedKey = "profile.ranged";
        private const string CryptClearKey = "profile.crypt.clear";
        private const string AshenClearKey = "profile.ashen.clear";
        private const string VoidClearKey = "profile.void.clear";
        private const string OwnedEquipmentKey = "profile.equipment.owned";
        private const string EquippedMeleeKey = "profile.equipment.melee";
        private const string EquippedRangedKey = "profile.equipment.ranged";
        private const string ArchivistRewardKey = "profile.quest.archivist";

        public static int Gold => PlayerPrefs.GetInt(GoldKey, 0);
        public static int MeleePower => PlayerPrefs.GetInt(MeleeKey, 0);
        public static int RangedPower => PlayerPrefs.GetInt(RangedKey, 0);
        public static bool CrystalCryptCleared => PlayerPrefs.GetInt(CryptClearKey, 0) == 1;
        public static bool AshenForgeCleared => PlayerPrefs.GetInt(AshenClearKey, 0) == 1;
        public static bool VoidGardenCleared => PlayerPrefs.GetInt(VoidClearKey, 0) == 1;

        public static bool AshenForgeUnlocked => CrystalCryptCleared;
        public static bool VoidGardenUnlocked => AshenForgeCleared;

        public static string EquippedMeleeId =>
            PlayerPrefs.GetString(EquippedMeleeKey, "rustblade");

        public static string EquippedRangedId =>
            PlayerPrefs.GetString(EquippedRangedKey, "field_bow");

        public static EquipmentRecord EquippedMelee =>
            EquipmentCatalog.Get(EquippedMeleeId);

        public static EquipmentRecord EquippedRanged
        {
            get
            {
                string id = EquippedRangedId;
                foreach (EquipmentRecord item in EquipmentCatalog.All)
                {
                    if (item.Id == id)
                        return item;
                }
                return EquipmentCatalog.GetStarterRanged();
            }
        }

        public static void EnsureStarterEquipment()
        {
            HashSet<string> owned = GetOwnedSet();
            bool changed = owned.Add("rustblade");
            changed |= owned.Add("field_bow");

            if (changed)
                SaveOwnedSet(owned);

            if (!PlayerPrefs.HasKey(EquippedMeleeKey))
                PlayerPrefs.SetString(EquippedMeleeKey, "rustblade");

            if (!PlayerPrefs.HasKey(EquippedRangedKey))
                PlayerPrefs.SetString(EquippedRangedKey, "field_bow");

            PlayerPrefs.Save();
        }

        public static void AddGold(int amount)
        {
            if (amount <= 0) return;
            PlayerPrefs.SetInt(GoldKey, Gold + amount);
            PlayerPrefs.Save();
        }

        public static bool SpendGold(int amount)
        {
            if (amount <= 0 || Gold < amount)
                return false;

            PlayerPrefs.SetInt(GoldKey, Gold - amount);
            PlayerPrefs.Save();
            return true;
        }

        public static bool UpgradeMelee(int cost, int power)
        {
            if (!SpendGold(cost))
                return false;

            PlayerPrefs.SetInt(MeleeKey, MeleePower + Mathf.Max(1, power));
            PlayerPrefs.Save();
            return true;
        }

        public static bool UpgradeRanged(int cost, int power)
        {
            if (!SpendGold(cost))
                return false;

            PlayerPrefs.SetInt(RangedKey, RangedPower + Mathf.Max(1, power));
            PlayerPrefs.Save();
            return true;
        }

        public static bool AddEquipment(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
                return false;

            EnsureStarterEquipment();
            HashSet<string> owned = GetOwnedSet();
            bool added = owned.Add(itemId);

            if (added)
                SaveOwnedSet(owned);

            return added;
        }

        public static bool OwnsEquipment(string itemId)
        {
            EnsureStarterEquipment();
            return GetOwnedSet().Contains(itemId);
        }

        public static List<EquipmentRecord> GetOwnedEquipment(EquipmentSlot? slot = null)
        {
            EnsureStarterEquipment();
            HashSet<string> owned = GetOwnedSet();
            List<EquipmentRecord> result = new List<EquipmentRecord>();

            foreach (EquipmentRecord item in EquipmentCatalog.All)
            {
                if (!owned.Contains(item.Id))
                    continue;

                if (slot.HasValue && item.Slot != slot.Value)
                    continue;

                result.Add(item);
            }

            result.Sort((a, b) =>
            {
                int rarity = b.Rarity.CompareTo(a.Rarity);
                return rarity != 0 ? rarity : b.Power.CompareTo(a.Power);
            });

            return result;
        }

        public static bool Equip(string itemId)
        {
            if (!OwnsEquipment(itemId))
                return false;

            EquipmentRecord item = EquipmentCatalog.Get(itemId);

            if (item.Slot == EquipmentSlot.Melee)
                PlayerPrefs.SetString(EquippedMeleeKey, item.Id);
            else
                PlayerPrefs.SetString(EquippedRangedKey, item.Id);

            PlayerPrefs.Save();
            return true;
        }

        public static bool BuyEquipment(string itemId, int cost)
        {
            if (OwnsEquipment(itemId))
                return false;

            if (!SpendGold(cost))
                return false;

            AddEquipment(itemId);
            return true;
        }

        public static void CompleteStage(string stageId)
        {
            if (stageId == "stage.crypt")
                PlayerPrefs.SetInt(CryptClearKey, 1);
            else if (stageId == "stage.ashen")
                PlayerPrefs.SetInt(AshenClearKey, 1);
            else if (stageId == "stage.void")
                PlayerPrefs.SetInt(VoidClearKey, 1);

            PlayerPrefs.Save();
        }

        public static bool ClaimArchivistReward()
        {
            if (!AshenForgeCleared || PlayerPrefs.GetInt(ArchivistRewardKey, 0) == 1)
                return false;

            PlayerPrefs.SetInt(ArchivistRewardKey, 1);
            AddGold(80);
            AddEquipment("void_staff");
            PlayerPrefs.Save();
            return true;
        }

        public static bool ArchivistRewardClaimed =>
            PlayerPrefs.GetInt(ArchivistRewardKey, 0) == 1;

        public static void ResetProfile()
        {
            PlayerPrefs.DeleteKey(GoldKey);
            PlayerPrefs.DeleteKey(MeleeKey);
            PlayerPrefs.DeleteKey(RangedKey);
            PlayerPrefs.DeleteKey(CryptClearKey);
            PlayerPrefs.DeleteKey(AshenClearKey);
            PlayerPrefs.DeleteKey(VoidClearKey);
            PlayerPrefs.DeleteKey(OwnedEquipmentKey);
            PlayerPrefs.DeleteKey(EquippedMeleeKey);
            PlayerPrefs.DeleteKey(EquippedRangedKey);
            PlayerPrefs.DeleteKey(ArchivistRewardKey);
            PlayerPrefs.Save();
        }

        private static HashSet<string> GetOwnedSet()
        {
            string raw = PlayerPrefs.GetString(OwnedEquipmentKey, string.Empty);
            HashSet<string> owned = new HashSet<string>(StringComparer.Ordinal);

            if (!string.IsNullOrEmpty(raw))
            {
                string[] parts = raw.Split('|');
                foreach (string part in parts)
                {
                    if (!string.IsNullOrWhiteSpace(part))
                        owned.Add(part);
                }
            }

            return owned;
        }

        private static void SaveOwnedSet(HashSet<string> owned)
        {
            string raw = string.Join("|", owned);
            PlayerPrefs.SetString(OwnedEquipmentKey, raw);
            PlayerPrefs.Save();
        }
    }
}
