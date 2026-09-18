using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Core;

namespace VoxelDungeon.UI
{
    public sealed class MissionHeaderUI : MonoBehaviour
    {
        private void Start()
        {
            GameObject safe = GameObject.Find("SafeArea");
            if (safe == null)
                return;

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            GameObject root = new GameObject("MissionHeader");
            root.transform.SetParent(safe.transform, false);

            RectTransform rect = root.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -28f);
            rect.sizeDelta = new Vector2(760f, 80f);

            Text title = root.AddComponent<Text>();
            title.font = font;
            title.text = GameFlowState.SelectedStageName;
            title.fontSize = 26;
            title.fontStyle = FontStyle.Bold;
            title.alignment = TextAnchor.UpperCenter;
            title.color = Color.white;
            title.raycastTarget = false;

            GameObject subGo = new GameObject("Mode");
            subGo.transform.SetParent(root.transform, false);

            RectTransform subRect = subGo.AddComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0f, 0f);
            subRect.anchorMax = new Vector2(1f, 0f);
            subRect.pivot = new Vector2(0.5f, 0f);
            subRect.anchoredPosition = new Vector2(0f, 4f);
            subRect.sizeDelta = new Vector2(0f, 28f);

            Text sub = subGo.AddComponent<Text>();
            sub.font = font;
            sub.text = GameFlowState.Mode == PlayModeKind.Solo ? "SOLO MISSION" : "MULTIPLAYER MISSION";
            sub.fontSize = 16;
            sub.fontStyle = FontStyle.Bold;
            sub.alignment = TextAnchor.LowerCenter;
            sub.color = new Color(0.12f, 0.82f, 0.95f);
            sub.raycastTarget = false;
        }
    }
}
