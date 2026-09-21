using System.Collections;
using UnityEngine;

namespace VoxelDungeon.UI
{
    public sealed class MissionResultAnimator : MonoBehaviour
    {
        private RectTransform card;
        private CanvasGroup group;

        public void Configure(RectTransform target, CanvasGroup canvasGroup)
        {
            card = target;
            group = canvasGroup;
        }

        private void Start()
        {
            if (card != null && group != null)
                StartCoroutine(Play());
        }

        private IEnumerator Play()
        {
            group.alpha = 0f;
            card.localScale = Vector3.one * 0.90f;

            float age = 0f;
            const float duration = 0.42f;

            while (age < duration)
            {
                age += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(age / duration);
                float eased = 1f - Mathf.Pow(1f - t, 3f);

                group.alpha = eased;
                float overshoot = Mathf.Sin(t * Mathf.PI) * 0.025f;
                card.localScale = Vector3.one * (Mathf.Lerp(0.90f, 1f, eased) + overshoot);
                yield return null;
            }

            group.alpha = 1f;
            card.localScale = Vector3.one;
        }
    }
}
