namespace VoxelDungeon.Core
{
    public static class MobileRuntimeSettings
    {
        public const int BaselineFrameRate = 30;
        public const int PerformanceFrameRate = 60;
        public static int TargetFrameRate { get; set; } = BaselineFrameRate;
    }
}
