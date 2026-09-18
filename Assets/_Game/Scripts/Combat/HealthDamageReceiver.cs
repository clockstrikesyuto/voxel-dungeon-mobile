using UnityEngine;

namespace VoxelDungeon.Combat
{
    [RequireComponent(typeof(Health))]
    public sealed class HealthDamageReceiver : MonoBehaviour, IDamageReceiver
    {
        private Health health;

        public bool CanReceiveDamage => health != null && !health.IsDead;

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        public void ReceiveDamage(in DamagePayload payload)
        {
            if (!CanReceiveDamage) return;
            health.ApplyDamage(payload.Amount);
        }
    }
}
