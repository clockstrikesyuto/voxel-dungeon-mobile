using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Combat;

namespace VoxelDungeon.UI
{
    public sealed class BossHealthUI : MonoBehaviour
    {
        private Health health;
        private RectTransform fill;
        private Text hpText;
        private CanvasGroup group;

        public static BossHealthUI Show(string bossName, Health bossHealth)
        {
            GameObject old = GameObject.Find("BossHealthUI");
            if (old != null)
                Destroy(old);

            GameObject root = new GameObject("BossHealthUI");
            BossHealthUI ui = root.AddComponent<BossHealthUI>();
            ui.Build(bossName, bossHealth);
            return ui;
        }

        private void Build(string bossName, Health bossHealth)
        {
            health = bossHealth;

            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(transform, false);
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 88;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject safe = new GameObject("SafeArea");
            safe.transform.SetParent(canvasGo.transform, false);
            RectTransform safeRect = safe.AddComponent<RectTransform>();
            safeRect.anchorMin = Vector2.zero;
            safeRect.anchorMax = Vector2.one;
            safeRect.offsetMin = Vector2.zero;
            safeRect.offsetMax = Vector2.zero;
            safe.AddComponent<SafeAreaFitter>();

            GameObject panel = new GameObject("BossPanel");
            panel.transform.SetParent(safe.transform, false);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.20f, 1f);
            panelRect.anchorMax = new Vector2(0.80f, 1f);
            panelRect.pivot = new Vector2(0.5f, 1f);
            panelRect.anchoredPosition = new Vector2(0f, -118f);
            panelRect.sizeDelta = new Vector2(0f, 104f);

            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.015f, 0.025f, 0.04f, 0.92f);

            group = panel.AddComponent<CanvasGroup>();
            group.alpha = 0f;

            GameObject nameGo = new GameObject("BossName");
            nameGo.transform.SetParent(panel.transform, false);
            RectTransform nameRect = nameGo.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.04f, 0.52f);
            nameRect.anchorMax = new Vector2(0.96f, 0.95f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;

            Text nameText = nameGo.AddComponent<Text>();
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            nameText.text = bossName.Replace("Boss_", string.Empty).Replace("_", " ").ToUpperInvariant();
            nameText.fontSize = 28;
            nameText.fontStyle = FontStyle.Bold;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.color = Color.white;

            GameObject bar = new GameObject("Bar");
            bar.transform.SetParent(panel.transform, false);
            RectTransform barRect = bar.AddComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0.05f, 0.14f);
            barRect.anchorMax = new Vector2(0.95f, 0.48f);
            barRect.offsetMin = Vector2.zero;
            barRect.offsetMax = Vector2.zero;
            Image barImage = bar.AddComponent<Image>();
            barImage.color = new Color(0.06f, 0.07f, 0.09f, 1f);

            GameObject fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(bar.transform, false);
            fill = fillGo.AddComponent<RectTransform>();
            fill.anchorMin = Vector2.zero;
            fill.anchorMax = Vector2.one;
            fill.pivot = new Vector2(0f, 0.5f);
            fill.offsetMin = new Vector2(4f, 4f);
            fill.offsetMax = new Vector2(-4f, -4f);
            Image fillImage = fillGo.AddComponent<Image>();
            fillImage.color = new Color(0.90f, 0.12f, 0.16f, 1f);

            GameObject hpGo = new GameObject("HpText");
            hpGo.transform.SetParent(bar.transform, false);
            RectTransform hpRect = hpGo.AddComponent<RectTransform>();
            hpRect.anchorMin = Vector2.zero;
            hpRect.anchorMax = Vector2.one;
            hpRect.offsetMin = Vector2.zero;
            hpRect.offsetMax = Vector2.zero;
            hpText = hpGo.AddComponent<Text>();
            hpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            hpText.fontSize = 16;
            hpText.fontStyle = FontStyle.Bold;
            hpText.alignment = TextAnchor.MiddleCenter;
            hpText.color = Color.white;
            hpText.raycastTarget = false;

            if (health != null)
            {
                health.Changed += OnHealthChanged;
                health.Died += OnBossDied;
                OnHealthChanged(health.CurrentHealth, health.MaxHealth);
            }

            StartCoroutine(FadeIn());
        }

        private void OnHealthChanged(int current, int max)
        {
            if (fill == null || hpText == null)
                return;

            float ratio = max > 0 ? Mathf.Clamp01(current / (float)max) : 0f;
            fill.anchorMax = new Vector2(ratio, 1f);
            hpText.text = $"{current} / {max}";
        }

        private void OnBossDied()
        {
            StartCoroutine(FadeOutAndDestroy());
        }

        private IEnumerator FadeIn()
        {
            float age = 0f;
            while (age < 0.25f)
            {
                age += Time.deltaTime;
                if (group != null)
                    group.alpha = Mathf.Clamp01(age / 0.25f);
                yield return null;
            }
        }

        private IEnumerator FadeOutAndDestroy()
        {
            yield return new WaitForSeconds(0.45f);
            float age = 0f;
            while (age < 0.35f)
            {
                age += Time.deltaTime;
                if (group != null)
                    group.alpha = 1f - Mathf.Clamp01(age / 0.35f);
                yield return null;
            }
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.Changed -= OnHealthChanged;
                health.Died -= OnBossDied;
            }
        }
    }
}
