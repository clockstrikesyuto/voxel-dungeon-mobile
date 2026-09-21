using System;
using System.Collections.Generic;
using UnityEngine;
using VoxelDungeon.Items;

namespace VoxelDungeon.Core
{
    public readonly struct EquipmentRoll
    {
        public readonly int PowerBonus;
        public readonly int DefenseBonus;
        public readonly int VitalityBonus;
        public readonly float CritBonus;
        public readonly float MoveSpeedBonus;

        public EquipmentRoll(int power, int defense, int vitality, float crit, float moveSpeed)
        {
            PowerBonus = power;
            DefenseBonus = defense;
            VitalityBonus = vitality;
            CritBonus = crit;
            MoveSpeedBonus = moveSpeed;
        }

        public string Summary =>
            $"+{PowerBonus} PWR  +{DefenseBonus} DEF  +{VitalityBonus} HP  +{Mathf.RoundToInt(CritBonus * 100f)}% CRIT";
    }

    public static class ProfileProgress
    {
        private const string GoldKey = "profile.gold";
        private const string MeleeKey = "profile.melee";
        private const string RangedKey = "profile.ranged";
        private const string ArmorKey = "profile.armor";
        private const string CryptClearKey = "profile.crypt.clear";
        private const string AshenClearKey = "profile.ashen.clear";
        private const string VoidClearKey = "profile.void.clear";
        private const string OwnedEquipmentKey = "profile.equipment.owned";
        private const string EquippedMeleeKey = "profile.equipment.melee";
        private const string EquippedRangedKey = "profile.equipment.ranged";
        private const string EquippedHeadKey = "profile.equipment.head";
        private const string EquippedBodyKey = "profile.equipment.body";
        private const string EquippedBootsKey = "profile.equipment.boots";
        private const string EquippedAccessoryKey = "profile.equipment.accessory";
        private const string ArchivistRewardKey = "profile.quest.archivist";
        private const string InitializedKey = "profile.adventure.initialized";
        private const string LevelKey = "profile.level";
        private const string ExperienceKey = "profile.experience";
        private const string SkillPointsKey = "profile.skill.points";
        private const string MightRankKey = "profile.skill.might";
        private const string GuardRankKey = "profile.skill.guard";
        private const string AgilityRankKey = "profile.skill.agility";

        private static readonly string[] MaterialIds =
        {
            "crystal_shard",
            "iron_ore",
            "ember_core",
            "moon_bloom",
            "void_fragment",
            "ancient_relic"
        };

        private static readonly string[] ConsumableIds =
        {
            "healing_potion",
            "power_tonic",
            "guard_tonic",
            "haste_tonic",
            "fire_bomb",
            "frost_flask"
        };

        public static int Gold => PlayerPrefs.GetInt(GoldKey, 0);
        public static int MeleePower => PlayerPrefs.GetInt(MeleeKey, 0);
        public static int RangedPower => PlayerPrefs.GetInt(RangedKey, 0);
        public static int ArmorPower => PlayerPrefs.GetInt(ArmorKey, 0);
        public static bool CrystalCryptCleared => PlayerPrefs.GetInt(CryptClearKey, 0) == 1;
        public static bool AshenForgeCleared => PlayerPrefs.GetInt(AshenClearKey, 0) == 1;
        public static bool VoidGardenCleared => PlayerPrefs.GetInt(VoidClearKey, 0) == 1;

        public static bool AshenForgeUnlocked => CrystalCryptCleared;
        public static bool VoidGardenUnlocked => AshenForgeCleared;

        public static int Level => Mathf.Max(1, PlayerPrefs.GetInt(LevelKey, 1));
        public static int Experience => Mathf.Max(0, PlayerPrefs.GetInt(ExperienceKey, 0));
        public static int SkillPoints => Mathf.Max(0, PlayerPrefs.GetInt(SkillPointsKey, 0));
        public static int MightRank => Mathf.Max(0, PlayerPrefs.GetInt(MightRankKey, 0));
        public static int GuardRank => Mathf.Max(0, PlayerPrefs.GetInt(GuardRankKey, 0));
        public static int AgilityRank => Mathf.Max(0, PlayerPrefs.GetInt(AgilityRankKey, 0));
        public static int ExperienceToNextLevel => 80 + (Level - 1) * 45;

