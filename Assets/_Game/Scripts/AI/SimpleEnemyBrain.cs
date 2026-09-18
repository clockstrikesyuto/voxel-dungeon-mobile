using System.Collections;
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

        public void SetTarget(Transform value)
        {
            target = value;
            ResolveTargetReceiver();
        }

        private void Awake()
        {
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
                controller.Move((direction * moveSpeed + Vector3.up * verticalVelocity) * Time.deltaTime);
                return;
            }

            if (Time.time >= nextAttackTime)
                StartCoroutine(AttackSequence());
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
