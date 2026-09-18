using System.Collections;
using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.Player
{
    public sealed class PlayerCombat : MonoBehaviour
    {
        [SerializeField, Min(1)] private int meleeDamage = 34;
        [SerializeField, Min(0.1f)] private float meleeRadius = 1.15f;
        [SerializeField, Min(0f)] private float meleeForwardOffset = 1.1f;
        [SerializeField, Min(0.05f)] private float meleeCooldown = 0.45f;
        [SerializeField, Min(1f)] private float attackPulseScale = 1.12f;
        [SerializeField, Min(0.01f)] private float attackPulseDuration = 0.07f;

        private readonly Collider[] hitBuffer = new Collider[24];
        private float nextMeleeTime;
        private Vector3 baseScale;
        private Coroutine pulseRoutine;

        private void Awake()
        {
            baseScale = transform.localScale;
        }

        public bool TryMelee()
        {
            if (Time.time < nextMeleeTime)
                return false;

            nextMeleeTime = Time.time + meleeCooldown;

            if (pulseRoutine != null)
                StopCoroutine(pulseRoutine);
            pulseRoutine = StartCoroutine(AttackPulse());

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
                if (receiver == null || !receiver.CanReceiveDamage)
                    continue;

                Vector3 direction = (hit.transform.position - transform.position).normalized;
                receiver.ReceiveDamage(new DamagePayload(
                    gameObject,
                    meleeDamage,
                    hit.ClosestPoint(center),
                    direction,
                    false));
            }

            return true;
        }

        private IEnumerator AttackPulse()
        {
            transform.localScale = baseScale * attackPulseScale;
            yield return new WaitForSeconds(attackPulseDuration);
            transform.localScale = baseScale;
            pulseRoutine = null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(
                transform.position + Vector3.up * 0.9f + transform.forward * meleeForwardOffset,
                meleeRadius);
        }
#endif
    }
}
