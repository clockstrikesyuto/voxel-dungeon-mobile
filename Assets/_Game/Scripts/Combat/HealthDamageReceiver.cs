using System.Collections;
using UnityEngine;
using VoxelDungeon.Presentation;

namespace VoxelDungeon.Combat
{
    [RequireComponent(typeof(Health))]
    public sealed class HealthDamageReceiver : MonoBehaviour, IDamageReceiver
    {
        [SerializeField, Min(1f)] private float hitPulseScale = 1.14f;
        [SerializeField, Min(0.01f)] private float hitPulseDuration = 0.08f;
        [SerializeField, Min(0.01f)] private float flashDuration = 0.07f;

        private Health health;
        private Vector3 baseScale;
        private Coroutine pulseRoutine;
        private Coroutine flashRoutine;
        private Renderer cachedRenderer;
        private MaterialPropertyBlock propertyBlock;
        private string colorProperty;
        private Color baseColor = Color.white;

        public bool CanReceiveDamage => health != null && !health.IsDead;

        private void Awake()
        {
            health = GetComponent<Health>();
            baseScale = transform.localScale;

            cachedRenderer = GetComponentInChildren<Renderer>();
            if (cachedRenderer != null && cachedRenderer.sharedMaterial != null)
            {
                if (cachedRenderer.sharedMaterial.HasProperty("_BaseColor"))
                {
                    colorProperty = "_BaseColor";
                    baseColor = cachedRenderer.sharedMaterial.GetColor(colorProperty);
                }
                else if (cachedRenderer.sharedMaterial.HasProperty("_Color"))
                {
                    colorProperty = "_Color";
                    baseColor = cachedRenderer.sharedMaterial.GetColor(colorProperty);
                }

                propertyBlock = new MaterialPropertyBlock();
            }
        }

        public void ReceiveDamage(in DamagePayload payload)
        {
            if (!CanReceiveDamage) return;

            health.ApplyDamage(payload.Amount);
            FloatingDamageText.Spawn(payload.HitPoint, payload.Amount, payload.Critical);

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
            if (cachedRenderer == null || propertyBlock == null || string.IsNullOrEmpty(colorProperty))
                yield break;

            cachedRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(colorProperty, Color.white * 1.5f);
            cachedRenderer.SetPropertyBlock(propertyBlock);

            yield return new WaitForSeconds(flashDuration);

            cachedRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(colorProperty, baseColor);
            cachedRenderer.SetPropertyBlock(propertyBlock);
            flashRoutine = null;
        }
    }
}
