using System.Collections;
using UnityEngine;
using VoxelDungeon.Combat;
using VoxelDungeon.Presentation;

namespace VoxelDungeon.Player
{
    [RequireComponent(typeof(Health))]
    public sealed class PlayerCombat : MonoBehaviour
    {
        [Header("Melee")]
        [SerializeField, Min(1)] private int meleeDamage = 34;
        [SerializeField, Min(0.1f)] private float meleeRadius = 1.15f;
        [SerializeField, Min(0f)] private float meleeForwardOffset = 1.1f;
        [SerializeField, Min(0.05f)] private float meleeCooldown = 0.45f;

        [Header("Ranged")]
        [SerializeField, Min(1)] private int rangedDamage = 22;
        [SerializeField, Min(0.1f)] private float rangedSpeed = 12f;
        [SerializeField, Min(0.05f)] private float rangedCooldown = 0.7f;
        [SerializeField, Min(1f)] private float aimAssistRange = 9f;
        [SerializeField, Range(-1f, 1f)] private float aimAssistMinDot = 0.15f;

        [Header("Potion")]
        [SerializeField, Min(1)] private int potionHeal = 45;
        [SerializeField, Min(0.1f)] private float potionCooldown = 8f;

        [Header("Feedback")]
        [SerializeField, Min(1f)] private float attackPulseScale = 1.12f;
        [SerializeField, Min(0.01f)] private float attackPulseDuration = 0.07f;

        private readonly Collider[] hitBuffer = new Collider[24];
        private readonly Collider[] aimBuffer = new Collider[32];

        private Health health;
        private PlayerProgress progress;
        private float nextMeleeTime;
        private float nextRangedTime;
        private float nextPotionTime;
        private Vector3 baseScale;
        private Coroutine pulseRoutine;

        private void Awake()
        {
            health = GetComponent<Health>();
            progress = GetComponent<PlayerProgress>();
            baseScale = transform.localScale;
        }

        public bool TryMelee()
        {
            if (Time.time < nextMeleeTime)
                return false;

            nextMeleeTime = Time.time + meleeCooldown;
            PlayAttackPulse();
            MeleeArcVisual.Spawn(transform);

            Vector3 center = transform.position + Vector3.up * 0.9f + transform.forward * meleeForwardOffset;
            int count = Physics.OverlapSphereNonAlloc(
                center,
                meleeRadius,
                hitBuffer,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                Collider hit = hitBuffer[i];
                if (hit == null || hit.transform.IsChildOf(transform))
                    continue;

                HealthDamageReceiver receiver = hit.GetComponentInParent<HealthDamageReceiver>();
                if (receiver == null || !receiver.CanReceiveDamage || receiver.gameObject == gameObject)
                    continue;

                Vector3 direction = hit.transform.position - transform.position;
                direction.y = 0f;

                receiver.ReceiveDamage(new DamagePayload(
                    gameObject,
                    meleeDamage + (progress != null ? progress.MeleePowerBonus : 0),
                    hit.ClosestPoint(center),
                    direction.normalized,
                    false));
            }

            return true;
        }

        public bool TryRanged()
        {
            if (Time.time < nextRangedTime)
                return false;

            nextRangedTime = Time.time + rangedCooldown;
            PlayAttackPulse();

            Vector3 direction = FindAimDirection();

            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "PlayerProjectile";
            projectile.transform.position = transform.position + Vector3.up * 0.9f + direction * 0.9f;
            projectile.transform.localScale = Vector3.one * 0.32f;

            Collider primitiveCollider = projectile.GetComponent<Collider>();
            if (primitiveCollider != null)
                Destroy(primitiveCollider);

            Renderer renderer = projectile.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = new Color(0.35f, 0.8f, 1f, 1f);

            SimpleProjectile projectileLogic = projectile.AddComponent<SimpleProjectile>();
            projectileLogic.Initialize(
                gameObject,
                direction,
                rangedSpeed,
                rangedDamage + (progress != null ? progress.RangedPowerBonus : 0));

            transform.forward = direction;
            return true;
        }

        public bool TryPotion()
        {
            if (Time.time < nextPotionTime || health == null)
                return false;

            if (!health.Heal(potionHeal))
                return false;

            nextPotionTime = Time.time + potionCooldown;
            StartCoroutine(PotionPulse());
            return true;
        }

        private Vector3 FindAimDirection()
        {
            Vector3 forward = transform.forward;
            Vector3 bestDirection = forward;
            float bestScore = aimAssistMinDot;

            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                aimAssistRange,
                aimBuffer,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                Collider candidate = aimBuffer[i];
                if (candidate == null || candidate.transform.IsChildOf(transform))
                    continue;

                HealthDamageReceiver receiver = candidate.GetComponentInParent<HealthDamageReceiver>();
                if (receiver == null || !receiver.CanReceiveDamage || receiver.gameObject == gameObject)
                    continue;

                Vector3 toTarget = receiver.transform.position - transform.position;
                toTarget.y = 0f;

                float distance = toTarget.magnitude;
                if (distance < 0.001f)
                    continue;

                Vector3 dir = toTarget / distance;
                float dot = Vector3.Dot(forward, dir);
                float distanceBonus = 1f - Mathf.Clamp01(distance / aimAssistRange);
                float score = dot + distanceBonus * 0.25f;

                if (dot >= aimAssistMinDot && score > bestScore)
                {
                    bestScore = score;
                    bestDirection = dir;
                }
            }

            return bestDirection.normalized;
        }

        private void PlayAttackPulse()
        {
            if (pulseRoutine != null)
                StopCoroutine(pulseRoutine);

            pulseRoutine = StartCoroutine(AttackPulse());
        }

        private IEnumerator AttackPulse()
        {
            transform.localScale = baseScale * attackPulseScale;
            yield return new WaitForSeconds(attackPulseDuration);
            transform.localScale = baseScale;
            pulseRoutine = null;
        }

        private IEnumerator PotionPulse()
        {
            Vector3 original = transform.localScale;
            transform.localScale = original * 1.18f;
            yield return new WaitForSeconds(0.12f);
            transform.localScale = original;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(
                transform.position + Vector3.up * 0.9f + transform.forward * meleeForwardOffset,
                meleeRadius);

            Gizmos.DrawWireSphere(transform.position, aimAssistRange);
        }
#endif
    }
}
