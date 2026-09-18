using System.Collections.Generic;
using UnityEngine;
using VoxelDungeon.Items;

namespace VoxelDungeon.Loot
{
    [CreateAssetMenu(menuName = "Voxel Dungeon/Loot/Loot Table")]
    public sealed class LootTable : ScriptableObject
    {
        [SerializeField] private List<LootEntry> entries = new();

        public ItemDefinition Roll(System.Random random = null)
        {
            if (entries == null || entries.Count == 0) return null;

            float total = 0f;
            for (int i = 0; i < entries.Count; i++)
                total += Mathf.Max(0f, entries[i].weight);

            if (total <= 0f) return null;

            double roll = (random?.NextDouble() ?? Random.value) * total;
            float cursor = 0f;

            for (int i = 0; i < entries.Count; i++)
            {
                cursor += Mathf.Max(0f, entries[i].weight);
                if (roll <= cursor)
                    return entries[i].item;
            }

            return entries[entries.Count - 1].item;
        }
    }
}