        public static string EquippedMeleeId => PlayerPrefs.GetString(EquippedMeleeKey, "rustblade");
        public static string EquippedRangedId => PlayerPrefs.GetString(EquippedRangedKey, "field_bow");
        public static string EquippedHeadId => PlayerPrefs.GetString(EquippedHeadKey, "frontier_cap");
        public static string EquippedBodyId => PlayerPrefs.GetString(EquippedBodyKey, "frontier_vest");
        public static string EquippedBootsId => PlayerPrefs.GetString(EquippedBootsKey, "trail_boots");
        public static string EquippedAccessoryId => PlayerPrefs.GetString(EquippedAccessoryKey, "scout_charm");

        public static EquipmentRecord EquippedMelee => EquipmentCatalog.Get(EquippedMeleeId);
        public static EquipmentRecord EquippedRanged => EquipmentCatalog.Get(EquippedRangedId);
        public static EquipmentRecord EquippedHead => EquipmentCatalog.Get(EquippedHeadId);
        public static EquipmentRecord EquippedBody => EquipmentCatalog.Get(EquippedBodyId);
        public static EquipmentRecord EquippedBoots => EquipmentCatalog.Get(EquippedBootsId);
        public static EquipmentRecord EquippedAccessory => EquipmentCatalog.Get(EquippedAccessoryId);

        public static void EnsureStarterEquipment()
        {
            HashSet<string> owned = GetOwnedSet();
            bool changed = false;

            changed |= owned.Add("rustblade");
            changed |= owned.Add("field_bow");
            changed |= owned.Add("frontier_cap");
            changed |= owned.Add("frontier_vest");
            changed |= owned.Add("trail_boots");
            changed |= owned.Add("scout_charm");

            if (changed)
                SaveOwnedSet(owned);

            EnsureEquippedKey(EquippedMeleeKey, "rustblade");
            EnsureEquippedKey(EquippedRangedKey, "field_bow");
            EnsureEquippedKey(EquippedHeadKey, "frontier_cap");
            EnsureEquippedKey(EquippedBodyKey, "frontier_vest");
            EnsureEquippedKey(EquippedBootsKey, "trail_boots");
            EnsureEquippedKey(EquippedAccessoryKey, "scout_charm");

            foreach (string id in owned)
                EnsureEquipmentRoll(id);

            if (!PlayerPrefs.HasKey(LevelKey))
                PlayerPrefs.SetInt(LevelKey, 1);

            if (PlayerPrefs.GetInt(InitializedKey, 0) == 0)
            {
                PlayerPrefs.SetInt(InitializedKey, 1);
                AddConsumable("healing_potion", 3);
            }

            PlayerPrefs.Save();
        }

        private static void EnsureEquippedKey(string key, string fallback)
        {
            if (!PlayerPrefs.HasKey(key))
                PlayerPrefs.SetString(key, fallback);
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

        public static bool UpgradeArmor(int cost, int defense)
        {
            if (!SpendGold(cost))
                return false;

            PlayerPrefs.SetInt(ArmorKey, ArmorPower + Mathf.Max(1, defense));
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
            {
                SaveOwnedSet(owned);
                EnsureEquipmentRoll(itemId, true);
            }

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
                if (rarity != 0) return rarity;

                int scoreA = a.Power + a.Defense + a.Vitality / 2;
                int scoreB = b.Power + b.Defense + b.Vitality / 2;
                return scoreB.CompareTo(scoreA);
            });

            return result;
        }

        public static string GetEquippedId(EquipmentSlot slot)
        {
            return slot switch
            {
                EquipmentSlot.Melee => EquippedMeleeId,
                EquipmentSlot.Ranged => EquippedRangedId,
                EquipmentSlot.Head => EquippedHeadId,
                EquipmentSlot.Body => EquippedBodyId,
                EquipmentSlot.Boots => EquippedBootsId,
                EquipmentSlot.Accessory => EquippedAccessoryId,
                _ => EquippedMeleeId
            };
        }

