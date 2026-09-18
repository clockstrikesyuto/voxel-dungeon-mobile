using UnityEngine;

namespace VoxelDungeon.Core
{
    public sealed class StageSceneIdentity : MonoBehaviour
    {
        [SerializeField] private string stageId;
        [SerializeField] private string stageName;

        public void Configure(string id, string displayName)
        {
            stageId = id;
            stageName = displayName;
        }

        private void Awake()
        {
            if (!string.IsNullOrEmpty(stageId))
                GameFlowState.SelectedStageId = stageId;

            if (!string.IsNullOrEmpty(stageName))
                GameFlowState.SelectedStageName = stageName;
        }
    }
}
