using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class WallTransparencyManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private float fadeSpeed = 3f;
    [Range(0f, 1f)]
    [SerializeField] private float transparencyLevel = 0.2f;
    private readonly RaycastHit[] _hits = new RaycastHit[64];
    private readonly Dictionary<Renderer, WallData> _walls = new();
    private readonly HashSet<Renderer> _current = new();
    private float _nextCheck;

    /* ─────────────────────────────────────────────────────────── */

    private sealed class WallData
    {
        public readonly Material Opaque;
        public readonly Material Transparent;
        public readonly MaterialPropertyBlock Block = new();

        public Coroutine Fade;
        public float Alpha = 1f;
        public float TargetAlpha = 1f;   // new

        public WallData(Material original)
        {
            Opaque = new Material(original);
            Transparent = new Material(original);
            ConvertToTransparent(Transparent);
        }

        /// <summary>
        /// Converts a material to a transparent version for URP with alpha blending.
        /// Keeps depth writing on to prevent back-face blending artifacts.
        /// </summary>
        private static void ConvertToTransparent(Material mat)
        {
            // Set surface type to Transparent (URP-specific convention)
            mat.SetFloat("_Surface", 1f);  // 0 = Opaque, 1 = Transparent

            // Enable standard alpha blending
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);

            // Enable depth writing to avoid inner-face blending issues
            mat.SetInt("_ZWrite", 1);

            // Shader feature keywords for URP transparency
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_SURFACE_TYPE_OPAQUE");

            // Render queue just after default transparent range
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + 1;
        }


    }

    /* ─────────────────────────────────────────────────────────── */

    private void Update()
    {
        if (Time.time < _nextCheck) return;
        _nextCheck = Time.time + checkInterval;

        Vector3 camPos = transform.position;
        Vector3 dir = (player.position - camPos).normalized;
        float dist = Vector3.Distance(camPos, player.position);

        _current.Clear();

        int hitCount = Physics.SphereCastNonAlloc(
            camPos, sphereRadius, dir, _hits, dist,
            wallLayer, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            var hit = _hits[i];
            var rend = hit.collider.GetComponent<Renderer>();
            if (!rend) continue;

            // back side only
            if (Vector3.Dot(hit.normal, dir) >= 0f) continue;

            _current.Add(rend);

            if (!_walls.TryGetValue(rend, out var data))
            {
                data = new WallData(rend.sharedMaterial);
                _walls.Add(rend, data);
            }

            StartFade(rend, data, transparencyLevel);
        }

        foreach (var pair in _walls)
        {
            if (_current.Contains(pair.Key)) continue;
            StartFade(pair.Key, pair.Value, 1f);
        }
    }

    /* ─────────────────────────────────────────────────────────── */

    private void StartFade(Renderer rend, WallData data, float targetAlpha)
    {
        if (Mathf.Approximately(data.TargetAlpha, targetAlpha)) return;

        if (data.Fade != null)
            StopCoroutine(data.Fade);

        data.TargetAlpha = targetAlpha;
        data.Fade = StartCoroutine(Fade(rend, data, targetAlpha));
    }

    private IEnumerator Fade(Renderer rend, WallData data, float targetAlpha)
    {
        bool toTransparent = targetAlpha < 1f;

        if (toTransparent && data.Alpha >= 1f)
            rend.material = data.Transparent;

        while (!Mathf.Approximately(data.Alpha, targetAlpha))
        {
            data.Alpha = Mathf.MoveTowards(data.Alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            data.Block.SetColor("_BaseColor", new Color(1f, 1f, 1f, data.Alpha));
            rend.SetPropertyBlock(data.Block);
            yield return null;
        }

        if (!toTransparent)          // back to opaque
        {
            rend.SetPropertyBlock(null);
            yield return null;       // wait a frame
            rend.material = data.Opaque;
        }

        data.Fade = null;
    }
}
