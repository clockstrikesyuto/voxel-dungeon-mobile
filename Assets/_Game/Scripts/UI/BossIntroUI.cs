using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VoxelDungeon.UI
{
    public sealed class BossIntroUI : MonoBehaviour
    {
        public static void Show(string bossName)
        {
            GameObject existing = GameObject.Find("BossIntroUI");
            if (existing != null)
                Destroy(existing);

            GameObject go = new GameObject("BossIntroUI");
            BossIntroUI intro = go.AddComponent<BossIntroUI>();
            intro.StartCoroutine(intro.Run(bossName));
        }

        private IEnumerator Run(string bossName)
        {
            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(transform, false);

            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 85;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject textGo = new GameObject("BossName");
            textGo.transform.SetParent(canvasGo.transform, false);

            RectTransform rect = textGo.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.18f, 0.62f);
            rect.anchorMax = new Vector2(0.82f, 0.78f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            CanvasGroup group = textGo.AddComponent<CanvasGroup>();
            group.alpha = 0f;

            Text text = textGo.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = bossName.Replace("Boss_", string.Empty).Replace("_", " ").ToUpperInvariant();
            text.fontSize = 58;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.raycastTarget = false;

            Shadow shadow = textGo.AddComponent<Shadow>();
            shadow.effectDistance = new Vector2(4f, -4f);
            shadow.effectColor = new Color(0f, 0f, 0f, 0.72f);

            float age = 0f;
            while (age < 0.28f)
            {
                age += Time.deltaTime;
                group.alpha = Mathf.Clamp01(age / 0.28f);
                yield return null;
            }

            group.alpha = 1f;
            yield return new WaitForSeconds(1.1f);

            age = 0f;
            while (age < 0.55f)
            {
                age += Time.deltaTime;
                group.alpha = 1f - Mathf.Clamp01(age / 0.55f);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
