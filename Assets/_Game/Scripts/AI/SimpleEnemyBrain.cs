using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.AI
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Health))]
    public sealed class SimpleEnemyBrain : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float moveSpeed = 2.4f;
        [SerializeField, Min(0.1f)] private float aggroRange = 10f;
        [SerializeField, Min(0.1f)] private float attackRange = 1.6f;
        [SerializeField, Min(0.05f)] private float attackCooldown = 1.1f;
        [SerializeField, Min(1)] private int attackDamage = 10;
        [SerializeField] private Transform target;

        private CharacterController controller;
        private Health health;
        private HealthDamageReceiver targetReceiver;
        private float nextAttackTime;
        private float verticalVelocity;

        public void SetTarget(Transform value)
        {
            target = value;
            ResolveTargetReceiver();
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            health = GetComponent<Health>();
            ResolveTargetReceiver();
        }

        private void Start()
        {
            ResolveTargetReceiver();
        }

        private void ResolveTargetReceiver()
        {
            targetReceiver = target != null ? target.GetComponent<HealthDamageReceiver>() : null;
        }

        private void OnEnable()
        {
            if (health != null)
                health.Died += OnDied;
        }

        private void OnDisable()
        {
            if (health != null)
                health.Died -= OnDied;
        }

        private void Update()
        {
            if (health == null || health.IsDead || target == null)
                return;

            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;

            if (distance > aggroRange)
                return;

            if (distance > attackRange)
            {
                Vector3 direction = distance > 0.001f ? toTarget / distance : Vector3.zero;
                verticalVelocity = controller.isGrounded ? -2f : verticalVelocity - 25f * Time.deltaTime;
                controller.Move((direction * moveSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);

                if (direction.sqrMagnitude > 0.01f)
                    transform.forward = Vector3.Slerp(transform.forward, direction, 12f * Time.deltaTime);

                return;
            }

            if (targetReceiver == null)
                ResolveTargetReceiver();

            if (Time.time >= nextAttackTime && targetReceiver != null && targetReceiver.CanReceiveDamage)
            {
                nextAttackTime = Time.time + attackCooldown;
                Vector3 direction = toTarget.sqrMagnitude > 0.001f ? toTarget.normalized : transform.forward;
                targetReceiver.ReceiveDamage(new DamagePayload(
                    gameObject,
                    attackDamage,
                    target.position,
                    direction,
                    false));
            }
        }

        private void OnDied()
        {
            enabled = false;
            if (controller != null)
                controller.enabled = false;
            Destroy(gameObject, 0.35f);
        }
    }
}
