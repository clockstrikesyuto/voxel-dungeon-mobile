using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FallRecovery : MonoBehaviour
    {
        [SerializeField] private float recoveryY = -8f;
        [SerializeField] private float safeSampleInterval = 0.20f;
        [SerializeField] private float safeGroundedTime = 0.12f;

        private CharacterController controller;
        private TopDownPlayerMotor motor;
        private Health health;

        private Vector3 lastSafePosition;
        private float nextSampleTime;
        private float groundedSince = -1f;
        private bool hasSafePosition;
        private bool recovering;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            motor = GetComponent<TopDownPlayerMotor>();
            health = GetComponent<Health>();

            lastSafePosition = transform.position;
            hasSafePosition = true;
        }

        private void Update()
        {
            if (recovering)
                return;

            TrackSafePosition();

            if (transform.position.y < recoveryY)
                Recover();
        }

        private void TrackSafePosition()
        {
            if (controller == null)
                return;

            if (controller.isGrounded)
            {
                if (groundedSince < 0f)
                    groundedSince = Time.time;

                if (Time.time >= nextSampleTime &&
                    Time.time - groundedSince >= safeGroundedTime)
                {
                    Vector3 candidate = transform.position;

                    // Keep the recovery point slightly above the surface to avoid
                    // spawning inside a floor collider.
                    candidate.y += 0.18f;

                    lastSafePosition = candidate;
                    hasSafePosition = true;
                    nextSampleTime = Time.time + safeSampleInterval;
                }
            }
            else
            {
                groundedSince = -1f;
            }
        }

        private void Recover()
        {
            recovering = true;

            Vector3 target = hasSafePosition
                ? lastSafePosition
                : Vector3.up * 1.5f;

            bool wasEnabled = controller != null && controller.enabled;
            if (controller != null && wasEnabled)
                controller.enabled = false;

            transform.position = target;

            if (motor != null)
                motor.ResetMotion();

            if (controller != null && wasEnabled)
                controller.enabled = true;

            // Falling is a positioning failure, not an instant death.
            // Keep the player alive and return control immediately.
            if (health != null && health.IsDead)
                health.ResetHealth();

            groundedSince = -1f;
            nextSampleTime = Time.time + safeSampleInterval;
            recovering = false;
        }
    }
}
