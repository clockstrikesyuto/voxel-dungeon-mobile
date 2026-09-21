using UnityEngine;
using UnityEngine.SceneManagement;

namespace VoxelDungeon.Loot
{
    public static class StageLootCatalog
    {
        private static readonly string[] CryptEquipment =
        {
            "crystal_saber", "crystal_daggers", "crystal_bow",
            "crystal_hood", "crystal_mail", "crystal_steps", "crystal_charm"
        };

        private static readonly string[] ForgeEquipment =
        {
            "ember_axe", "forge_spear", "ember_repeater",
            "forge_helm", "ember_plate", "ember_greaves", "forge_emblem"
        };

        private static readonly string[] VoidEquipment =
        {
            "moonblade", "starbow", "astral_crossbow",
            "astral_crown", "void_mantle", "moonstep_boots", "void_talisman"
        };

        public static string[] GetEquipmentPool()
        {
            string scene = SceneManager.GetActiveScene().name;
            if (scene == "Mission_Ashen") return ForgeEquipment;
            if (scene == "Mission_Void") return VoidEquipment;
            return CryptEquipment;
        }

        public static string RollEquipment()
        {
            string[] pool = GetEquipmentPool();
            return pool[Random.Range(0, pool.Length)];
        }

        public static string PrimaryMaterial()
        {
            string scene = SceneManager.GetActiveScene().name;
            if (scene == "Mission_Ashen") return "iron_ore";
            if (scene == "Mission_Void") return "moon_bloom";
            return "crystal_shard";
        }

        public static string RareMaterial()
        {
            string scene = SceneManager.GetActiveScene().name;
            if (scene == "Mission_Ashen") return "ember_core";
            if (scene == "Mission_Void") return "void_fragment";
            return "ancient_relic";
        }
    }
}
