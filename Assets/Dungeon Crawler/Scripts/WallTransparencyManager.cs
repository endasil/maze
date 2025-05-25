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

    private readonly RaycastHit[] _hits = new RaycastHit[64];
    private readonly Dictionary<Renderer, WallData> _walls = new();
    private readonly HashSet<Renderer> _current = new();
    private float _nextCheck;

    private sealed class WallData
    {
        public readonly Material Opaque;
        public readonly Material Transparent;
        public readonly MaterialPropertyBlock Block = new();
        public Coroutine Fade;
        public float Alpha = 1f;

        public WallData(Material original)
        {
            Opaque = new Material(original);
            Transparent = new Material(original);
            ConvertToTransparent(Transparent);
        }

        private static void ConvertToTransparent(Material mat)
        {
            mat.SetFloat("_Surface", 1f);
            mat.SetOverrideTag("RenderType", "Transparent");

            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);

            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_SURFACE_TYPE_OPAQUE");

            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
    }

    private void Update()
    {
        if (Time.time < _nextCheck) return;
        _nextCheck = Time.time + checkInterval;

        Vector3 camPos = transform.position;
        Vector3 dir = (player.position - camPos).normalized;
        float dist = Vector3.Distance(camPos, player.position);

        _current.Clear();
        
        int hitCount = Physics.SphereCastNonAlloc(camPos, sphereRadius, dir, _hits, dist, wallLayer, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hitCount; i++)
        {
            var hit = _hits[i];
            var rend = hit.collider.GetComponent<Renderer>();
            if (!rend) continue;

            // Only fade if back of wall is hit
            if (Vector3.Dot(hit.normal, dir) >= 0f) continue;

            _current.Add(rend);

            if (!_walls.TryGetValue(rend, out var data))
            {
                data = new WallData(rend.sharedMaterial);
                _walls.Add(rend, data);
            }

            if (data.Fade == null) data.Fade = StartCoroutine(Fade(rend, data, 0.1f));
        }

        foreach (var pair in _walls)
        {
            if (_current.Contains(pair.Key) || pair.Value.Fade != null) continue;
            pair.Value.Fade = StartCoroutine(Fade(pair.Key, pair.Value, 1f));
        }
    }

    private IEnumerator Fade(Renderer rend, WallData data, float targetAlpha)
    {
        bool goingTransparent = targetAlpha < 1f;
        bool goingOpaque      = targetAlpha >= 1f;

        if (goingTransparent && data.Alpha >= 1f)
            rend.material = data.Transparent;

        while (!Mathf.Approximately(data.Alpha, targetAlpha))
        {
            data.Alpha = Mathf.MoveTowards(data.Alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            data.Block.SetColor("_BaseColor", new Color(1f, 1f, 1f, data.Alpha));
            rend.SetPropertyBlock(data.Block);
            yield return null;
        }

        if (goingOpaque)
        {
            rend.SetPropertyBlock(null);
            yield return null; // ✅ wait one frame before swapping back
            rend.material = data.Opaque;
        }

        data.Fade = null;
    }

}
