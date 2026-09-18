using UnityEngine;

namespace VoxelDungeon.Combat
{
    public sealed class SimpleProjectile : MonoBehaviour
    {
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
                HealthDamageReceiver receiver = hit.collider.GetComponentInParent<HealthDamageReceiver>();
                if (receiver != null && receiver.CanReceiveDamage && receiver.gameObject != source)
                {
                    receiver.ReceiveDamage(new DamagePayload(
                        source,
                        damage,
                        hit.point,
                        direction,
                        false));
                    Destroy(gameObject);
                    return;
                }

                if (hit.collider.gameObject != source)
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
    }
}
