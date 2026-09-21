namespace VoxelDungeon.Items
{
    public enum ItemRarity
    {
        Common = 0,

        // Current equipment naming.
        Rare = 1,
        Epic = 2,
        Legendary = 3,

        // Legacy aliases kept so older ItemDefinition assets remain compatible.
        Refined = Rare,
        Heroic = Epic,
        Mythic = Legendary
    }
}
