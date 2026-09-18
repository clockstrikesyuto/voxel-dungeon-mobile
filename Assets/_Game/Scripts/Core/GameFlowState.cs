namespace VoxelDungeon.Core
{
    public enum PlayModeKind
    {
        Solo,
        Multiplayer
    }

    public static class GameFlowState
    {
        public static PlayModeKind Mode { get; set; } = PlayModeKind.Solo;
        public static string SelectedStageId { get; set; } = "stage.crypt";
        public static string SelectedStageName { get; set; } = "CRYSTAL CRYPT";
        public static string RoomCode { get; set; } = string.Empty;

        public static void ResetToTitle()
        {
            Mode = PlayModeKind.Solo;
            SelectedStageId = "stage.crypt";
            SelectedStageName = "CRYSTAL CRYPT";
            RoomCode = string.Empty;
        }
    }
}
