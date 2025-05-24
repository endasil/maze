using UnityEngine;
using System.Collections;

public class ToggleTransparency : MonoBehaviour
{
    private Renderer _renderer;
    private Material _opaqueMaterialInstance;
    [SerializeField] private Material _transparentMaterial;

    private Coroutine _fadeCoroutine;
    private bool _isTransparent = false;

    private float _targetAlpha = 1f;
    private float _fadeSpeed = 2f; // alpha units per second

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        _opaqueMaterialInstance = new Material(_renderer.material);
        _renderer.material = _opaqueMaterialInstance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float currentAlpha = _renderer.material.color.a;

            // Switch material only if fully opaque and going transparent
            if (!_isTransparent && Mathf.Approximately(currentAlpha, 1f))
            {
                _renderer.material = Material.Instantiate(_transparentMaterial);

                _isTransparent = true;
            }

            // Set new target direction
            _targetAlpha = (Mathf.Approximately(_targetAlpha, 1f)) ? 0.1f : 1f;
            Debug.Log("Setting target alpha to " + _targetAlpha);

            // Restart fade
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeAlpha(_renderer.material, _targetAlpha));
        }
    }

    private IEnumerator FadeAlpha(Material mat, float to)
    {
        Color color = mat.color;
        float from = color.a;

        while (!Mathf.Approximately(color.a, to))
        {
            float step = _fadeSpeed * Time.deltaTime * Mathf.Sign(to - from);
            color.a = Mathf.MoveTowards(color.a, to, Mathf.Abs(step));
            mat.color = color;
            yield return null;
        }

        color.a = to;
        mat.color = color;

        // If we reached alpha 1, switch back to opaque material
        if (Mathf.Approximately(to, 1f) && _isTransparent)
        {
            Debug.Log("Switching to opaque material");
            _renderer.material = _opaqueMaterialInstance;
            _isTransparent = false;
        }

        _fadeCoroutine = null;
    }
}
