using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RampGenerator : MonoBehaviour
{
    Mesh myMesh;
    MeshFilter meshFilter;

    [SerializeField] Vector2 planeSize = new Vector2(5, 5);
    [SerializeField, Range(1, 50)] int planeResolution = 10;
    [SerializeField] Vector3 controlPoint = new Vector3(2.5f, 2f, 2.5f);

    List<Vector3> vertices;
    List<int> triangles;

    private void Awake()
    {
        myMesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = myMesh;
    }

    private void Update()
    {
        GeneratePlane(planeSize, planeResolution);
        CurvePlane();
        AssignMesh();
    }

    void GeneratePlane(Vector2 size, int resolution)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();

        float xStep = size.x / resolution;
        float zStep = size.y / resolution;

        for (int z = 0; z <= resolution; z++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                vertices.Add(new Vector3(x * xStep, 0, z * zStep));
            }
        }

        for (int row = 0; row < resolution; row++)
        {
            for (int col = 0; col < resolution; col++)
            {
                int i = row * (resolution + 1) + col;

                triangles.Add(i);
                triangles.Add(i + resolution + 1);
                triangles.Add(i + resolution + 2);

                triangles.Add(i);
                triangles.Add(i + resolution + 2);
                triangles.Add(i + 1);
            }
        }
    }

    void AssignMesh()
    {
        myMesh.Clear();
        myMesh.vertices = vertices.ToArray();
        myMesh.triangles = triangles.ToArray();
        myMesh.RecalculateNormals();
    }

    void CurvePlane()
    {
        // Curve along Z direction (like a ramp)
        Vector3 p0 = vertices[0];
        Vector3 p2 = vertices[^1];

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 v = vertices[i];
            float t = v.z / planeSize.y;

            // Get Bezier Y displacement along Z
            Vector3 bezierPoint = CalculateQuadraticBezierPoint(t, p0, controlPoint, p2);

            // Apply only the Y offset from Bezier to vertex height
            v.y = bezierPoint.y;
            vertices[i] = v;
        }
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}
