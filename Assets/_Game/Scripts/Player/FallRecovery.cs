using System.Collections;
using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FallRecovery : MonoBehaviour
    {
        [SerializeField] private float recoveryY = -9.5f;

        private CharacterController controller;
        private TopDownPlayerMotor playerMotor;
        private StageCheckpointSystem checkpoints;
        private KnockbackMotor knockback;

        private Vector3 homePosition;
        private bool enemyRecovering;
        private float recoveryLockUntil;

        private bool IsPlayer => playerMotor != null;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            playerMotor = GetComponent<TopDownPlayerMotor>();
            knockback = GetComponent<KnockbackMotor>();
            homePosition = transform.position + Vector3.up * 0.18f;

            if (playerMotor != null)
            {
                checkpoints = GetComponent<StageCheckpointSystem>();
                if (checkpoints == null)
                    checkpoints = gameObject.AddComponent<StageCheckpointSystem>();
            }
        }

        private void Update()
        {
            if (transform.position.y >= recoveryY || Time.time < recoveryLockUntil)
                return;

            if (IsPlayer)
            {
                if (checkpoints == null)
                    checkpoints = GetComponent<StageCheckpointSystem>();

                checkpoints?.BeginRecovery();
                recoveryLockUntil = Time.time + 0.75f;
                return;
            }

            if (!enemyRecovering)
                StartCoroutine(RecoverEnemy());
        }

        private IEnumerator RecoverEnemy()
        {
            enemyRecovering = true;

            // Let the fall read visually before snapping an AI actor home.
            yield return new WaitForSeconds(0.16f);

            bool wasEnabled = controller != null && controller.enabled;
            if (wasEnabled)
                controller.enabled = false;

            transform.position = homePosition;

            if (wasEnabled)
                controller.enabled = true;

            if (knockback != null)
                knockback.ResetMotion();

            recoveryLockUntil = Time.time + 1.25f;
            enemyRecovering = false;
        }
    }
}
