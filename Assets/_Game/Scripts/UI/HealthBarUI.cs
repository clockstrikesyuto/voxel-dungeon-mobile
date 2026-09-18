using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.UI
{
    public sealed class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Health source;
        [SerializeField] private RectTransform fill;

        public void Configure(Health health, RectTransform fillRect)
        {
            if (source != null)
                source.Changed -= OnHealthChanged;

            source = health;
            fill = fillRect;

            if (source != null)
            {
                source.Changed += OnHealthChanged;
                OnHealthChanged(source.CurrentHealth, source.MaxHealth);
            }
        }

        private void OnDestroy()
        {
            if (source != null)
                source.Changed -= OnHealthChanged;
        }

        private void OnHealthChanged(int current, int max)
        {
            if (fill == null)
                return;

            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            Vector2 anchorMax = fill.anchorMax;
            anchorMax.x = ratio;
            fill.anchorMax = anchorMax;
        }
    }
}
