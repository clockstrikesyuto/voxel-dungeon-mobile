namespace VoxelDungeon.Network
{
    // This file intentionally avoids a dependency on networking packages so the
    // project can compile before package bootstrap completes.
    public static class NetworkDesignContract
    {
        public const int MaxPlayers = 4;
        public const bool HostAuthoritativeCombat = true;
    }
}
