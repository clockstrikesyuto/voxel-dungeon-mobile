using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Combat;

namespace VoxelDungeon.UI
{
    [RequireComponent(typeof(Health))]
    public sealed class WorldHealthBar : MonoBehaviour
    {
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2.35f, 0f);
        [SerializeField] private Vector2 size = new Vector2(1.4f, 0.16f);

        private Health health;
        private RectTransform fill;
        private Transform barRoot;
        private Camera mainCamera;

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
            if (health != null)
                OnHealthChanged(health.CurrentHealth, health.MaxHealth);
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

            barRoot.position = transform.position + worldOffset;

            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera != null)
                barRoot.rotation = Quaternion.LookRotation(barRoot.position - mainCamera.transform.position);
        }

        private void BuildBar()
        {
            GameObject root = new GameObject("WorldHealthBar");
            barRoot = root.transform;
            barRoot.SetParent(transform, false);

            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 20;

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.sizeDelta = size;
            rootRect.localScale = Vector3.one * 0.01f;

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
