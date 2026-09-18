using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Core;

namespace VoxelDungeon.UI
{
    public sealed class StageJourneyUI : MonoBehaviour
    {
        private Text zoneText;
        private CanvasGroup group;
        private int currentZone = -1;
        private Coroutine fadeRoutine;

        private static readonly string[] CryptZones =
        {
            "CRYSTAL CRYPT  •  ENTRY COURT",
            "CRYSTAL CRYPT  •  CRYSTAL HALL",
            "CRYSTAL CRYPT  •  CROSSING",
            "CRYSTAL CRYPT  •  CRYSTAL GALLERY",
            "CRYSTAL CRYPT  •  INNER SANCTUM",
            "CRYSTAL CRYPT  •  STONE WARDEN"
        };

        private static readonly string[] AshenZones =
        {
            "ASHEN FORGE  •  FORGE GATE",
            "ASHEN FORGE  •  SMELTER ROW",
            "ASHEN FORGE  •  CONVEYOR CROSS",
            "ASHEN FORGE  •  FURNACE HALL",
            "ASHEN FORGE  •  CORE WORKS",
            "ASHEN FORGE  •  FORGE COLOSSUS"
        };

        private void Start()
        {
            Build();
            UpdateZone(force: true);
        }

        private void Update()
        {
            UpdateZone(force: false);
        }

        private void UpdateZone(bool force)
        {
            int zone = ResolveZone(transform.position.z);
            if (!force && zone == currentZone)
                return;

            currentZone = zone;
            string[] zones = GameFlowState.SelectedStageId == "stage.ashen" ? AshenZones : CryptZones;
            ShowZone(zones[Mathf.Clamp(zone, 0, zones.Length - 1)]);
        }

        private int ResolveZone(float z)
        {
            if (z < -20f) return 0;
            if (z < -6f) return 1;
            if (z < 11f) return 2;
            if (z < 27f) return 3;
            if (z < 43f) return 4;
            return 5;
        }

        private void Build()
        {
            GameObject safe = GameObject.Find("SafeArea");
            if (safe == null)
                return;

            GameObject root = new GameObject("JourneyBanner");
            root.transform.SetParent(safe.transform, false);

            RectTransform rect = root.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -105f);
            rect.sizeDelta = new Vector2(900f, 64f);

            group = root.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;

            zoneText = root.AddComponent<Text>();
            zoneText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            zoneText.fontSize = 24;
            zoneText.fontStyle = FontStyle.Bold;
            zoneText.alignment = TextAnchor.MiddleCenter;
            zoneText.color = GameFlowState.SelectedStageId == "stage.ashen"
                ? new Color(1f, 0.72f, 0.34f)
                : new Color(0.86f, 0.96f, 1f);
            zoneText.raycastTarget = false;
        }

        private void ShowZone(string value)
        {
            if (zoneText == null || group == null)
                return;

            zoneText.text = value;

            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(FadeSequence());
        }

        private IEnumerator FadeSequence()
        {
            float age = 0f;
            while (age < 0.22f)
            {
                age += Time.deltaTime;
                group.alpha = Mathf.Clamp01(age / 0.22f);
                yield return null;
            }

            group.alpha = 1f;
            yield return new WaitForSeconds(1.8f);

            age = 0f;
            while (age < 0.45f)
            {
                age += Time.deltaTime;
                group.alpha = 1f - Mathf.Clamp01(age / 0.45f);
                yield return null;
            }

            group.alpha = 0f;
            fadeRoutine = null;
        }
    }
}
