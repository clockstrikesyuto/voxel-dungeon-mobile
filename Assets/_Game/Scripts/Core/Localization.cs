using System.Collections.Generic;
using UnityEngine;

namespace VoxelDungeon.Core
{
    public enum GameLanguage
    {
        English = 0,
        Japanese = 1
    }

    public static class Localization
    {
        private const string LanguageKey = "settings.language";

        private static readonly Dictionary<string, string> English = new()
        {
            ["solo"] = "SOLO PLAY",
            ["multi"] = "MULTIPLAYER",
            ["language"] = "LANGUAGE",
            ["settings"] = "SETTINGS",
            ["back"] = "BACK",
            ["enter"] = "ENTER",
            ["open_map"] = "OPEN MAP",
            ["use_forge"] = "USE FORGE",
            ["talk"] = "TALK",
            ["train"] = "TRAIN",
            ["open_arsenal"] = "OPEN ARSENAL",
            ["locked"] = "LOCKED",
            ["mission_clear"] = "MISSION CLEAR",
            ["return_hub"] = "RETURN HUB",
            ["retry"] = "RETRY",
            ["title"] = "TITLE",
            ["new_route"] = "NEW ROUTE UNLOCKED",
            ["world_map"] = "FRONTIER MAP",
            ["choose_expedition"] = "Choose your next expedition.",
            ["recommended"] = "RECOMMENDED",
            ["materials"] = "MATERIALS",
            ["equipment"] = "EQUIPMENT",
            ["boss"] = "BOSS",
            ["cleared"] = "CLEARED",
            ["unlocked"] = "UNLOCKED",
            ["sealed"] = "SEALED",
            ["solo_mission"] = "SOLO MISSION",
            ["multiplayer_mission"] = "MULTIPLAYER MISSION",
            ["gold"] = "GOLD",
            ["exp"] = "EXP",
            ["loot"] = "LOOT"
        };

        private static readonly Dictionary<string, string> Japanese = new()
        {
            ["solo"] = "ソロプレイ",
            ["multi"] = "マルチプレイ",
            ["language"] = "言語",
            ["settings"] = "設定",
            ["back"] = "戻る",
            ["enter"] = "入る",
            ["open_map"] = "マップを開く",
            ["use_forge"] = "鍛冶を使う",
            ["talk"] = "話す",
            ["train"] = "訓練する",
            ["open_arsenal"] = "装備を開く",
            ["locked"] = "未解放",
            ["mission_clear"] = "ミッションクリア",
            ["return_hub"] = "拠点へ戻る",
            ["retry"] = "もう一度",
            ["title"] = "タイトル",
            ["new_route"] = "新ルート解放",
            ["world_map"] = "フロンティアマップ",
            ["choose_expedition"] = "次の冒険先を選択",
            ["recommended"] = "推奨",
            ["materials"] = "素材",
            ["equipment"] = "装備",
            ["boss"] = "ボス",
            ["cleared"] = "クリア済み",
            ["unlocked"] = "解放済み",
            ["sealed"] = "封印中",
            ["solo_mission"] = "ソロミッション",
            ["multiplayer_mission"] = "マルチミッション",
            ["gold"] = "ゴールド",
            ["exp"] = "経験値",
            ["loot"] = "戦利品"
        };

        public static GameLanguage Current
        {
            get => (GameLanguage)PlayerPrefs.GetInt(LanguageKey, (int)GameLanguage.Japanese);
            set
            {
                PlayerPrefs.SetInt(LanguageKey, (int)value);
                PlayerPrefs.Save();
            }
        }

        public static bool IsJapanese => Current == GameLanguage.Japanese;

        public static string T(string key)
        {
            Dictionary<string, string> table = IsJapanese ? Japanese : English;
            return table.TryGetValue(key, out string value) ? value : key;
        }

