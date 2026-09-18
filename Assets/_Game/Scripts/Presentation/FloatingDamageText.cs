using UnityEngine;

namespace VoxelDungeon.Presentation
{
    public sealed class FloatingDamageText : MonoBehaviour
    {
        [SerializeField, Min(0.05f)] private float lifetime = 0.65f;
        [SerializeField, Min(0f)] private float riseSpeed = 1.6f;

        private TextMesh textMesh;
        private Camera mainCamera;
        private float age;
        private Color startColor;

        public void Initialize(int amount, bool critical)
        {
            textMesh = gameObject.AddComponent<TextMesh>();
            textMesh.text = critical ? $"{amount}!" : amount.ToString();
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.fontSize = critical ? 64 : 52;
            textMesh.characterSize = 0.055f;
            textMesh.color = critical
                ? new Color(1f, 0.82f, 0.18f, 1f)
                : new Color(1f, 1f, 1f, 1f);
            startColor = textMesh.color;
            mainCamera = Camera.main;
        }

        private void Update()
        {
            age += Time.deltaTime;
            transform.position += Vector3.up * (riseSpeed * Time.deltaTime);

            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera != null)
                transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);

            if (textMesh != null)
            {
                float t = Mathf.Clamp01(age / lifetime);
                Color c = startColor;
                c.a = 1f - t;
                textMesh.color = c;
            }

            if (age >= lifetime)
                Destroy(gameObject);
        }

        public static void Spawn(Vector3 position, int amount, bool critical)
        {
            GameObject go = new GameObject("DamageText");
            go.transform.position = position + Vector3.up * 0.35f;
            FloatingDamageText popup = go.AddComponent<FloatingDamageText>();
            popup.Initialize(amount, critical);
        }
    }
}
