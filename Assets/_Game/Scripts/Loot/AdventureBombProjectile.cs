using System.Collections.Generic;
using UnityEngine;
using VoxelDungeon.AI;
using VoxelDungeon.Combat;

namespace VoxelDungeon.Loot
{
    public sealed class AdventureBombProjectile : MonoBehaviour
    {
        private GameObject owner;
        private Vector3 direction;
        private float speed;
        private int damage;
        private float radius;
        private float remaining;
        private bool frost;
        private bool exploded;

        public void Initialize(
            GameObject source,
            Vector3 moveDirection,
            int attackDamage,
            float blastRadius,
            float moveSpeed,
            bool isFrost)
        {
            owner = source;
            direction = moveDirection.normalized;
            damage = Mathf.Max(1, attackDamage);
            radius = Mathf.Max(1f, blastRadius);
            speed = Mathf.Max(1f, moveSpeed);
            frost = isFrost;
            remaining = 1.15f;
        }

        private void Update()
        {
            if (exploded)
                return;

            float distance = speed * Time.deltaTime;
            Vector3 origin = transform.position;

            if (Physics.SphereCast(
                    origin,
                    0.24f,
                    direction,
                    out RaycastHit hit,
                    distance,
                    Physics.AllLayers,
                    QueryTriggerInteraction.Ignore))
            {
                if (owner == null ||
                    (hit.collider.gameObject != owner &&
                     !hit.collider.transform.IsChildOf(owner.transform)))
                {
                    transform.position = hit.point;
                    Explode();
                    return;
                }
            }

            transform.position += direction * distance;
            remaining -= Time.deltaTime;

            if (remaining <= 0f)
                Explode();
        }

        private void Explode()
        {
            if (exploded)
                return;

            exploded = true;
            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                radius,
                Physics.AllLayers,
                QueryTriggerInteraction.Ignore);

            HashSet<HealthDamageReceiver> damaged = new HashSet<HealthDamageReceiver>();

            foreach (Collider hit in hits)
            {
                if (hit == null)
                    continue;

                HealthDamageReceiver receiver = hit.GetComponentInParent<HealthDamageReceiver>();
                if (receiver == null ||
                    !receiver.CanReceiveDamage ||
                    receiver.gameObject == owner ||
                    !damaged.Add(receiver))
                    continue;

                Vector3 delta = receiver.transform.position - transform.position;
                delta.y = 0f;

                receiver.ReceiveDamage(new DamagePayload(
                    owner,
                    damage,
                    receiver.transform.position,
                    delta.sqrMagnitude > 0.001f ? delta.normalized : direction,
                    false));

                if (frost)
                {
                    SimpleEnemyBrain brain = receiver.GetComponent<SimpleEnemyBrain>();
                    if (brain != null)
                        brain.ApplySlow(2.6f, 0.55f);
                }
            }

            GameObject pulse = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pulse.name = frost ? "FrostBurst" : "FireBurst";
            pulse.transform.position = transform.position;
            pulse.transform.localScale = Vector3.one * radius * 1.35f;

            Collider pulseCollider = pulse.GetComponent<Collider>();
            if (pulseCollider != null)
                Destroy(pulseCollider);

            Renderer renderer = pulse.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = frost
                    ? new Color(0.25f, 0.75f, 1f, 0.75f)
                    : new Color(1f, 0.28f, 0.05f, 0.75f);
            }

            Destroy(pulse, 0.18f);
            Destroy(gameObject);
        }
    }
}
