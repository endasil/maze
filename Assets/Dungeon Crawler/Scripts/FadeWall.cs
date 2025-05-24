using System.Collections;
using UnityEngine;

public class FadeWall : MonoBehaviour
{
    private Renderer _renderer;
    private Material _transparentMaterialInstance;
    private Material _opaqueMaterialInstance;    
    private Coroutine _fadeCoroutine;
    private bool _isTransparent;
    private readonly float _fadeSpeed = 3f;
    
    private static Material _sharedTransparentMaterial;
    
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _opaqueMaterialInstance = Instantiate(_renderer.material);
        _renderer.material = _opaqueMaterialInstance;
        
    }

    public static void SetSharedTransparentMaterial(Material transparentMaterial)
    {
        _sharedTransparentMaterial = transparentMaterial;
    }
    
    private void EnsureTransparentInstance()
    {
        if (!_sharedTransparentMaterial)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Debug.LogError(
                "Shared transparent material not assigned. SetSharedTransparentMaterial should have been called from the manager class before this.");
        }

        if (!_transparentMaterialInstance)
            _transparentMaterialInstance = new Material(_sharedTransparentMaterial);
    }

    public void FadeToTransparent()
    {
        if (!_isTransparent)
        {
            EnsureTransparentInstance();
            _renderer.material = _transparentMaterialInstance;
            _isTransparent = true;
        }

        StartFade(0.3f);
    }
    public void FadeToOpaque()
    {
        if (_isTransparent)
        {
            StartFade(1f);
        }
    }

    private void StartFade(float targetAlpha)
    {
        if(_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        
        _fadeCoroutine = StartCoroutine(FadeAlpha(_renderer.material, targetAlpha));
    }

    private IEnumerator FadeAlpha(Material material, float targetAlpha)
    {
        Color color = material.color;
        float from = color.a;
        while (!Mathf.Approximately(color.a, targetAlpha))
        {
            float step = _fadeSpeed * Time.deltaTime * Mathf.Sign(targetAlpha - from);
            color.a = Mathf.MoveTowards(color.a, targetAlpha, Mathf.Abs(step));
            material.color = color;
            yield return null;
        }
        
        if(Mathf.Approximately(targetAlpha, 1f) && _isTransparent)
        {
            _renderer.material = _opaqueMaterialInstance;
            _isTransparent = false;
        }
        _fadeCoroutine = null;
    }
    

    
    
}
