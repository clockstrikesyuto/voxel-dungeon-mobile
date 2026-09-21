using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VoxelDungeon.Network;

namespace VoxelDungeon.Core
{
    public sealed class MainMenuController : MonoBehaviour
    {
        private enum ScreenKind
        {
            Title,
            StageSelect,
            Multiplayer,
            Lobby
        }

        private Canvas canvas;
        private RectTransform safeRoot;
        private RectTransform contentRoot;
        private Font font;
        private ScreenKind currentScreen;

        private readonly Color background = new Color(0.018f, 0.028f, 0.045f, 1f);
        private readonly Color panel = new Color(0.035f, 0.055f, 0.078f, 0.94f);
        private readonly Color cyan = new Color(0.10f, 0.82f, 0.95f, 1f);
        private readonly Color violet = new Color(0.55f, 0.28f, 1f, 1f);
        private readonly Color warm = new Color(1f, 0.48f, 0.16f, 1f);

        private void Start()
        {
            font = UiFontProvider.Get();
            BuildCanvas();
            ShowTitle();
        }

        private void BuildCanvas()
        {
            GameObject canvasGo = new GameObject("MainMenuCanvas");
            canvasGo.transform.SetParent(transform, false);

            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject bg = CreateRect(
                "Background",
                canvasGo.transform,
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                background);
            bg.transform.SetAsFirstSibling();

            AddDecor(bg.transform);

            GameObject safe = new GameObject("SafeArea");
            safe.transform.SetParent(canvasGo.transform, false);
            safeRoot = safe.AddComponent<RectTransform>();
            safeRoot.anchorMin = Vector2.zero;
            safeRoot.anchorMax = Vector2.one;
            safeRoot.offsetMin = Vector2.zero;
            safeRoot.offsetMax = Vector2.zero;
            safe.AddComponent<VoxelDungeon.UI.SafeAreaFitter>();

            GameObject content = new GameObject("Content");
            content.transform.SetParent(safeRoot, false);
            contentRoot = content.AddComponent<RectTransform>();
            contentRoot.anchorMin = Vector2.zero;
            contentRoot.anchorMax = Vector2.one;
            contentRoot.offsetMin = Vector2.zero;
            contentRoot.offsetMax = Vector2.zero;

            if (UnityEngine.Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
        }

        private void AddDecor(Transform parent)
        {
            CreateDecorSquare(parent, new Vector2(0.08f, 0.74f), new Vector2(280f, 280f), new Color(0.08f, 0.36f, 0.42f, 0.18f), 18f);
            CreateDecorSquare(parent, new Vector2(0.87f, 0.20f), new Vector2(360f, 360f), new Color(0.34f, 0.12f, 0.55f, 0.17f), -22f);
            CreateDecorSquare(parent, new Vector2(0.75f, 0.78f), new Vector2(170f, 170f), new Color(0.78f, 0.28f, 0.08f, 0.11f), 34f);
        }

        private void ShowTitle()
        {
            currentScreen = ScreenKind.Title;
            ClearContent();

            AddText("VOXEL DUNGEON", new Vector2(0.5f, 0.72f), new Vector2(980f, 150f), 82, FontStyle.Bold, Color.white);
            AddText("CRYSTAL FRONTIER", new Vector2(0.5f, 0.625f), new Vector2(760f, 70f), 30, FontStyle.Bold, cyan);
            AddText(
                Localization.IsJapanese ? "探索して、拾って、強くなる。" : "DESCEND. LOOT. GROW STRONGER.",
                new Vector2(0.5f, 0.55f),
                new Vector2(900f, 55f),
                22,
                FontStyle.Normal,
                new Color(0.72f, 0.78f, 0.86f));

            CreateButton(Localization.T("solo"), new Vector2(0.5f, 0.40f), new Vector2(520f, 92f), cyan, () =>
            {
                GameFlowState.Mode = PlayModeKind.Solo;
                SceneManager.LoadScene("Hub");
            });

            CreateButton(Localization.T("multi"), new Vector2(0.5f, 0.285f), new Vector2(520f, 92f), violet, () =>
            {
                GameFlowState.Mode = PlayModeKind.Multiplayer;
                ShowMultiplayer();
            });

            AddText(
                Localization.IsJapanese ? "1〜4人  •  モバイル対応アクションRPG" : "1-4 PLAYERS  •  MOBILE-FIRST ACTION RPG",
                new Vector2(0.5f, 0.11f),
                new Vector2(900f, 50f),
                20,
                FontStyle.Normal,
                new Color(0.52f, 0.6f, 0.7f));

            CreateButton(
                Localization.IsJapanese ? "LANGUAGE: 日本語" : "LANGUAGE: English",
                new Vector2(0.86f, 0.08f),
                new Vector2(300f, 64f),
                new Color(0.36f, 0.44f, 0.52f),
                () =>
                {
                    Localization.Current = Localization.IsJapanese
                        ? GameLanguage.English
                        : GameLanguage.Japanese;
                    ShowTitle();
                });
        }

        private void ShowStageSelect()
        {
            currentScreen = ScreenKind.StageSelect;
            ClearContent();

            AddTopHeader(
                Localization.IsJapanese ? "ステージ選択" : "SELECT STAGE",
                GameFlowState.Mode == PlayModeKind.Solo
                    ? (Localization.IsJapanese ? "ソロ" : "SOLO")
                    : Localization.T("multi"));

            CreateStageCard(
                "01",
                Localization.StageName("stage.crypt"),
                Localization.StageDescription("stage.crypt"),
                "NORMAL",
                new Vector2(0.5f, 0.59f),
                cyan,
                true,
                () =>
                {
                    GameFlowState.SelectedStageId = "stage.crypt";
                    GameFlowState.SelectedStageName = Localization.StageName("stage.crypt");

                    if (GameFlowState.Mode == PlayModeKind.Solo)
                        SceneManager.LoadScene("Mission_Test");
                    else
                        ShowLobby();
                });

            CreateStageCard(
                "02",
                Localization.StageName("stage.ashen"),
                Localization.StageDescription("stage.ashen"),
                "COMING SOON",
                new Vector2(0.5f, 0.37f),
                warm,
                false,
                null);

            CreateStageCard(
                "03",
                Localization.StageName("stage.void"),
                Localization.StageDescription("stage.void"),
                "COMING SOON",
                new Vector2(0.5f, 0.15f),
                violet,
                false,
                null);

            CreateBackButton(GameFlowState.Mode == PlayModeKind.Solo ? ShowTitle : ShowMultiplayer);
        }

        private void ShowMultiplayer()
        {
            currentScreen = ScreenKind.Multiplayer;
            ClearContent();

            AddTopHeader(
                Localization.T("multi"),
                Localization.IsJapanese
                    ? $"最大 {NetworkDesignContract.MaxPlayers} 人"
                    : $"UP TO {NetworkDesignContract.MaxPlayers} PLAYERS");
            AddText(Localization.IsJapanese ? "ルームコードでオンライン参加。" : "Play online with a room code.", new Vector2(0.5f, 0.66f), new Vector2(750f, 60f), 26, FontStyle.Normal, new Color(0.72f, 0.78f, 0.86f));

            CreateButton("CREATE ROOM", new Vector2(0.5f, 0.51f), new Vector2(520f, 92f), cyan, () =>
            {
                GameFlowState.RoomCode = GenerateRoomCode();
                ShowStageSelect();
            });

            CreateButton("JOIN ROOM", new Vector2(0.5f, 0.39f), new Vector2(520f, 92f), violet, ShowJoinRoom);

            AddText("Relay/online synchronization is the next networking pass.", new Vector2(0.5f, 0.23f), new Vector2(920f, 55f), 19, FontStyle.Normal, new Color(0.5f, 0.58f, 0.68f));
            CreateBackButton(ShowTitle);
        }

        private void ShowJoinRoom()
        {
            ClearContent();
            AddTopHeader("JOIN ROOM", "ENTER 6-CHARACTER CODE");

            InputField input = CreateInputField(new Vector2(0.5f, 0.52f), new Vector2(500f, 86f));
            AddText("Current build validates the lobby flow; Relay connection comes next.", new Vector2(0.5f, 0.38f), new Vector2(900f, 55f), 19, FontStyle.Normal, new Color(0.55f, 0.62f, 0.72f));

            Button join = CreateButton("CONTINUE", new Vector2(0.5f, 0.26f), new Vector2(500f, 86f), violet, () =>
            {
                string code = (input.text ?? string.Empty).Trim().ToUpperInvariant();
                if (code.Length != 6)
                {
                    input.text = string.Empty;
                    input.placeholder.GetComponent<Text>().text = "ENTER 6 CHARACTERS";
                    return;
                }

                GameFlowState.RoomCode = code;
                ShowLobby();
            });

            CreateBackButton(ShowMultiplayer);
        }

        private void ShowLobby()
        {
            currentScreen = ScreenKind.Lobby;
            ClearContent();

            if (string.IsNullOrEmpty(GameFlowState.RoomCode))
                GameFlowState.RoomCode = GenerateRoomCode();

            AddTopHeader("ROOM LOBBY", GameFlowState.SelectedStageName);
            AddText("ROOM CODE", new Vector2(0.5f, 0.62f), new Vector2(520f, 50f), 22, FontStyle.Bold, new Color(0.62f, 0.68f, 0.76f));
            AddText(GameFlowState.RoomCode, new Vector2(0.5f, 0.53f), new Vector2(700f, 100f), 62, FontStyle.Bold, cyan);

            CreatePlayerSlot("PLAYER 1", "HOST / READY", new Vector2(0.5f, 0.39f), true);
            CreatePlayerSlot("PLAYER 2", "WAITING...", new Vector2(0.5f, 0.31f), false);
            CreatePlayerSlot("PLAYER 3", "WAITING...", new Vector2(0.5f, 0.23f), false);
            CreatePlayerSlot("PLAYER 4", "WAITING...", new Vector2(0.5f, 0.15f), false);

            AddText("ONLINE CONNECT / READY SYNC NEXT", new Vector2(0.5f, 0.065f), new Vector2(780f, 42f), 18, FontStyle.Bold, violet);
            CreateBackButton(ShowMultiplayer);
        }

        private void AddTopHeader(string title, string subtitle)
        {
            AddText(title, new Vector2(0.5f, 0.865f), new Vector2(950f, 100f), 52, FontStyle.Bold, Color.white);
            AddText(subtitle, new Vector2(0.5f, 0.795f), new Vector2(700f, 50f), 21, FontStyle.Bold, cyan);
        }

        private void CreateStageCard(
            string number,
            string title,
            string description,
            string status,
            Vector2 anchor,
            Color accent,
            bool interactable,
            Action onClick)
        {
            GameObject card = CreateRect("StageCard", contentRoot, anchor, anchor, Vector2.zero, new Vector2(900f, 180f), panel);
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;

            CreateRect("Accent", card.transform, new Vector2(0f, 0f), new Vector2(0f, 1f), Vector2.zero, new Vector2(9f, 0f), accent);

            Text num = CreateChildText(card.transform, number, new Vector2(35f, -28f), new Vector2(100f, 80f), 40, FontStyle.Bold, accent, TextAnchor.UpperLeft);
            Text name = CreateChildText(card.transform, title, new Vector2(135f, -28f), new Vector2(520f, 65f), 32, FontStyle.Bold, Color.white, TextAnchor.UpperLeft);
            Text desc = CreateChildText(card.transform, description, new Vector2(135f, -86f), new Vector2(560f, 55f), 19, FontStyle.Normal, new Color(0.65f, 0.72f, 0.8f), TextAnchor.UpperLeft);
            Text badge = CreateChildText(card.transform, status, new Vector2(690f, -57f), new Vector2(170f, 55f), 18, FontStyle.Bold, interactable ? accent : new Color(0.48f, 0.5f, 0.55f), TextAnchor.MiddleCenter);

            if (interactable)
            {
                Button button = card.AddComponent<Button>();
                button.targetGraphic = card.GetComponent<Image>();
                if (onClick != null)
                    button.onClick.AddListener(() => onClick());
            }
            else
            {
                card.GetComponent<Image>().color = new Color(panel.r, panel.g, panel.b, 0.55f);
            }
        }

        private void CreatePlayerSlot(string playerName, string state, Vector2 anchor, bool ready)
        {
            GameObject row = CreateRect("PlayerSlot", contentRoot, anchor, anchor, Vector2.zero, new Vector2(650f, 66f), new Color(0.04f, 0.065f, 0.09f, 0.92f));
            CreateChildText(row.transform, playerName, new Vector2(24f, -5f), new Vector2(300f, 55f), 22, FontStyle.Bold, Color.white, TextAnchor.MiddleLeft);
            CreateChildText(row.transform, state, new Vector2(350f, -5f), new Vector2(270f, 55f), 18, FontStyle.Bold, ready ? cyan : new Color(0.45f, 0.52f, 0.62f), TextAnchor.MiddleRight);
        }

        private Button CreateButton(string label, Vector2 anchor, Vector2 size, Color accent, Action action)
        {
            GameObject go = CreateRect(label.Replace(" ", "_"), contentRoot, anchor, anchor, Vector2.zero, size, new Color(accent.r * 0.26f, accent.g * 0.26f, accent.b * 0.26f, 0.96f));
            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(accent.r, accent.g, accent.b, 0.75f);
            outline.effectDistance = new Vector2(2f, -2f);

            Button button = go.AddComponent<Button>();
            button.targetGraphic = go.GetComponent<Image>();
            if (action != null)
                button.onClick.AddListener(() => action());

            CreateChildText(go.transform, label, Vector2.zero, size, 27, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            return button;
        }

        private InputField CreateInputField(Vector2 anchor, Vector2 size)
        {
            GameObject go = CreateRect("RoomCodeInput", contentRoot, anchor, anchor, Vector2.zero, size, new Color(0.03f, 0.055f, 0.08f, 1f));
            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(violet.r, violet.g, violet.b, 0.8f);
            outline.effectDistance = new Vector2(2f, -2f);

            InputField input = go.AddComponent<InputField>();

            Text text = CreateChildText(go.transform, string.Empty, new Vector2(20f, 0f), new Vector2(size.x - 40f, size.y), 34, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            Text placeholder = CreateChildText(go.transform, "ABC123", new Vector2(20f, 0f), new Vector2(size.x - 40f, size.y), 30, FontStyle.Normal, new Color(0.35f, 0.42f, 0.5f), TextAnchor.MiddleCenter);

            input.textComponent = text;
            input.placeholder = placeholder;
            input.characterLimit = 6;
            input.contentType = InputField.ContentType.Alphanumeric;
            input.lineType = InputField.LineType.SingleLine;
            return input;
        }

        private void CreateBackButton(Action action)
        {
            Button b = CreateButton(Localization.T("back"), new Vector2(0.09f, 0.08f), new Vector2(190f, 64f), new Color(0.38f, 0.44f, 0.52f), action);
            b.GetComponentInChildren<Text>().fontSize = 20;
        }

        private Text AddText(string value, Vector2 anchor, Vector2 size, int fontSize, FontStyle style, Color color)
        {
            GameObject go = new GameObject("Text");
            go.transform.SetParent(contentRoot, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;

            Text t = go.AddComponent<Text>();
            t.font = font;
            t.text = value;
            t.fontSize = fontSize;
            t.fontStyle = style;
            t.color = color;
            t.alignment = TextAnchor.MiddleCenter;
            t.raycastTarget = false;
            return t;
        }

        private Text CreateChildText(
            Transform parent,
            string value,
            Vector2 anchoredPosition,
            Vector2 size,
            int fontSize,
            FontStyle style,
            Color color,
            TextAnchor alignment)
        {
            GameObject go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            if (alignment == TextAnchor.MiddleCenter || alignment == TextAnchor.MiddleRight)
            {
                rect.anchorMin = new Vector2(0f, 0.5f);
                rect.anchorMax = new Vector2(0f, 0.5f);
                rect.pivot = new Vector2(0f, 0.5f);
            }

            Text t = go.AddComponent<Text>();
            t.font = font;
            t.text = value;
            t.fontSize = fontSize;
            t.fontStyle = style;
            t.color = color;
            t.alignment = alignment;
            t.raycastTarget = false;
            return t;
        }

        private GameObject CreateRect(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 size,
            Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;

            if (anchorMin == anchorMax)
                rect.sizeDelta = size;
            else
            {
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }

            Image image = go.AddComponent<Image>();
            image.color = color;
            return go;
        }

        private void CreateDecorSquare(Transform parent, Vector2 anchor, Vector2 size, Color color, float rotation)
        {
            GameObject go = CreateRect("Decor", parent, anchor, anchor, Vector2.zero, size, color);
            go.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
            go.GetComponent<Image>().raycastTarget = false;
        }

        private string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            System.Random random = new System.Random();
            char[] code = new char[6];
            for (int i = 0; i < code.Length; i++)
                code[i] = chars[random.Next(chars.Length)];
            return new string(code);
        }

        private void ClearContent()
        {
            for (int i = contentRoot.childCount - 1; i >= 0; i--)
                Destroy(contentRoot.GetChild(i).gameObject);
        }
    }
}