        public static string StageName(string stageId)
        {
            return stageId switch
            {
                "stage.crypt" => IsJapanese ? "クリスタル洞窟" : "CRYSTAL CRYPT",
                "stage.ashen" => IsJapanese ? "灼熱の鍛冶場" : "ASHEN FORGE",
                "stage.void" => IsJapanese ? "虚空の庭園" : "VOID GARDEN",
                _ => IsJapanese ? "未知の地" : "UNKNOWN"
            };
        }

        public static string StageDescription(string stageId)
        {
            return stageId switch
            {
                "stage.crypt" => IsJapanese
                    ? "結晶が輝く洞窟と古代遺跡。"
                    : "Luminous caverns and ancient crystal ruins.",
                "stage.ashen" => IsJapanese
                    ? "溶岩と巨大炉が広がる鉱山・鍛冶エリア。"
                    : "Molten mines, iron bridges and colossal furnaces.",
                "stage.void" => IsJapanese
                    ? "水辺と白い遺跡が広がる幻想的な庭園。"
                    : "Moonlit gardens, white ruins and strange waters.",
                _ => string.Empty
            };
        }

        public static string EquipmentName(string id, string fallback = null)
        {
            if (!IsJapanese)
                return string.IsNullOrEmpty(fallback) ? id.Replace("_", " ").ToUpperInvariant() : fallback;

            return id switch
            {
                "rustblade" => "フロンティアブレード",
                "crystal_saber" => "クリスタルセイバー",
                "crystal_daggers" => "クリスタルダガー",
                "warden_cleaver" => "ウォーデンクリーバー",
                "ember_axe" => "エンバーアックス",
                "forge_spear" => "フォージスピア",
                "colossus_maul" => "コロッサスモール",
                "moonblade" => "ムーンブレード",
                "void_edge" => "ヴォイドエッジ",
                "field_bow" => "フィールドボウ",
                "crystal_bow" => "クリスタルボウ",
                "ember_repeater" => "エンバーリピーター",
                "void_staff" => "ヴォイドスタッフ",
                "starbow" => "スターボウ",
                "astral_crossbow" => "アストラルクロスボウ",
                "frontier_cap" => "フロンティアキャップ",
                "crystal_hood" => "クリスタルフード",
                "forge_helm" => "フォージヘルム",
                "astral_crown" => "アストラルクラウン",
                "frontier_vest" => "フロンティアベスト",
                "crystal_mail" => "クリスタルメイル",
                "ember_plate" => "エンバープレート",
                "void_mantle" => "ヴォイドマント",
                "trail_boots" => "トレイルブーツ",
                "crystal_steps" => "クリスタルステップ",
                "ember_greaves" => "エンバーグリーヴ",
                "moonstep_boots" => "ムーンステップ",
                "scout_charm" => "スカウトチャーム",
                "crystal_charm" => "クリスタルチャーム",
                "forge_emblem" => "フォージエンブレム",
                "void_talisman" => "ヴォイドタリスマン",
                _ => string.IsNullOrEmpty(fallback) ? id : fallback
            };
        }

        public static string ConsumableName(string id)
        {
            if (!IsJapanese) return id.Replace("_", " ").ToUpperInvariant();

            return id switch
            {
                "healing_potion" => "回復ポーション",
                "power_tonic" => "攻撃トニック",
                "guard_tonic" => "防御トニック",
                "haste_tonic" => "速度トニック",
                "fire_bomb" => "火炎爆弾",
                "frost_flask" => "氷結瓶",
                _ => id
            };
        }

        public static string MaterialName(string id)
        {
            if (!IsJapanese) return id.Replace("_", " ").ToUpperInvariant();

            return id switch
            {
                "crystal_shard" => "クリスタルの欠片",
                "iron_ore" => "鉄鉱石",
                "ember_core" => "炎の核",
                "moon_bloom" => "月光花",
                "void_fragment" => "虚空の欠片",
                "ancient_relic" => "古代遺物",
                _ => id
            };
        }
    }
}
