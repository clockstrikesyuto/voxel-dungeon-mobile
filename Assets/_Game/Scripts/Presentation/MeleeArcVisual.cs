using UnityEngine;

namespace VoxelDungeon.Presentation
{
    public sealed class MeleeArcVisual : MonoBehaviour
    {
        private float age;
        private float lifetime = 0.16f;
        private LineRenderer line;

        public static void Spawn(Transform owner)
        {
            GameObject go = new GameObject("MeleeArc");
            go.transform.position = owner.position + Vector3.up * 0.9f;
            go.transform.rotation = owner.rotation;

            MeleeArcVisual visual = go.AddComponent<MeleeArcVisual>();
            visual.Build();
        }

        private void Build()
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.useWorldSpace = false;
            line.positionCount = 13;
            line.startWidth = 0.12f;
            line.endWidth = 0.035f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = new Color(1f, 0.92f, 0.55f, 0.95f);
            line.endColor = new Color(1f, 0.45f, 0.12f, 0.05f);

            const float radius = 1.5f;
            for (int i = 0; i < line.positionCount; i++)
            {
                float t = i / (float)(line.positionCount - 1);
                float angle = Mathf.Lerp(-62f, 62f, t) * Mathf.Deg2Rad;
                line.SetPosition(i, new Vector3(Mathf.Sin(angle) * radius, 0f, Mathf.Cos(angle) * radius));
            }
        }

        private void Update()
        {
            age += Time.deltaTime;
            float t = Mathf.Clamp01(age / lifetime);
            transform.localScale = Vector3.one * Mathf.Lerp(0.75f, 1.15f, t);

            if (age >= lifetime)
                Destroy(gameObject);
        }
    }
}
