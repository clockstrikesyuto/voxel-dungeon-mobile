using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Player;

namespace VoxelDungeon.UI
{
    [RequireComponent(typeof(PlayerProgress))]
    public sealed class ProgressHudUI : MonoBehaviour
    {
        private PlayerProgress progress;
        private Text label;

        private void Start()
        {
            progress = GetComponent<PlayerProgress>();
            Build();
            progress.Changed += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (progress != null)
                progress.Changed -= Refresh;
        }

        private void Build()
        {
            GameObject safe = GameObject.Find("SafeArea");
            if (safe == null)
                return;

            GameObject go = new GameObject("ProgressText");
            go.transform.SetParent(safe.transform, false);

            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-55f, -45f);
            rect.sizeDelta = new Vector2(420f, 80f);

            label = go.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 30;
            label.alignment = TextAnchor.UpperRight;
            label.color = Color.white;
        }

        private void Refresh()
        {
            if (label == null || progress == null)
                return;

            label.text = $"GOLD {progress.Gold}   MELEE +{progress.MeleePowerBonus}   RANGE +{progress.RangedPowerBonus}";
        }
    }
}
