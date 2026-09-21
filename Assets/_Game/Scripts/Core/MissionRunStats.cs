using System.Collections.Generic;
using System.Linq;

namespace VoxelDungeon.Core
{
    public static class MissionRunStats
    {
        private static readonly Dictionary<string, int> Materials = new();
        private static readonly Dictionary<string, int> Consumables = new();
        private static readonly List<string> Equipment = new();

        public static int ExperienceGained { get; private set; }
        public static int GoldCollected { get; private set; }

        public static void Reset()
        {
            Materials.Clear();
            Consumables.Clear();
            Equipment.Clear();
            ExperienceGained = 0;
            GoldCollected = 0;
        }

        public static void AddExperience(int amount)
        {
            if (amount > 0) ExperienceGained += amount;
        }

        public static void AddGold(int amount)
        {
            if (amount > 0) GoldCollected += amount;
        }

        public static void AddMaterial(string id, int amount)
        {
            if (string.IsNullOrEmpty(id) || amount <= 0) return;
            Materials[id] = Materials.TryGetValue(id, out int current) ? current + amount : amount;
        }

        public static void AddConsumable(string id, int amount)
        {
            if (string.IsNullOrEmpty(id) || amount <= 0) return;
            Consumables[id] = Consumables.TryGetValue(id, out int current) ? current + amount : amount;
        }

        public static void AddEquipment(string displayName)
        {
            if (!string.IsNullOrEmpty(displayName))
                Equipment.Add(displayName);
        }

        public static IReadOnlyDictionary<string, int> MaterialSnapshot => Materials;
        public static IReadOnlyDictionary<string, int> ConsumableSnapshot => Consumables;
        public static IReadOnlyList<string> EquipmentSnapshot => Equipment;

        public static string BuildMaterialSummary(int maxLines = 4)
        {
            if (Materials.Count == 0)
                return Localization.IsJapanese ? "なし" : "None";

            return string.Join("\n", Materials.Take(maxLines).Select(pair =>
                $"{Localization.MaterialName(pair.Key)} ×{pair.Value}"));
        }

        public static string BuildEquipmentSummary(int maxLines = 3)
        {
            if (Equipment.Count == 0)
                return Localization.IsJapanese ? "なし" : "None";

            return string.Join("\n", Equipment.Take(maxLines));
        }
    }
}
