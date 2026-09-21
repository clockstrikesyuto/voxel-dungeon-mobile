using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Core;
using VoxelDungeon.Player;

namespace VoxelDungeon.UI
{
    [RequireComponent(typeof(PlayerProgress))]
    public sealed class ProgressHudUI : MonoBehaviour
    {
        private PlayerProgress progress;
        private Text label;
        private float nextRefreshTime;

        private void Start()
        {
            progress = GetComponent<PlayerProgress>();
            Build();
            progress.Changed += Refresh;
            Refresh();
        }

        private void Update()
        {
            if (Time.time < nextRefreshTime)
                return;

            nextRefreshTime = Time.time + 0.25f;
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
            rect.sizeDelta = new Vector2(720f, 155f);

            label = go.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 23;
            label.alignment = TextAnchor.UpperRight;
            label.color = Color.white;
        }

        private void Refresh()
        {
            if (label == null || progress == null)
                return;

            var melee = ProfileProgress.EquippedMelee;
            var ranged = ProfileProgress.EquippedRanged;

            label.text =
                $"LV {ProfileProgress.Level}  EXP {ProfileProgress.Experience}/{ProfileProgress.ExperienceToNextLevel}  POT {ProfileProgress.GetConsumable("healing_potion")}  FIRE {ProfileProgress.GetConsumable("fire_bomb")}  ICE {ProfileProgress.GetConsumable("frost_flask")}\n" +
                $"GOLD {progress.Gold}   MELEE +{progress.MeleePowerBonus}   RANGE +{progress.RangedPowerBonus}\n" +
                $"{melee.Name}  •  {ranged.Name}";
        }
    }
}
