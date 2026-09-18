using UnityEngine;

namespace VoxelDungeon.Combat
{
    public sealed class SimpleProjectile : MonoBehaviour
    {
        private readonly Collider[] overlapBuffer = new Collider[16];

        private GameObject source;
        private Vector3 direction;
        private float speed;
        private int damage;
        private float remainingLifetime;
        private float radius;

        public void Initialize(
            GameObject owner,
            Vector3 moveDirection,
            float projectileSpeed,
            int projectileDamage,
            float lifetime = 2.5f,
            float hitRadius = 0.28f)
        {
            source = owner;
            direction = moveDirection.normalized;
            speed = projectileSpeed;
            damage = projectileDamage;
            remainingLifetime = lifetime;
            radius = hitRadius;
        }

        private void Update()
        {
            if (TryHitOverlappingTarget())
                return;

            float distance = speed * Time.deltaTime;
            Vector3 origin = transform.position;

            if (Physics.SphereCast(
                    origin,
                    radius,
                    direction,
                    out RaycastHit hit,
                    distance,
                    Physics.AllLayers,
                    QueryTriggerInteraction.Ignore))
            {
                if (TryApplyHit(hit.collider, hit.point))
                    return;

                if (!BelongsToSource(hit.collider))
                {
                    Destroy(gameObject);
                    return;
                }
            }

            transform.position += direction * distance;
            remainingLifetime -= Time.deltaTime;

            if (remainingLifetime <= 0f)
                Destroy(gameObject);
        }

        private bool TryHitOverlappingTarget()
        {
            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                radius,
                overlapBuffer,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                Collider hit = overlapBuffer[i];
                if (hit == null || BelongsToSource(hit))
                    continue;

                if (TryApplyHit(hit, transform.position))
                    return true;
            }

            return false;
        }

        private bool TryApplyHit(Collider hit, Vector3 hitPoint)
        {
            HealthDamageReceiver receiver = hit.GetComponentInParent<HealthDamageReceiver>();
            if (receiver == null || !receiver.CanReceiveDamage || receiver.gameObject == source)
                return false;

            receiver.ReceiveDamage(new DamagePayload(
                source,
                damage,
                hitPoint,
                direction,
                false));

            Destroy(gameObject);
            return true;
        }

        private bool BelongsToSource(Collider hit)
        {
            if (source == null || hit == null)
                return false;

            return hit.gameObject == source || hit.transform.IsChildOf(source.transform);
        }
    }
}
