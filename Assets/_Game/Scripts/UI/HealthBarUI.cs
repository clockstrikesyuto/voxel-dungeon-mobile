using UnityEngine;
using VoxelDungeon.Combat;

namespace VoxelDungeon.UI
{
    public sealed class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Health source;
        [SerializeField] private RectTransform fill;

        private bool subscribed;

        public void Configure(Health health, RectTransform fillRect)
        {
            Unsubscribe();
            source = health;
            fill = fillRect;
            SubscribeAndRefresh();
        }

        private void Awake()
        {
            SubscribeAndRefresh();
        }

        private void OnEnable()
        {
            SubscribeAndRefresh();
        }

        private void Start()
        {
            RefreshFromSource();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void SubscribeAndRefresh()
        {
            if (source == null)
                return;

            if (!subscribed)
            {
                source.Changed += OnHealthChanged;
                subscribed = true;
            }

            RefreshFromSource();
        }

        private void Unsubscribe()
        {
            if (source != null && subscribed)
            {
                source.Changed -= OnHealthChanged;
                subscribed = false;
            }
        }

        private void RefreshFromSource()
        {
            if (source == null)
                return;

            int max = Mathf.Max(1, source.MaxHealth);
            int current = source.CurrentHealth > 0 ? source.CurrentHealth : max;
            OnHealthChanged(current, max);
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
