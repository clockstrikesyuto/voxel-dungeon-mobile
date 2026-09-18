using UnityEngine;
using UnityEngine.EventSystems;

namespace VoxelDungeon.UI
{
    public sealed class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        [SerializeField, Range(0.1f, 1f)] private float handleRange = 0.55f;

        public Vector2 Value { get; private set; }

        public void Configure(RectTransform backgroundRect, RectTransform handleRect)
        {
            background = backgroundRect;
            handle = handleRect;
            ResetStick();
        }

        public void OnPointerDown(PointerEventData eventData) => UpdateStick(eventData);
        public void OnDrag(PointerEventData eventData) => UpdateStick(eventData);
        public void OnPointerUp(PointerEventData eventData) => ResetStick();

        private void UpdateStick(PointerEventData eventData)
        {
            if (background == null || handle == null)
                return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    background,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPoint))
                return;

            float radius = Mathf.Max(1f, Mathf.Min(background.rect.width, background.rect.height) * 0.5f);
            Value = Vector2.ClampMagnitude(localPoint / radius, 1f);
            handle.anchoredPosition = Value * radius * handleRange;
        }

        private void ResetStick()
        {
            Value = Vector2.zero;
            if (handle != null)
                handle.anchoredPosition = Vector2.zero;
        }
    }
}
