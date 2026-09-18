using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.AI
{
    [RequireComponent(typeof(Health))]
    public sealed class BossPhaseController : MonoBehaviour
    {
        [SerializeField] private bool forgeVariant;
        [SerializeField] private float burstInterval = 3.4f;

        private Health health;
        private SimpleEnemyBrain brain;
        private BossPulseAttack pulse;
        private int phase = 1;
        private float nextBurst;

        public void Configure(bool isForgeVariant)
        {
            forgeVariant = isForgeVariant;
        }

        private void Awake()
        {
            health = GetComponent<Health>();
            brain = GetComponent<SimpleEnemyBrain>();
            pulse = GetComponent<BossPulseAttack>();
        }

        private void OnEnable()
        {
            phase = 1;
            nextBurst = Time.time + 3.5f;
            ApplyPhase(1);
        }

        private void Update()
        {
            if (health == null || health.IsDead)
                return;

            float ratio = health.MaxHealth <= 0
                ? 1f
                : health.CurrentHealth / (float)health.MaxHealth;

            int wanted = ratio <= 0.33f ? 3 : ratio <= 0.66f ? 2 : 1;
            if (wanted != phase)
            {
                phase = wanted;
                ApplyPhase(phase);
            }

            if (phase >= 3 && Time.time >= nextBurst)
            {
                SpawnRadialBurst();
                nextBurst = Time.time + burstInterval;
            }
        }

        private void ApplyPhase(int value)
        {
            if (brain != null)
            {
                if (forgeVariant)
                {
                    if (value == 1) brain.Configure(2.15f, 20f, 2.1f, 1.00f, 0.52f, 31);
                    if (value == 2) brain.Configure(2.45f, 20f, 2.1f, 0.86f, 0.46f, 34);
                    if (value == 3) brain.Configure(2.85f, 21f, 2.2f, 0.72f, 0.40f, 38);
                }
                else
                {
                    if (value == 1) brain.Configure(2.05f, 20f, 2.0f, 1.10f, 0.58f, 26);
                    if (value == 2) brain.Configure(2.35f, 20f, 2.0f, 0.94f, 0.50f, 29);
                    if (value == 3) brain.Configure(2.70f, 21f, 2.1f, 0.78f, 0.43f, 33);
                }
            }

            if (pulse != null)
            {
                if (forgeVariant)
                {
                    if (value == 1) pulse.Configure(4.4f, 4.2f, 0.72f, 24);
                    if (value == 2) pulse.Configure(4.8f, 3.4f, 0.62f, 28);
                    if (value == 3) pulse.Configure(5.2f, 2.7f, 0.52f, 32);
                }
                else
                {
                    if (value == 1) pulse.Configure(4.2f, 4.5f, 0.80f, 22);
                    if (value == 2) pulse.Configure(4.6f, 3.6f, 0.68f, 25);
                    if (value == 3) pulse.Configure(5.0f, 2.9f, 0.56f, 29);
                }
            }

            burstInterval = forgeVariant
                ? (value >= 3 ? 2.7f : 3.4f)
                : (value >= 3 ? 3.0f : 3.7f);
        }

        private void SpawnRadialBurst()
        {
            int projectileCount = forgeVariant ? 10 : 8;
            int damage = forgeVariant ? 15 : 12;
            float speed = forgeVariant ? 8.5f : 7.5f;

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = i / (float)projectileCount * Mathf.PI * 2f;
                Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

                GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                projectile.name = forgeVariant ? "ForgeBossProjectile" : "BossProjectile";
                projectile.transform.position = transform.position + Vector3.up * 1.0f + direction * 1.2f;
                projectile.transform.localScale = Vector3.one * 0.34f;

                Collider collider = projectile.GetComponent<Collider>();
                if (collider != null)
                    Destroy(collider);

                Renderer renderer = projectile.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.material.color = forgeVariant
                        ? new Color(1f, 0.28f, 0.04f)
                        : new Color(0.68f, 0.30f, 1f);

                SimpleProjectile logic = projectile.AddComponent<SimpleProjectile>();
                logic.Initialize(gameObject, direction, speed, damage, 3.0f, 0.28f);
            }
        }
    }
}