        public static EquipmentRecord GetEquipped(EquipmentSlot slot) =>
            EquipmentCatalog.Get(GetEquippedId(slot));

        public static bool Equip(string itemId)
        {
            if (!OwnsEquipment(itemId))
                return false;

            EquipmentRecord item = EquipmentCatalog.Get(itemId);
            string key = item.Slot switch
            {
                EquipmentSlot.Melee => EquippedMeleeKey,
                EquipmentSlot.Ranged => EquippedRangedKey,
                EquipmentSlot.Head => EquippedHeadKey,
                EquipmentSlot.Body => EquippedBodyKey,
                EquipmentSlot.Boots => EquippedBootsKey,
                EquipmentSlot.Accessory => EquippedAccessoryKey,
                _ => null
            };

            if (string.IsNullOrEmpty(key))
                return false;

            PlayerPrefs.SetString(key, item.Id);
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

        public static EquipmentRoll GetEquipmentRoll(string itemId)
        {
            EquipmentRecord item = EquipmentCatalog.Get(itemId);
            int seed = EnsureEquipmentRoll(itemId);
            System.Random random = new System.Random(seed);

            int tier = item.Rarity switch
            {
                ItemRarity.Legendary => 4,
                ItemRarity.Epic => 3,
                ItemRarity.Rare => 2,
                _ => 1
            };

            int power = item.Slot == EquipmentSlot.Melee || item.Slot == EquipmentSlot.Ranged
                ? random.Next(0, tier * 3 + 1)
                : random.Next(0, tier + 1);

            int defense = item.Slot == EquipmentSlot.Head ||
                          item.Slot == EquipmentSlot.Body ||
                          item.Slot == EquipmentSlot.Boots
                ? random.Next(0, tier * 2 + 1)
                : random.Next(0, tier + 1);

            int vitality = random.Next(0, tier * 4 + 1);
            float crit = random.Next(0, tier + 1) * 0.01f;
            float speed = random.Next(0, tier + 1) * 0.005f;

            return new EquipmentRoll(power, defense, vitality, crit, speed);
        }

        private static int EnsureEquipmentRoll(string itemId, bool reroll = false)
        {
            string key = "profile.roll." + itemId;
            int seed = PlayerPrefs.GetInt(key, 0);

            if (seed == 0 || reroll)
            {
                seed = UnityEngine.Random.Range(1000, int.MaxValue);
                PlayerPrefs.SetInt(key, seed);
                PlayerPrefs.Save();
            }

            return seed;
        }

        public static int TotalDefense =>
            SumEquipped(item => item.Defense, roll => roll.DefenseBonus) + GuardRank * 2 + ArmorPower;

        public static int TotalVitality =>
            SumEquipped(item => item.Vitality, roll => roll.VitalityBonus) + GuardRank * 3;

        public static float TotalCritChance =>
            Mathf.Clamp01(SumEquippedFloat(item => item.CritChance, roll => roll.CritBonus) + MightRank * 0.01f);

        public static float TotalMoveSpeedBonus =>
            Mathf.Clamp(SumEquippedFloat(item => item.MoveSpeedBonus, roll => roll.MoveSpeedBonus) + AgilityRank * 0.012f, 0f, 0.35f);

        public static int EquippedWeaponRollPower(EquipmentSlot slot)
        {
            if (slot != EquipmentSlot.Melee && slot != EquipmentSlot.Ranged)
                return 0;

            return GetEquipmentRoll(GetEquippedId(slot)).PowerBonus;
        }

        private static int SumEquipped(Func<EquipmentRecord, int> baseSelector, Func<EquipmentRoll, int> rollSelector)
        {
            int total = 0;
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                EquipmentRecord item = GetEquipped(slot);
                EquipmentRoll roll = GetEquipmentRoll(item.Id);
                total += baseSelector(item) + rollSelector(roll);
            }
            return total;
        }

        private static float SumEquippedFloat(Func<EquipmentRecord, float> baseSelector, Func<EquipmentRoll, float> rollSelector)
        {
            float total = 0f;
            foreach (EquipmentSlot slot in Enum.GetValues(typeof(EquipmentSlot)))
            {
                EquipmentRecord item = GetEquipped(slot);
                EquipmentRoll roll = GetEquipmentRoll(item.Id);
                total += baseSelector(item) + rollSelector(roll);
            }
            return total;
        }

