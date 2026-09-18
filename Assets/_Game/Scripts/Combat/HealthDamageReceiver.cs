using System.Collections;
using UnityEngine;

namespace VoxelDungeon.Combat
{
    [RequireComponent(typeof(Health))]
    public sealed class HealthDamageReceiver : MonoBehaviour, IDamageReceiver
    {
        [SerializeField, Min(1f)] private float hitPulseScale = 1.14f;
        [SerializeField, Min(0.01f)] private float hitPulseDuration = 0.08f;

        private Health health;
        private Vector3 baseScale;
        private Coroutine pulseRoutine;

        public bool CanReceiveDamage => health != null && !health.IsDead;

        private void Awake()
        {
            health = GetComponent<Health>();
            baseScale = transform.localScale;
        }

        public void ReceiveDamage(in DamagePayload payload)
        {
            if (!CanReceiveDamage) return;

            health.ApplyDamage(payload.Amount);

            if (pulseRoutine != null)
                StopCoroutine(pulseRoutine);

            pulseRoutine = StartCoroutine(HitPulse());
        }

        private IEnumerator HitPulse()
        {
            transform.localScale = baseScale * hitPulseScale;
            yield return new WaitForSeconds(hitPulseDuration);
            transform.localScale = baseScale;
            pulseRoutine = null;
        }
    }
}
