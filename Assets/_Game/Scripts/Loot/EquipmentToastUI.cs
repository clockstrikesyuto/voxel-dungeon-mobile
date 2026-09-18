using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Items;

namespace VoxelDungeon.Loot
{
    public sealed class EquipmentToastUI : MonoBehaviour
    {
        public static void Show(EquipmentRecord item)
        {
            GameObject go = new GameObject("EquipmentToast");
            EquipmentToastUI ui = go.AddComponent<EquipmentToastUI>();
            ui.StartCoroutine(ui.Run(item));
        }

        private IEnumerator Run(EquipmentRecord item)
        {
            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(transform, false);
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 90;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            GameObject panel = new GameObject("Panel");
            panel.transform.SetParent(canvasGo.transform, false);
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.30f, 0.72f);
            rect.anchorMax = new Vector2(0.70f, 0.86f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image bg = panel.AddComponent<Image>();
            Color rarity = EquipmentCatalog.GetRarityColor(item.Rarity);
            bg.color = new Color(0.02f, 0.035f, 0.055f, 0.94f);

            Outline outline = panel.AddComponent<Outline>();
            outline.effectColor = rarity;
            outline.effectDistance = new Vector2(2f, -2f);

            CanvasGroup group = panel.AddComponent<CanvasGroup>();
            group.alpha = 0f;

            Text text = panel.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 25;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.text = $"NEW EQUIPMENT\n{item.Name}   POWER +{item.Power}";
            text.raycastTarget = false;

            float age = 0f;
            while (age < 0.20f)
            {
                age += Time.deltaTime;
                group.alpha = Mathf.Clamp01(age / 0.20f);
                yield return null;
            }

            yield return new WaitForSeconds(1.8f);

            age = 0f;
            while (age < 0.45f)
            {
                age += Time.deltaTime;
                group.alpha = 1f - Mathf.Clamp01(age / 0.45f);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
