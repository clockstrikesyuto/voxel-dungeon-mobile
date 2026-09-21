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
                scale.x = Mathf.Max(scale.x, size.x + 4f);
                scale.y = Mathf.Max(scale.y, size.y + 1.5f);
                scale.z = Mathf.Max(0.34f, scale.z);
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

            if (other.GetComponentInParent<VoxelDungeon.Player.TopDownPlayerMotor>() == null)
                return;

            triggered = true;

            if (closeBarrier != null)
                closeBarrier.SetActive(true);

            mission?.NotifyBossArenaEntered();

            BoxCollider box = GetComponent<BoxCollider>();
            if (box != null)
                box.enabled = false;
        }
    }
}
