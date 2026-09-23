using System.Collections;
using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.AI
{
    [RequireComponent(typeof(Health))]
    public sealed class BossPulseAttack : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float radius = 4.2f;
        [SerializeField, Min(0.5f)] private float interval = 4.5f;
        [SerializeField, Min(0.1f)] private float windup = 0.8f;
        [SerializeField, Min(1)] private int damage = 22;

        private Health health;
        private Transform player;
        private HealthDamageReceiver receiver;
        private float nextTime;
        private LineRenderer ring;
        private bool encounterStarted;

        public void BeginEncounter()
        {
            encounterStarted = true;
            nextTime = Time.time + 0.35f;
            if (ring != null)
                SetRingVisible(false);
        }

        public void Configure(float attackRadius, float attackInterval, float attackWindup, int attackDamage)
        {
            radius = Mathf.Max(1f, attackRadius);
            interval = Mathf.Max(0.5f, attackInterval);
            windup = Mathf.Max(0.1f, attackWindup);
            damage = Mathf.Max(1, attackDamage);
        }

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void Start()
        {
            if (!encounterStarted)
                nextTime = Time.time + 1.5f;

            BuildRing();
            SetRingVisible(false);
        }

        private void Update()
        {
            if (health == null || health.IsDead)
                return;

            if (player == null)
            {
                GameObject go = GameObject.Find("Player_Debug");
                if (go != null)
                {
                    player = go.transform;
                    receiver = go.GetComponent<HealthDamageReceiver>();
                }
            }

            if (player != null && Time.time >= nextTime)
                StartCoroutine(Pulse());
        }

        private IEnumerator Pulse()
        {
            nextTime = float.MaxValue;
            SetRingVisible(true);

            float age = 0f;
            while (age < windup)
            {
                age += Time.deltaTime;
                float t = Mathf.Clamp01(age / windup);
                UpdateRingScale(Mathf.Lerp(0.25f, radius, t));
                yield return null;
            }

            if (player != null && receiver != null && receiver.CanReceiveDamage)
            {
                Vector3 delta = player.position - transform.position;
                delta.y = 0f;

                if (delta.magnitude <= radius)
                {
                    receiver.ReceiveDamage(new DamagePayload(
                        gameObject,
                        damage,
                        player.position,
                        delta.sqrMagnitude > 0.001f ? delta.normalized : transform.forward,
                        false));
                }
            }

            SetRingVisible(false);
            nextTime = Time.time + interval;
        }

        private void BuildRing()
        {
            GameObject go = new GameObject("BossPulseRing");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0f, -0.95f, 0f);

            ring = go.AddComponent<LineRenderer>();
            ring.useWorldSpace = false;
            ring.loop = true;
            ring.positionCount = 48;
            ring.startWidth = 0.12f;
            ring.endWidth = 0.12f;
            ring.material = new Material(Shader.Find("Sprites/Default"));
            ring.startColor = new Color(1f, 0.1f, 0.08f, 0.9f);
            ring.endColor = ring.startColor;
            UpdateRingScale(radius);
        }

        private void UpdateRingScale(float currentRadius)
        {
            if (ring == null)
                return;

            for (int i = 0; i < ring.positionCount; i++)
            {
                float angle = i / (float)ring.positionCount * Mathf.PI * 2f;
                ring.SetPosition(i, new Vector3(Mathf.Cos(angle) * currentRadius, 0f, Mathf.Sin(angle) * currentRadius));
            }
        }

        private void SetRingVisible(bool visible)
        {
            if (ring != null)
                ring.enabled = visible;
        }
    }
}
