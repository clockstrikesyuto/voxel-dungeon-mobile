using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Combat;

namespace VoxelDungeon.UI
{
    [RequireComponent(typeof(Health))]
    public sealed class WorldHealthBar : MonoBehaviour
    {
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2.80f, 0f);
        [SerializeField] private Vector2 pixelSize = new Vector2(150f, 18f);
        [SerializeField] private float worldScale = 0.01f;

        private Health health;
        private RectTransform fill;
        private RectTransform barRoot;
        private Camera mainCamera;
        private float resolvedHeight;

        private void Awake()
        {
            health = GetComponent<Health>();
            BuildBar();
        }

        private void OnEnable()
        {
            if (health != null)
                health.Changed += OnHealthChanged;
        }

        private void Start()
        {
            mainCamera = Camera.main;
            ResolveHeight();

            if (health != null)
                OnHealthChanged(health.CurrentHealth, health.MaxHealth);
        }

        private void ResolveHeight()
        {
            float highest = transform.position.y + worldOffset.y;
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || !renderer.enabled)
                    continue;

                if (renderer.bounds.max.y > highest)
                    highest = renderer.bounds.max.y;
            }

            resolvedHeight = Mathf.Max(worldOffset.y, highest - transform.position.y + 0.38f);
        }

        private void OnDisable()
        {
            if (health != null)
                health.Changed -= OnHealthChanged;
        }

        private void LateUpdate()
        {
            if (barRoot == null)
                return;

            barRoot.position = transform.position + new Vector3(worldOffset.x, resolvedHeight, worldOffset.z);

            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera != null)
                barRoot.rotation = mainCamera.transform.rotation;
        }

        private void BuildBar()
        {
            GameObject root = new GameObject("WorldHealthBar");
            root.transform.SetParent(transform, false);

            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 20;

            barRoot = root.GetComponent<RectTransform>();
            barRoot.sizeDelta = pixelSize;
            barRoot.localScale = Vector3.one * worldScale;

            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(root.transform, false);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            Image bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.05f, 0.05f, 0.06f, 0.88f);

            GameObject fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(bg.transform, false);
            fill = fillGo.AddComponent<RectTransform>();
            fill.anchorMin = Vector2.zero;
            fill.anchorMax = Vector2.one;
            fill.offsetMin = new Vector2(3f, 3f);
            fill.offsetMax = new Vector2(-3f, -3f);
            Image fillImage = fillGo.AddComponent<Image>();
            fillImage.color = new Color(0.95f, 0.18f, 0.12f, 0.96f);
        }

        private void OnHealthChanged(int current, int max)
        {
            if (fill == null)
                return;

            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            Vector2 maxAnchor = fill.anchorMax;
            maxAnchor.x = ratio;
            fill.anchorMax = maxAnchor;
        }
    }
}
