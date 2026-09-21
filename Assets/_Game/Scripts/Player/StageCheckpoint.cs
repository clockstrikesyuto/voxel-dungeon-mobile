using UnityEngine;

namespace VoxelDungeon.Player
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class StageCheckpoint : MonoBehaviour
    {
        [SerializeField] private string checkpointName = "CHECKPOINT";
        [SerializeField] private Vector3 respawnOffset = new Vector3(0f, 0.25f, -1.4f);
        private bool activated;

        public void Configure(string label, Vector3 size, Vector3 offset)
        {
            checkpointName = label;
            respawnOffset = offset;

            BoxCollider box = GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = size;

            Rigidbody body = GetComponent<Rigidbody>();
            if (body == null)
                body = gameObject.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            StageCheckpointSystem checkpoints =
                other.GetComponentInParent<StageCheckpointSystem>();

            if (checkpoints == null)
                return;

            checkpoints.ActivateCheckpoint(
                transform.position + respawnOffset,
                checkpointName);

            activated = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + respawnOffset, 0.55f);
        }
    }
}
