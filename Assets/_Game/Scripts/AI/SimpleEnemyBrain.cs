using System.Collections;
using UnityEngine;
using VoxelDungeon.Combat;
using VoxelDungeon.Player;

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
        [SerializeField, Min(0.05f)] private float attackWindup = 0.34f;
        [SerializeField, Min(1)] private int attackDamage = 10;
        [SerializeField] private Transform target;

        private CharacterController controller;
        private Health health;
        private HealthDamageReceiver targetReceiver;
        private Renderer cachedRenderer;
        private MaterialPropertyBlock block;
        private string colorProperty;
        private Color baseColor = Color.white;

        private float nextAttackTime;
        private float verticalVelocity;
        private bool attacking;
        private bool dying;
        private float slowUntil;
        private float slowMultiplier = 1f;

        public void SetTarget(Transform value)
        {
            target = value;
            ResolveTargetReceiver();
        }

        public void ApplySlow(float duration, float multiplier)
        {
            slowUntil = Mathf.Max(slowUntil, Time.time + Mathf.Max(0.1f, duration));
            slowMultiplier = Mathf.Clamp(multiplier, 0.2f, 1f);
        }

        public void BeginEncounter()
        {
            StopAllCoroutines();
            attacking = false;
            dying = false;
            verticalVelocity = 0f;
            nextAttackTime = Time.time + 0.20f;
            aggroRange = Mathf.Max(aggroRange, 40f);

            EnsureTarget();
            enabled = true;
        }

        public void Configure(
            float speed,
            float aggro,
            float range,
            float cooldown,
            float windup,
            int damage)
        {
            moveSpeed = Mathf.Max(0.1f, speed);
            aggroRange = Mathf.Max(0.1f, aggro);
            attackRange = Mathf.Max(0.1f, range);
            attackCooldown = Mathf.Max(0.05f, cooldown);
            attackWindup = Mathf.Max(0.05f, windup);
            attackDamage = Mathf.Max(1, damage);
        }

        private void Awake()
        {
            if (GetComponent<FallRecovery>() == null)
                gameObject.AddComponent<FallRecovery>();
            controller = GetComponent<CharacterController>();
            health = GetComponent<Health>();

            cachedRenderer = GetComponentInChildren<Renderer>();
            if (cachedRenderer != null && cachedRenderer.sharedMaterial != null)
            {
                if (cachedRenderer.sharedMaterial.HasProperty("_BaseColor"))
                {
                    colorProperty = "_BaseColor";
                    baseColor = cachedRenderer.sharedMaterial.GetColor(colorProperty);
                }
                else if (cachedRenderer.sharedMaterial.HasProperty("_Color"))
                {
                    colorProperty = "_Color";
                    baseColor = cachedRenderer.sharedMaterial.GetColor(colorProperty);
                }

                block = new MaterialPropertyBlock();
            }
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
            if (dying || health == null || health.IsDead)
                return;

            if (target == null)
            {
                EnsureTarget();
                if (target == null)
                    return;
            }

            if (attacking)
                return;

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

                Vector3 safeDirection = ResolveSafeDirection(direction);
                float speedMultiplier = Time.time < slowUntil ? slowMultiplier : 1f;

                controller.Move(
                    (safeDirection * moveSpeed * speedMultiplier +
                     Vector3.up * verticalVelocity) * Time.deltaTime);
                return;
            }

            if (Time.time >= nextAttackTime)
                StartCoroutine(AttackSequence());
        }

        private Vector3 ResolveSafeDirection(Vector3 desired)
        {
            if (desired.sqrMagnitude < 0.001f)
                return Vector3.zero;

            if (DirectionIsSafe(desired))
                return desired;

            float[] angles = { 38f, -38f, 72f, -72f, 110f, -110f };
            foreach (float angle in angles)
            {
                Vector3 candidate = Quaternion.Euler(0f, angle, 0f) * desired;
                if (DirectionIsSafe(candidate))
                    return candidate.normalized;
            }

            return Vector3.zero;
        }

        private bool DirectionIsSafe(Vector3 direction)
        {
            direction.y = 0f;
            direction.Normalize();

            Vector3 groundProbe =
                transform.position + Vector3.up * 1.1f + direction * 0.95f;

            bool hasGround = Physics.Raycast(
                groundProbe,
                Vector3.down,
                3.0f,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore);

            if (!hasGround)
                return false;

            Vector3 obstacleOrigin = transform.position + Vector3.up * 0.85f;
            if (Physics.Raycast(
                    obstacleOrigin,
                    direction,
                    out RaycastHit hit,
                    0.95f,
                    Physics.AllLayers,
                    QueryTriggerInteraction.Ignore))
            {
                if (hit.transform != transform &&
                    !hit.transform.IsChildOf(transform) &&
                    (target == null &&
                     !hit.transform.IsChildOf(target) ||
                     target != null &&
                     hit.transform != target &&
                     !hit.transform.IsChildOf(target)))
                    return false;
            }

            return true;
        }

        private IEnumerator AttackSequence()
        {
            attacking = true;
            SetTelegraphColor(new Color(1f, 0.32f, 0.12f, 1f));

            yield return new WaitForSeconds(attackWindup);

            if (!dying && target != null)
            {
                Vector3 toTarget = target.position - transform.position;
                toTarget.y = 0f;
                float distance = toTarget.magnitude;

                if (distance <= attackRange + 0.35f)
                {
                    if (targetReceiver == null)
                        ResolveTargetReceiver();

                    if (targetReceiver != null && targetReceiver.CanReceiveDamage)
                    {
                        Vector3 direction = toTarget.sqrMagnitude > 0.001f
                            ? toTarget.normalized
                            : transform.forward;

                        targetReceiver.ReceiveDamage(new DamagePayload(
                            gameObject,
                            attackDamage,
                            target.position,
                            direction,
                            false));
                    }
                }
            }

            RestoreColor();
            nextAttackTime = Time.time + attackCooldown;
            attacking = false;
        }

        private void SetTelegraphColor(Color color)
        {
            if (cachedRenderer == null || block == null || string.IsNullOrEmpty(colorProperty))
                return;

            cachedRenderer.GetPropertyBlock(block);
            block.SetColor(colorProperty, color);
            cachedRenderer.SetPropertyBlock(block);
        }

        private void RestoreColor()
        {
            if (cachedRenderer == null || block == null || string.IsNullOrEmpty(colorProperty))
                return;

            cachedRenderer.GetPropertyBlock(block);
            block.SetColor(colorProperty, baseColor);
            cachedRenderer.SetPropertyBlock(block);
        }

        private void OnDied()
        {
            if (dying)
                return;

            dying = true;
            StopAllCoroutines();
            RestoreColor();

            if (controller != null)
                controller.enabled = false;

            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            Vector3 startScale = transform.localScale;
            float duration = 0.28f;
            float age = 0f;

            while (age < duration)
            {
                age += Time.deltaTime;
                float t = Mathf.Clamp01(age / duration);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                transform.Rotate(Vector3.up, 360f * Time.deltaTime);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
