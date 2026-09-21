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

        private static readonly string[] VoidZones =
        {
            "VOID GARDEN  •  GARDEN APPROACH",
            "VOID GARDEN  •  MOON TERRACE",
            "VOID GARDEN  •  MIRROR GROVE",
            "VOID GARDEN  •  STAR SHRINE",
            "VOID GARDEN  •  ASTRAL COURT",
            "VOID GARDEN  •  ASTRAL WARDEN"
        };

        private static readonly string[] CryptZonesJa =
        {
            "クリスタル洞窟  •  入口広場",
            "クリスタル洞窟  •  結晶回廊",
            "クリスタル洞窟  •  外縁の道",
            "クリスタル洞窟  •  深層洞窟",
            "クリスタル洞窟  •  地下神殿",
            "クリスタル洞窟  •  ストーンウォーデン"
        };

        private static readonly string[] AshenZonesJa =
        {
            "灼熱の鍛冶場  •  鉱山入口",
            "灼熱の鍛冶場  •  精錬通り",
            "灼熱の鍛冶場  •  鉄橋",
            "灼熱の鍛冶場  •  巨大炉",
            "灼熱の鍛冶場  •  炉心部",
            "灼熱の鍛冶場  •  フォージコロッサス"
        };

        private static readonly string[] VoidZonesJa =
        {
            "虚空の庭園  •  庭園入口",
            "虚空の庭園  •  月光テラス",
            "虚空の庭園  •  鏡の森",
            "虚空の庭園  •  星の神殿",
            "虚空の庭園  •  アストラルコート",
            "虚空の庭園  •  アストラルウォーデン"
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
            string[] zones;
            if (Localization.IsJapanese)
            {
                zones = GameFlowState.SelectedStageId switch
                {
                    "stage.ashen" => AshenZonesJa,
                    "stage.void" => VoidZonesJa,
                    _ => CryptZonesJa
                };
            }
            else
            {
                zones = GameFlowState.SelectedStageId switch
                {
                    "stage.ashen" => AshenZones,
                    "stage.void" => VoidZones,
                    _ => CryptZones
                };
            }
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
            zoneText.font = UiFontProvider.Get();
            zoneText.fontSize = 24;
            zoneText.fontStyle = FontStyle.Bold;
            zoneText.alignment = TextAnchor.MiddleCenter;
            zoneText.color = GameFlowState.SelectedStageId switch
            {
                "stage.ashen" => new Color(1f, 0.72f, 0.34f),
                "stage.void" => new Color(0.72f, 0.54f, 1f),
                _ => new Color(0.86f, 0.96f, 1f)
            };
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
