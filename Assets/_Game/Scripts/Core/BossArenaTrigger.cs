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
                closeBarrier.SetActive(false);
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

            if (mission != null)
                mission.NotifyBossArenaEntered();

            BoxCollider box = GetComponent<BoxCollider>();
            if (box != null)
                box.enabled = false;
        }
    }
}
