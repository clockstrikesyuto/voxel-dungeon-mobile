using System;
using VoxelDungeon.Items;

namespace VoxelDungeon.Loot
{
    [Serializable]
    public sealed class LootEntry
    {
        public ItemDefinition item;
        public float weight = 1f;
        public int minPowerOffset = 0;
        public int maxPowerOffset = 0;
    }
}
