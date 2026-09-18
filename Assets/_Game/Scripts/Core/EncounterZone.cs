using UnityEngine;

namespace VoxelDungeon.Core
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class EncounterZone : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private bool triggered;

        public void Configure(GameObject[] targets, Vector3 size)
        {
            enemies = targets;

            BoxCollider box = GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = size;

            Rigidbody body = GetComponent<Rigidbody>();
            if (body == null)
                body = gameObject.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;
        }

        private void Awake()
        {
            if (enemies == null)
                return;

            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                    enemy.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered)
                return;

            if (other.GetComponentInParent<VoxelDungeon.Player.TopDownPlayerMotor>() == null)
                return;

            triggered = true;

            if (enemies != null)
            {
                foreach (GameObject enemy in enemies)
                {
                    if (enemy != null)
                        enemy.SetActive(true);
                }
            }

            BoxCollider box = GetComponent<BoxCollider>();
            if (box != null)
                box.enabled = false;
        }
    }
}
