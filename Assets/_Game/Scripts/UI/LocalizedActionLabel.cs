using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Core;

namespace VoxelDungeon.UI
{
    [RequireComponent(typeof(Text))]
    public sealed class LocalizedActionLabel : MonoBehaviour
    {
        [SerializeField] private string englishLabel;

        public void Configure(string value)
        {
            englishLabel = value;
            Apply();
        }

        private void Start()
        {
            Apply();
        }

        private void Apply()
        {
            Text text = GetComponent<Text>();
            if (text == null)
                return;

            text.font = UiFontProvider.Get();

            if (!Localization.IsJapanese)
            {
                text.text = englishLabel;
                return;
            }

            text.text = englishLabel switch
            {
                "ATK" => "攻撃",
                "DODGE" => "回避",
                "RNG" => "遠距離",
                "FIRE" => "炎",
                "ICE" => "氷",
                "TONIC" => "強化",
                _ => englishLabel
            };
        }
    }
}
