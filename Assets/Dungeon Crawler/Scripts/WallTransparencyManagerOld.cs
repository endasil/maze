using UnityEngine;
using System.Collections.Generic;

public class WallTransparencyManagerOld : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private float sphereRadius = 0.5f;

    private const int MaxWallHits = 32;
    private readonly RaycastHit[] _hitBuffer = new RaycastHit[MaxWallHits];

    private readonly HashSet<FadeWall> _currentWalls = new();
    private readonly HashSet<FadeWall> _previousFrameWalls = new();
    private float _lastCheckTime;

    private void Start()
    {
        FadeWall.SetSharedTransparentMaterial(transparentMaterial);
    }
    private void Update()
    {
        if (Time.time - _lastCheckTime < checkInterval) return;
        _lastCheckTime = Time.time;

        Vector3 camPos = transform.position;
        Vector3 dir = (player.position - camPos).normalized;
        float dist = Vector3.Distance(camPos, player.position);
        
        _previousFrameWalls.Clear();
        // Copy current walls to previous frames list of walls. 
        _previousFrameWalls.UnionWith(_currentWalls);
        _currentWalls.Clear();

        // SphereCast catches walls more reliably than thin Raycast
        var size = Physics.SphereCastNonAlloc(camPos, sphereRadius, dir, _hitBuffer, dist, wallLayer, QueryTriggerInteraction.Ignore);
        
        for (int i = 0; i < size; i++)
        {
            FadeWall wall = _hitBuffer[i].collider.GetComponent<FadeWall>();
            if (!wall) continue;
            
            Vector3 rayDir = (player.position - camPos).normalized;
            Vector3 hitPoint = _hitBuffer[i].point;
            Vector3 normal = _hitBuffer[i].normal;

            float facingDot = Vector3.Dot(normal, rayDir);

            // Draw wall surface normal in green
            Debug.DrawRay(hitPoint, normal, Color.green, 1f);

            // Draw camera-to-player ray in red (direction we're casting)
            Debug.DrawRay(camPos, rayDir * Vector3.Distance(camPos, player.position), Color.red, 1f);
            
            if (facingDot < 0)
            {
                Debug.Log($"facingDot {facingDot}");
                // Player is behind the wall
                wall.FadeToTransparent();
                _currentWalls.Add(wall);
            }
            else
            {
                Debug.Log("Player not behind the wall");
            }
            
        }

        foreach (FadeWall wall in _previousFrameWalls)
        {
            if (!_currentWalls.Contains(wall))
                wall.FadeToOpaque();
        }
    }
}