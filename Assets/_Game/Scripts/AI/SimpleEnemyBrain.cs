using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.AI
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Health))]
    public sealed class SimpleEnemyBrain : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float moveSpeed = 2.4f;
        [SerializeField, Min(0.1f)] private float aggroRange = 12f;
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
        }

        private void Start()
        {
            EnsureTarget();
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

        private void EnsureTarget()
        {
            if (target == null)
            {
                GameObject player = GameObject.Find("Player_Debug");
                if (player != null)
                    target = player.transform;
            }

            ResolveTargetReceiver();
        }

        private void ResolveTargetReceiver()
        {
            targetReceiver = target != null ? target.GetComponent<HealthDamageReceiver>() : null;
        }

        private void Update()
        {
            if (health == null || health.IsDead)
                return;

            if (target == null)
            {
                EnsureTarget();
                if (target == null)
                    return;
            }

            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            float distance = toTarget.magnitude;

            if (distance > aggroRange)
                return;

            Vector3 direction = distance > 0.001f ? toTarget / distance : Vector3.zero;

            if (direction.sqrMagnitude > 0.01f)
                transform.forward = Vector3.Slerp(transform.forward, direction, 12f * Time.deltaTime);

            if (distance > attackRange)
            {
                verticalVelocity = controller.isGrounded ? -2f : verticalVelocity - 25f * Time.deltaTime;
                controller.Move((direction * moveSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
                return;
            }

            if (targetReceiver == null)
                ResolveTargetReceiver();

            if (Time.time >= nextAttackTime && targetReceiver != null && targetReceiver.CanReceiveDamage)
            {
                nextAttackTime = Time.time + attackCooldown;
                targetReceiver.ReceiveDamage(new DamagePayload(
                    gameObject,
                    attackDamage,
                    target.position,
                    direction.sqrMagnitude > 0.001f ? direction : transform.forward,
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
