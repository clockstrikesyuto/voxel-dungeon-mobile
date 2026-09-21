using UnityEngine;
using UnityEngine.Rendering;

namespace VoxelDungeon.Core
{
    [RequireComponent(typeof(Collider))]
    public sealed class BossArenaBarrier : MonoBehaviour
    {
        [SerializeField] private float revealDistance = 2.6f;
        [SerializeField, Range(0.01f, 0.40f)] private float farAlpha = 0.075f;
        [SerializeField, Range(0.05f, 0.55f)] private float nearAlpha = 0.20f;

        private Renderer barrierRenderer;
        private Transform player;
        private Material glassMaterial;
        private LineRenderer edge;
        private Color glassColor = new Color(0.58f, 0.40f, 1f, 0.10f);

        private void Awake()
        {
            barrierRenderer = GetComponent<Renderer>();
            BuildGlassMaterial();
            BuildEdge();
        }

        private void OnEnable()
        {
            ResolvePlayer();

            if (barrierRenderer != null)
                barrierRenderer.enabled = true;

            ApplyAlpha(farAlpha);
        }

        private void Update()
        {
            if (player == null)
            {
                ResolvePlayer();
                return;
            }

            if (barrierRenderer == null)
                return;

            float planeDistance = Mathf.Abs(
                Vector3.Dot(
                    player.position - transform.position,
                    transform.forward));

            float proximity = 1f - Mathf.Clamp01(planeDistance / revealDistance);
            proximity = proximity * proximity * (3f - 2f * proximity);

            float alpha = Mathf.Lerp(farAlpha, nearAlpha, proximity);
            ApplyAlpha(alpha);
        }

        private void BuildGlassMaterial()
        {
            if (barrierRenderer == null)
                return;

            Material source = barrierRenderer.sharedMaterial;

            if (source != null)
            {
                if (source.HasProperty("_BaseColor"))
                    glassColor = source.GetColor("_BaseColor");
                else if (source.HasProperty("_Color"))
                    glassColor = source.GetColor("_Color");
            }

            glassColor = new Color(
                Mathf.Lerp(glassColor.r, 0.72f, 0.35f),
                Mathf.Lerp(glassColor.g, 0.78f, 0.45f),
                Mathf.Lerp(glassColor.b, 1f, 0.30f),
                farAlpha);

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");

            glassMaterial = new Material(shader);
            glassMaterial.name = "Runtime_BossGlassBarrier";

            if (glassMaterial.HasProperty("_BaseColor"))
                glassMaterial.SetColor("_BaseColor", glassColor);
            else if (glassMaterial.HasProperty("_Color"))
                glassMaterial.SetColor("_Color", glassColor);

            if (glassMaterial.HasProperty("_Metallic"))
                glassMaterial.SetFloat("_Metallic", 0.02f);

            if (glassMaterial.HasProperty("_Smoothness"))
                glassMaterial.SetFloat("_Smoothness", 0.92f);

            ConfigureTransparency(glassMaterial);

            barrierRenderer.material = glassMaterial;
            barrierRenderer.shadowCastingMode = ShadowCastingMode.Off;
            barrierRenderer.receiveShadows = false;
        }

        private static void ConfigureTransparency(Material material)
        {
            if (material == null)
                return;

            material.SetOverrideTag("RenderType", "Transparent");

            if (material.HasProperty("_Surface"))
                material.SetFloat("_Surface", 1f);

            if (material.HasProperty("_Blend"))
                material.SetFloat("_Blend", 0f);

            if (material.HasProperty("_SrcBlend"))
                material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);

            if (material.HasProperty("_DstBlend"))
                material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);

            if (material.HasProperty("_ZWrite"))
                material.SetFloat("_ZWrite", 0f);

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.DisableKeyword("_ALPHATEST_ON");
            material.renderQueue = (int)RenderQueue.Transparent;
        }

        private void BuildEdge()
        {
            GameObject frame = new GameObject("GlassEdge");
            frame.transform.SetParent(transform, false);
            frame.transform.localPosition = new Vector3(0f, 0f, -0.505f);

            edge = frame.AddComponent<LineRenderer>();
            edge.useWorldSpace = false;
            edge.loop = true;
            edge.positionCount = 4;
            edge.startWidth = 0.035f;
            edge.endWidth = 0.035f;
            edge.numCornerVertices = 2;
            edge.shadowCastingMode = ShadowCastingMode.Off;
            edge.receiveShadows = false;

            edge.SetPosition(0, new Vector3(-0.5f, -0.5f, 0f));
            edge.SetPosition(1, new Vector3(-0.5f, 0.5f, 0f));
            edge.SetPosition(2, new Vector3(0.5f, 0.5f, 0f));
            edge.SetPosition(3, new Vector3(0.5f, -0.5f, 0f));

            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null)
                edge.material = new Material(shader);
        }

        private void ApplyAlpha(float alpha)
        {
            Color color = glassColor;
            color.a = alpha;

            if (glassMaterial != null)
            {
                if (glassMaterial.HasProperty("_BaseColor"))
                    glassMaterial.SetColor("_BaseColor", color);
                else if (glassMaterial.HasProperty("_Color"))
                    glassMaterial.SetColor("_Color", color);
            }

            if (edge != null)
            {
                Color edgeColor = new Color(
                    Mathf.Min(1f, color.r + 0.15f),
                    Mathf.Min(1f, color.g + 0.15f),
                    1f,
                    Mathf.Clamp01(alpha + 0.16f));

                edge.startColor = edgeColor;
                edge.endColor = edgeColor;
            }
        }

        private void ResolvePlayer()
        {
            GameObject go = GameObject.Find("Player_Debug");
            if (go != null)
                player = go.transform;
        }

        private void OnDestroy()
        {
            if (glassMaterial != null)
                Destroy(glassMaterial);

            if (edge != null && edge.material != null)
                Destroy(edge.material);
        }
    }
}
