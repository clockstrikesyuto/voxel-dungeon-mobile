using UnityEngine;

namespace VoxelDungeon.Core
{
    public static class ProfileProgress
    {
        private const string GoldKey = "profile.gold";
        private const string MeleeKey = "profile.melee";
        private const string RangedKey = "profile.ranged";
        private const string CryptClearKey = "profile.crypt.clear";

        public static int Gold => PlayerPrefs.GetInt(GoldKey, 0);
        public static int MeleePower => PlayerPrefs.GetInt(MeleeKey, 0);
        public static int RangedPower => PlayerPrefs.GetInt(RangedKey, 0);
        public static bool CrystalCryptCleared => PlayerPrefs.GetInt(CryptClearKey, 0) == 1;

        public static bool AshenForgeUnlocked => CrystalCryptCleared;
        public static bool VoidGardenUnlocked => false;

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

        public static void CompleteStage(string stageId)
        {
            if (stageId == "stage.crypt")
            {
                PlayerPrefs.SetInt(CryptClearKey, 1);
                PlayerPrefs.Save();
            }
        }

        public static void ResetProfile()
        {
            PlayerPrefs.DeleteKey(GoldKey);
            PlayerPrefs.DeleteKey(MeleeKey);
            PlayerPrefs.DeleteKey(RangedKey);
            PlayerPrefs.DeleteKey(CryptClearKey);
            PlayerPrefs.Save();
        }
    }
}
