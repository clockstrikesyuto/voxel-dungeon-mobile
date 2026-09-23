using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.AI
{
    [RequireComponent(typeof(Health))]
    public sealed class BossPhaseController : MonoBehaviour
    {
        public enum BossVariant
        {
            Stone,
            Forge,
            Void
        }

        [SerializeField] private BossVariant variant;
        [SerializeField] private float burstInterval = 3.4f;

        private Health health;
        private SimpleEnemyBrain brain;
        private BossPulseAttack pulse;
        private Renderer coreRenderer;
        private MaterialPropertyBlock coreBlock;
        private string coreColorProperty;
        private int phase = 1;
        private float nextBurst;

        public void Configure(BossVariant bossVariant)
        {
            variant = bossVariant;
        }

        public void Configure(bool isForgeVariant)
        {
            variant = isForgeVariant ? BossVariant.Forge : BossVariant.Stone;
        }

        private void Awake()
        {
            health = GetComponent<Health>();
            brain = GetComponent<SimpleEnemyBrain>();
            pulse = GetComponent<BossPulseAttack>();

            Transform core = transform.Find("BossVisual/Core");
            if (core != null)
            {
                coreRenderer = core.GetComponent<Renderer>();
                if (coreRenderer != null && coreRenderer.sharedMaterial != null)
                {
                    coreColorProperty = coreRenderer.sharedMaterial.HasProperty("_BaseColor")
                        ? "_BaseColor"
                        : coreRenderer.sharedMaterial.HasProperty("_Color")
                            ? "_Color"
                            : null;

                    coreBlock = new MaterialPropertyBlock();
                }
            }
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
            UpdateCoreVisual(value);

            if (brain != null)
            {
                switch (variant)
                {
                    case BossVariant.Forge:
                        if (value == 1) brain.Configure(2.15f, 20f, 2.1f, 1.00f, 0.52f, 31);
                        if (value == 2) brain.Configure(2.45f, 20f, 2.1f, 0.86f, 0.46f, 34);
                        if (value == 3) brain.Configure(2.85f, 21f, 2.2f, 0.72f, 0.40f, 38);
                        break;

                    case BossVariant.Void:
                        if (value == 1) brain.Configure(2.30f, 21f, 2.15f, 0.90f, 0.48f, 34);
                        if (value == 2) brain.Configure(2.65f, 22f, 2.20f, 0.76f, 0.41f, 38);
                        if (value == 3) brain.Configure(3.05f, 23f, 2.25f, 0.64f, 0.35f, 43);
                        break;

                    default:
                        if (value == 1) brain.Configure(2.05f, 20f, 2.0f, 1.10f, 0.58f, 26);
                        if (value == 2) brain.Configure(2.35f, 20f, 2.0f, 0.94f, 0.50f, 29);
                        if (value == 3) brain.Configure(2.70f, 21f, 2.1f, 0.78f, 0.43f, 33);
                        break;
                }
            }

            if (pulse != null)
            {
                switch (variant)
                {
                    case BossVariant.Forge:
                        if (value == 1) pulse.Configure(4.4f, 4.2f, 0.72f, 24);
                        if (value == 2) pulse.Configure(4.8f, 3.4f, 0.62f, 28);
                        if (value == 3) pulse.Configure(5.2f, 2.7f, 0.52f, 32);
                        break;

                    case BossVariant.Void:
                        if (value == 1) pulse.Configure(4.8f, 3.8f, 0.62f, 29);
                        if (value == 2) pulse.Configure(5.2f, 3.0f, 0.54f, 33);
                        if (value == 3) pulse.Configure(5.6f, 2.35f, 0.46f, 37);
                        break;

                    default:
                        if (value == 1) pulse.Configure(4.2f, 4.5f, 0.80f, 22);
                        if (value == 2) pulse.Configure(4.6f, 3.6f, 0.68f, 25);
                        if (value == 3) pulse.Configure(5.0f, 2.9f, 0.56f, 29);
                        break;
                }
            }

            burstInterval = variant switch
            {
                BossVariant.Forge => value >= 3 ? 2.7f : 3.4f,
                BossVariant.Void => value >= 3 ? 2.2f : 3.0f,
                _ => value >= 3 ? 3.0f : 3.7f
            };
        }

        private void UpdateCoreVisual(int value)
        {
            if (coreRenderer == null || coreBlock == null || string.IsNullOrEmpty(coreColorProperty))
                return;

            Color color = variant switch
            {
                BossVariant.Forge => value switch
                {
                    1 => new Color(1f, 0.22f, 0.05f),
                    2 => new Color(1f, 0.52f, 0.06f),
                    _ => new Color(1f, 0.88f, 0.28f)
                },
                BossVariant.Void => value switch
                {
                    1 => new Color(0.28f, 0.90f, 0.62f),
                    2 => new Color(0.38f, 0.68f, 1f),
                    _ => new Color(0.76f, 0.42f, 1f)
                },
                _ => value switch
                {
                    1 => new Color(1f, 0.12f, 0.07f),
                    2 => new Color(0.82f, 0.28f, 1f),
                    _ => new Color(0.34f, 0.84f, 1f)
                }
            };

            coreRenderer.GetPropertyBlock(coreBlock);
            coreBlock.SetColor(coreColorProperty, color);

            if (coreRenderer.sharedMaterial.HasProperty("_EmissionColor"))
                coreBlock.SetColor("_EmissionColor", color * (value == 3 ? 3.4f : 2.4f));

            coreRenderer.SetPropertyBlock(coreBlock);
        }

        private void SpawnRadialBurst()
        {
            int projectileCount = variant switch
            {
                BossVariant.Forge => 10,
                BossVariant.Void => 12,
                _ => 8
            };

            int damage = variant switch
            {
                BossVariant.Forge => 15,
                BossVariant.Void => 18,
                _ => 12
            };

            float speed = variant switch
            {
                BossVariant.Forge => 8.5f,
                BossVariant.Void => 9.0f,
                _ => 7.5f
            };

            Color color = variant switch
            {
                BossVariant.Forge => new Color(1f, 0.28f, 0.04f),
                BossVariant.Void => new Color(0.50f, 0.92f, 0.72f),
                _ => new Color(0.68f, 0.30f, 1f)
            };

            string projectileName = variant switch
            {
                BossVariant.Forge => "ForgeBossProjectile",
                BossVariant.Void => "VoidBossProjectile",
                _ => "BossProjectile"
            };

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = i / (float)projectileCount * Mathf.PI * 2f;
                Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

                GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                projectile.name = projectileName;
                projectile.transform.position = transform.position + Vector3.up * 1.0f + direction * 1.2f;
                projectile.transform.localScale = Vector3.one * 0.34f;

                Collider collider = projectile.GetComponent<Collider>();
                if (collider != null)
                    Destroy(collider);

                Renderer renderer = projectile.GetComponent<Renderer>();
                if (renderer != null)
                    renderer.material.color = color;

                SimpleProjectile logic = projectile.AddComponent<SimpleProjectile>();
                logic.Initialize(gameObject, direction, speed, damage, 3.0f, 0.28f);
            }
        }
    }
}
