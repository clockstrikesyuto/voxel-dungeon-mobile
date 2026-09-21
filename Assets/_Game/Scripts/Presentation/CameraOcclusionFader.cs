using System.Collections.Generic;
using UnityEngine;

namespace VoxelDungeon.Presentation
{
    public sealed class CameraOcclusionFader : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private LayerMask mask = ~0;
        [SerializeField, Range(0.05f, 0.8f)] private float fadedAlpha = 0.16f;

        private readonly Dictionary<Renderer, Color> originalColors = new();
        private readonly HashSet<Renderer> fadedThisFrame = new();

        public void SetTarget(Transform value) => target = value;

        private void LateUpdate()
        {
            if (target == null)
                return;

            fadedThisFrame.Clear();

            Vector3 start = transform.position;
            Vector3 end = target.position + Vector3.up * 0.9f;
            Vector3 direction = end - start;
            float distance = direction.magnitude;

            RaycastHit[] hits = Physics.RaycastAll(
                start,
                direction.normalized,
                distance,
                mask,
                QueryTriggerInteraction.Ignore);

            foreach (RaycastHit hit in hits)
            {
                Renderer renderer = hit.collider != null
                    ? hit.collider.GetComponent<Renderer>()
                    : null;

                if (renderer == null)
                    continue;

                string n = renderer.gameObject.name;
                if (!n.Contains("Ceiling") &&
                    !n.Contains("Roof") &&
                    !n.Contains("ArchTop") &&
                    !n.Contains("Wall") &&
                    !n.Contains("Pillar"))
                    continue;

                Fade(renderer);
                fadedThisFrame.Add(renderer);
            }

            List<Renderer> restore = new List<Renderer>();
            foreach (var pair in originalColors)
            {
                if (!fadedThisFrame.Contains(pair.Key))
                    restore.Add(pair.Key);
            }

            foreach (Renderer renderer in restore)
                Restore(renderer);
        }

        private void Fade(Renderer renderer)
        {
            if (renderer == null || renderer.material == null)
                return;

            Material material = renderer.material;

            Color color;
            if (material.HasProperty("_BaseColor"))
                color = material.GetColor("_BaseColor");
            else if (material.HasProperty("_Color"))
                color = material.GetColor("_Color");
            else
                return;

            if (!originalColors.ContainsKey(renderer))
                originalColors[renderer] = color;

            color.a = fadedAlpha;

            material.SetOverrideTag("RenderType", "Transparent");
            if (material.HasProperty("_Surface")) material.SetFloat("_Surface", 1f);
            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0f);
            material.renderQueue = 3000;

            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            else
                material.SetColor("_Color", color);
        }

        private void Restore(Renderer renderer)
        {
            if (renderer == null)
            {
                originalColors.Remove(renderer);
                return;
            }

            if (!originalColors.TryGetValue(renderer, out Color color))
                return;

            Material material = renderer.material;
            if (material != null)
            {
                if (material.HasProperty("_BaseColor"))
                    material.SetColor("_BaseColor", color);
                else if (material.HasProperty("_Color"))
                    material.SetColor("_Color", color);

                if (material.HasProperty("_Surface")) material.SetFloat("_Surface", 0f);
                if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 1f);
                material.SetOverrideTag("RenderType", "Opaque");
                material.renderQueue = -1;
            }

            originalColors.Remove(renderer);
        }
    }
}
