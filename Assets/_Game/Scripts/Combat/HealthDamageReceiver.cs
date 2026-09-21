using System.Collections;
using UnityEngine;
using VoxelDungeon.Presentation;
using VoxelDungeon.Core;
using VoxelDungeon.Player;

namespace VoxelDungeon.Combat
{
    [RequireComponent(typeof(Health))]
    public sealed class HealthDamageReceiver : MonoBehaviour, IDamageReceiver
    {
        [SerializeField, Min(1f)] private float hitPulseScale = 1.14f;
        [SerializeField, Min(0.01f)] private float hitPulseDuration = 0.08f;
        [SerializeField, Min(0.01f)] private float flashDuration = 0.07f;
        [SerializeField, Min(0f)] private float knockbackStrength = 3.2f;

        private Health health;
        private KnockbackMotor knockback;
        private Vector3 baseScale;
        private Coroutine pulseRoutine;
        private Coroutine flashRoutine;
        private Renderer[] cachedRenderers;
        private MaterialPropertyBlock propertyBlock;

        public bool CanReceiveDamage => health != null && !health.IsDead;

        private void Awake()
        {
            health = GetComponent<Health>();
            knockback = GetComponent<KnockbackMotor>();
            baseScale = transform.localScale;

            cachedRenderers = GetComponentsInChildren<Renderer>(true);
            propertyBlock = new MaterialPropertyBlock();
        }

        public void ReceiveDamage(in DamagePayload payload)
        {
            if (!CanReceiveDamage) return;

            int appliedAmount = payload.Amount;

            if (GetComponent<PlayerProgress>() != null)
            {
                PlayerBuffState buffs = GetComponent<PlayerBuffState>();
                int temporaryDefense = buffs != null ? buffs.DefenseBonus : 0;
                int defense = Mathf.Max(0, ProfileProgress.TotalDefense + temporaryDefense);
                float multiplier = 100f / (100f + defense * 4f);
                appliedAmount = Mathf.Max(1, Mathf.RoundToInt(payload.Amount * multiplier));
            }

            health.ApplyDamage(appliedAmount);
            FloatingDamageText.Spawn(payload.HitPoint, appliedAmount, payload.Critical);

            if (health.IsDead)
                return;

            if (knockback != null)
                knockback.AddImpulse(payload.Direction, knockbackStrength);

            if (pulseRoutine != null)
                StopCoroutine(pulseRoutine);
            pulseRoutine = StartCoroutine(HitPulse());

            if (flashRoutine != null)
                StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(HitFlash());
        }

        private IEnumerator HitPulse()
        {
            transform.localScale = baseScale * hitPulseScale;
            yield return new WaitForSeconds(hitPulseDuration);
            transform.localScale = baseScale;
            pulseRoutine = null;
        }

        private IEnumerator HitFlash()
        {
            if (cachedRenderers == null || propertyBlock == null)
                yield break;

            Color[] baseColors = new Color[cachedRenderers.Length];
            string[] props = new string[cachedRenderers.Length];

            for (int i = 0; i < cachedRenderers.Length; i++)
            {
                Renderer renderer = cachedRenderers[i];
                if (renderer == null || !renderer.enabled || renderer.sharedMaterial == null)
                    continue;

                string prop = renderer.sharedMaterial.HasProperty("_BaseColor")
                    ? "_BaseColor"
                    : renderer.sharedMaterial.HasProperty("_Color")
                        ? "_Color"
                        : null;

                if (string.IsNullOrEmpty(prop))
                    continue;

                props[i] = prop;
                baseColors[i] = renderer.sharedMaterial.GetColor(prop);
                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(prop, Color.white * 1.55f);
                renderer.SetPropertyBlock(propertyBlock);
            }

            yield return new WaitForSeconds(flashDuration);

            for (int i = 0; i < cachedRenderers.Length; i++)
            {
                Renderer renderer = cachedRenderers[i];
                if (renderer == null || string.IsNullOrEmpty(props[i]))
                    continue;

                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(props[i], baseColors[i]);
                renderer.SetPropertyBlock(propertyBlock);
            }

            flashRoutine = null;
        }
    }
}
