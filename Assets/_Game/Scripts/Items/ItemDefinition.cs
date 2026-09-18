using UnityEngine;

namespace VoxelDungeon.Items
{
    public enum ItemSlot
    {
        Melee,
        Ranged,
        Armor,
        Relic
    }

    [CreateAssetMenu(menuName = "Voxel Dungeon/Items/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string stableId = "item.id";
        [SerializeField] private string displayName = "New Item";
        [SerializeField] private ItemSlot slot = ItemSlot.Melee;
        [SerializeField] private ItemRarity rarity = ItemRarity.Common;
        [SerializeField, Min(1)] private int basePower = 1;
        [SerializeField] private Sprite icon;

        public string StableId => stableId;
        public string DisplayName => displayName;
        public ItemSlot Slot => slot;
        public ItemRarity Rarity => rarity;
        public int BasePower => basePower;
        public Sprite Icon => icon;

#if UNITY_EDITOR
        private void OnValidate()
        {
            stableId = stableId.Trim();
            if (string.IsNullOrWhiteSpace(stableId))
                stableId = name.ToLowerInvariant().Replace(' ', '.');
        }
#endif
    }
}
