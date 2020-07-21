using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FogOfWar : MonoBehaviour
{

    public GameObject fogOfWarPlane;
    public Transform player;

    public LayerMask fogLayer;

    private static float radius = 27.0f;
    private float radiusSqr = radius * radius;
    private Color[] vertexColors;
    private Vector3[] vertices;

    // Start is called before the first frame update
    private Mesh fogOfWarMesh;
    void Start()
    {
        fogOfWarMesh = fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        vertices = fogOfWarMesh.vertices;
        
        // Transform all points in the plane from local to world space, was a bit expensive to do every frame.
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = fogOfWarPlane.transform.TransformPoint(vertices[i]);
        }

        vertexColors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertexColors[i] = Color.black;
        }
    }

    void UpdateColors()
    {
        fogOfWarMesh.colors = vertexColors;
    }

    // Update is called once per frame
    void Update()
    {
        return;
        // Create a ray starting at the camera going in the direction towards the player
        Ray cameraToPlayerRay = new Ray(transform.position, player.position - transform.position);

        // Check if it collides with fogofwar plane, if so search for all points close to it.
        if (Physics.Raycast(cameraToPlayerRay, out RaycastHit hit, 100, fogLayer, QueryTriggerInteraction.Collide))
        {
            Stopwatch sw = new Stopwatch();
            //sw.Start();
            bool updated = false;
            for (int i = 0; i < vertices.Length; i++)
            {
                float dist = Vector3.SqrMagnitude(vertices[i] - hit.point);
                if (dist < radiusSqr && vertexColors[i].a > 0.1f)
                {
                    updated = true;
                    // Every call to update colors on a mesh is a rather expensive thing.
                    // Storing a copy of the color array and then just swapping the array
                    // with a new one once all values are updated is much cheaper. 
                    vertexColors[i].a = Mathf.Min(vertexColors[i].a, dist / radiusSqr);
                }
            }
            
            if(updated)
                UpdateColors();
            //sw.Stop();
            //            Debug.Log(sw.ElapsedMilliseconds);

        }
    }
}
