using UnityEngine;

namespace VoxelDungeon.Core
{
    public enum HubPointKind
    {
        StageGate,
        WorldMap,
        Forge,
        Merchant,
        Training,
        Armory,
        Quest
    }

    public sealed class HubPoint : MonoBehaviour
    {
        [SerializeField] private HubPointKind kind;
        [SerializeField] private string displayName;
        [SerializeField] private string subtitle;
        [SerializeField] private string stageId;
        [SerializeField] private string sceneName = "Mission_Test";
        [SerializeField] private float interactionRadius = 2.4f;

        public HubPointKind Kind => kind;
        public string DisplayName => displayName;
        public string Subtitle => subtitle;
        public string StageId => stageId;
        public string SceneName => sceneName;
        public float InteractionRadius => interactionRadius;

        public void Configure(
            HubPointKind pointKind,
            string pointName,
            string pointSubtitle,
            string targetStageId = "",
            string targetSceneName = "Mission_Test",
            float radius = 2.4f)
        {
            kind = pointKind;
            displayName = pointName;
            subtitle = pointSubtitle;
            stageId = targetStageId;
            sceneName = targetSceneName;
            interactionRadius = Mathf.Max(0.5f, radius);
        }

        public bool IsUnlocked()
        {
            if (kind != HubPointKind.StageGate)
                return true;

            return stageId switch
            {
                "stage.crypt" => true,
                "stage.ashen" => ProfileProgress.AshenForgeUnlocked,
                "stage.void" => ProfileProgress.VoidGardenUnlocked,
                _ => false
            };
        }
    }
}
