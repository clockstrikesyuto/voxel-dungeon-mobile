using UnityEngine;

namespace VoxelDungeon.Core
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class BossArenaTrigger : MonoBehaviour
    {
        [SerializeField] private MissionController mission;
        [SerializeField] private GameObject closeBarrier;
        private bool triggered;

        public void Configure(MissionController controller, GameObject barrier, Vector3 size)
        {
            mission = controller;
            closeBarrier = barrier;

            BoxCollider box = GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = size;

            Rigidbody body = GetComponent<Rigidbody>();
            if (body == null)
                body = gameObject.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;

            if (closeBarrier != null)
            {
                Vector3 scale = closeBarrier.transform.localScale;
                scale.x = Mathf.Max(scale.x, size.x + 8f);
                scale.y = Mathf.Max(scale.y, size.y + 3f);
                scale.z = Mathf.Max(0.42f, scale.z);
                closeBarrier.transform.localScale = scale;

                Collider barrierCollider = closeBarrier.GetComponent<Collider>();
                if (barrierCollider != null)
                    barrierCollider.isTrigger = false;

                if (closeBarrier.GetComponent<BossArenaBarrier>() == null)
                    closeBarrier.AddComponent<BossArenaBarrier>();

                closeBarrier.SetActive(false);
            }

            mission?.RegisterArenaBarrier(closeBarrier);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered)
                return;

            VoxelDungeon.Player.TopDownPlayerMotor motor =
                other.GetComponentInParent<VoxelDungeon.Player.TopDownPlayerMotor>();

            if (motor == null)
                return;

            triggered = true;

            VoxelDungeon.Player.StageCheckpointSystem checkpoints =
                motor.GetComponent<VoxelDungeon.Player.StageCheckpointSystem>();

            if (checkpoints != null)
            {
                Vector3 inside = transform.position + transform.forward * 2.8f;
                inside.y = motor.transform.position.y;
                checkpoints.ActivateCheckpoint(inside, "BOSS ARENA");
            }

            if (closeBarrier != null)
                closeBarrier.SetActive(true);

            mission?.NotifyBossArenaEntered();

            BoxCollider box = GetComponent<BoxCollider>();
            if (box != null)
                box.enabled = false;
        }
    }
}