        public static void AddExperience(int amount)
        {
            if (amount <= 0)
                return;

            int level = Level;
            int xp = Experience + amount;
            int points = SkillPoints;

            while (xp >= 80 + (level - 1) * 45)
            {
                xp -= 80 + (level - 1) * 45;
                level++;
                points++;
            }

            PlayerPrefs.SetInt(LevelKey, level);
            PlayerPrefs.SetInt(ExperienceKey, xp);
            PlayerPrefs.SetInt(SkillPointsKey, points);
            PlayerPrefs.Save();
        }

        public static bool UpgradeSkill(string skillId)
        {
            if (SkillPoints <= 0)
                return false;

            string key = skillId switch
            {
                "might" => MightRankKey,
                "guard" => GuardRankKey,
                "agility" => AgilityRankKey,
                _ => null
            };

            if (string.IsNullOrEmpty(key))
                return false;

            int rank = PlayerPrefs.GetInt(key, 0);
            if (rank >= 10)
                return false;

            PlayerPrefs.SetInt(key, rank + 1);
            PlayerPrefs.SetInt(SkillPointsKey, SkillPoints - 1);
            PlayerPrefs.Save();
            return true;
        }

        public static int GetMaterial(string materialId) =>
            Mathf.Max(0, PlayerPrefs.GetInt("profile.material." + materialId, 0));

        public static void AddMaterial(string materialId, int amount)
        {
            if (string.IsNullOrEmpty(materialId) || amount <= 0)
                return;

            string key = "profile.material." + materialId;
            PlayerPrefs.SetInt(key, GetMaterial(materialId) + amount);
            PlayerPrefs.Save();
        }

        public static bool SpendMaterial(string materialId, int amount)
        {
            if (amount <= 0 || GetMaterial(materialId) < amount)
                return false;

            string key = "profile.material." + materialId;
            PlayerPrefs.SetInt(key, GetMaterial(materialId) - amount);
            PlayerPrefs.Save();
            return true;
        }

        public static int GetConsumable(string itemId) =>
            Mathf.Max(0, PlayerPrefs.GetInt("profile.consumable." + itemId, 0));

        public static void AddConsumable(string itemId, int amount)
        {
            if (string.IsNullOrEmpty(itemId) || amount <= 0)
                return;

            string key = "profile.consumable." + itemId;
            PlayerPrefs.SetInt(key, GetConsumable(itemId) + amount);
            PlayerPrefs.Save();
        }

        public static bool ConsumeItem(string itemId, int amount = 1)
        {
            if (amount <= 0 || GetConsumable(itemId) < amount)
                return false;

            string key = "profile.consumable." + itemId;
            PlayerPrefs.SetInt(key, GetConsumable(itemId) - amount);
            PlayerPrefs.Save();
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
            AddMaterial("ancient_relic", 2);
            PlayerPrefs.Save();
            return true;
        }

        public static bool ArchivistRewardClaimed =>
            PlayerPrefs.GetInt(ArchivistRewardKey, 0) == 1;

        public static void ResetProfile()
        {
            string[] directKeys =
            {
                GoldKey, MeleeKey, RangedKey, ArmorKey,
                CryptClearKey, AshenClearKey, VoidClearKey,
                OwnedEquipmentKey,
                EquippedMeleeKey, EquippedRangedKey, EquippedHeadKey,
                EquippedBodyKey, EquippedBootsKey, EquippedAccessoryKey,
                ArchivistRewardKey, InitializedKey,
                LevelKey, ExperienceKey, SkillPointsKey,
                MightRankKey, GuardRankKey, AgilityRankKey
            };

            foreach (string key in directKeys)
                PlayerPrefs.DeleteKey(key);

            foreach (string id in MaterialIds)
                PlayerPrefs.DeleteKey("profile.material." + id);

            foreach (string id in ConsumableIds)
                PlayerPrefs.DeleteKey("profile.consumable." + id);

            foreach (EquipmentRecord item in EquipmentCatalog.All)
                PlayerPrefs.DeleteKey("profile.roll." + item.Id);

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
