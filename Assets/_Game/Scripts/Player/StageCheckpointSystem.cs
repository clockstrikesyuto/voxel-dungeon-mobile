using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VoxelDungeon.Combat;
using VoxelDungeon.Core;

namespace VoxelDungeon.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class StageCheckpointSystem : MonoBehaviour
    {
        private Vector3 respawnPosition;
        private string checkpointName = "START";
        private bool recovering;
        private float recoveryLockUntil;

        public string CheckpointName => checkpointName;
        public bool IsRecovering => recovering;

        private void Awake()
        {
            respawnPosition = transform.position + Vector3.up * 0.18f;
        }

        public void ActivateCheckpoint(Vector3 position, string label)
        {
            if (recovering)
                return;

            respawnPosition = ResolveSafeGround(position);
            checkpointName = Localization.CheckpointName(
                string.IsNullOrEmpty(label) ? "CHECKPOINT" : label);
        }

        private Vector3 ResolveSafeGround(Vector3 requested)
        {
            Vector3[] offsets =
            {
                Vector3.zero,
                new Vector3(1.15f, 0f, 0f),
                new Vector3(-1.15f, 0f, 0f),
                new Vector3(0f, 0f, 1.15f),
                new Vector3(0f, 0f, -1.15f),
                new Vector3(0.85f, 0f, 0.85f),
                new Vector3(-0.85f, 0f, 0.85f),
                new Vector3(0.85f, 0f, -0.85f),
                new Vector3(-0.85f, 0f, -0.85f)
            };

            Vector3 best = requested + Vector3.up * 0.18f;
            float bestDistance = float.MaxValue;

            foreach (Vector3 offset in offsets)
            {
                Vector3 sample = requested + offset;
                Vector3 origin = sample + Vector3.up * 4f;

                if (!Physics.Raycast(
                        origin,
                        Vector3.down,
                        out RaycastHit hit,
                        10f,
                        Physics.AllLayers,
                        QueryTriggerInteraction.Ignore))
                    continue;

                if (hit.normal.y < 0.68f)
                    continue;

                float distance = offset.sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = hit.point + Vector3.up * 0.12f;
                }
            }

            return best;
        }

        public bool BeginRecovery()
        {
            if (recovering || Time.time < recoveryLockUntil)
                return false;

            StartCoroutine(RecoverRoutine());
            return true;
        }

        private IEnumerator RecoverRoutine()
        {
            recovering = true;

            RecoveryFadeUI fade = RecoveryFadeUI.Show(
                Localization.IsJapanese
                    ? $"{checkpointName}へ復帰中"
                    : $"RETURNING TO {checkpointName}");

            if (fade != null)
                yield return fade.FadeIn();

            yield return new WaitForSecondsRealtime(0.18f);

            CharacterController controller = GetComponent<CharacterController>();
            TopDownPlayerMotor motor = GetComponent<TopDownPlayerMotor>();
            KnockbackMotor knockback = GetComponent<KnockbackMotor>();

            if (motor != null)
                motor.enabled = false;

            bool controllerWasEnabled = controller != null && controller.enabled;
            if (controllerWasEnabled)
                controller.enabled = false;

            transform.position = respawnPosition;

            if (controllerWasEnabled)
                controller.enabled = true;

            if (motor != null)
            {
                motor.ResetMotion();
                motor.enabled = true;
            }

            if (knockback != null)
                knockback.ResetMotion();

            recoveryLockUntil = Time.time + 1.35f;

            if (fade != null)
                yield return fade.FadeOut();

            recovering = false;
        }
    }

    public sealed class RecoveryFadeUI : MonoBehaviour
    {
        private CanvasGroup group;
        private Text label;

        public static RecoveryFadeUI Show(string message)
        {
            GameObject old = GameObject.Find("RecoveryFadeUI");
            if (old != null)
                Destroy(old);

            GameObject root = new GameObject("RecoveryFadeUI");
            RecoveryFadeUI ui = root.AddComponent<RecoveryFadeUI>();
            ui.Build(message);
            return ui;
        }

        private void Build(string message)
        {
            GameObject canvasGo = new GameObject("Canvas");
            canvasGo.transform.SetParent(transform, false);

            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 140;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            GameObject panel = new GameObject("Fade");
            panel.transform.SetParent(canvasGo.transform, false);

            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Image image = panel.AddComponent<Image>();
            image.color = Color.black;

            group = panel.AddComponent<CanvasGroup>();
            group.alpha = 0f;

            GameObject textGo = new GameObject("Label");
            textGo.transform.SetParent(panel.transform, false);

            RectTransform textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.20f, 0.42f);
            textRect.anchorMax = new Vector2(0.80f, 0.58f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            label = textGo.AddComponent<Text>();
            label.font = UiFontProvider.Get();
            label.text = message;
            label.fontSize = 30;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
        }

        public IEnumerator FadeIn()
        {
            float age = 0f;
            const float duration = 0.34f;
            while (age < duration)
            {
                age += Time.unscaledDeltaTime;
                group.alpha = Mathf.Clamp01(age / duration);
                yield return null;
            }
            group.alpha = 1f;
        }

        public IEnumerator FadeOut()
        {
            float age = 0f;
            const float duration = 0.42f;
            while (age < duration)
            {
                age += Time.unscaledDeltaTime;
                group.alpha = 1f - Mathf.Clamp01(age / duration);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
