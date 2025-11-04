using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class RampMaker : MonoBehaviour
{
    Mesh rampMesh;
    MeshFilter rampMeshFilter;

    [Header("Ramp Settings")]
    public float length, width, height, liple;
    public int resolution;

    List<Vector3> verticesList; // empty list of vertices that make up mesh
    List<int> trianglesList; // empty list of triangles that make up mesh

    private void Awake()
    {
        rampMesh = new Mesh();
        rampMeshFilter = GetComponent<MeshFilter>();
        rampMeshFilter.mesh = rampMesh;

        length = 1;
        width = 1;
        height = 1;
    }

    private void Update()
    {
        GenerateRamp();
    }

    void GenerateRamp()
    {
        verticesList = new List<Vector3>();
        trianglesList = new List<int>();

        // base setup
        Vector3 rampOrigin = Vector3.zero;
        float baseHalfLength = length / 2;
        float baseHalfWidth = width / 2;

        // base verts
        verticesList.Add(rampOrigin + (baseHalfLength * Vector3.back) + (baseHalfWidth * Vector3.left));
        verticesList.Add(rampOrigin + (baseHalfLength * Vector3.forward) + (baseHalfWidth * Vector3.left));
        verticesList.Add(rampOrigin + (baseHalfLength * Vector3.forward) + (baseHalfWidth * Vector3.right));
        verticesList.Add(rampOrigin + (baseHalfLength * Vector3.back) + (baseHalfWidth * Vector3.right));

        // base triangles
        trianglesList.Add(0);
        trianglesList.Add(2);
        trianglesList.Add(1);

        trianglesList.Add(0);
        trianglesList.Add(3);
        trianglesList.Add(2);

        // back setup
        Vector3 backOrigin = rampOrigin + (baseHalfLength * Vector3.forward);

        // back verts
        verticesList.Add(backOrigin + (baseHalfWidth * Vector3.left));
        verticesList.Add(backOrigin + (baseHalfWidth * Vector3.left) + (height * Vector3.up));
        verticesList.Add(backOrigin + (baseHalfWidth * Vector3.right) + (height * Vector3.up));
        verticesList.Add(backOrigin + (baseHalfWidth * Vector3.right));

        // back triangles
        trianglesList.Add(7);
        trianglesList.Add(6);
        trianglesList.Add(4);

        trianglesList.Add(4);
        trianglesList.Add(6);
        trianglesList.Add(5);

        rampMesh.Clear();
        rampMesh.vertices = verticesList.ToArray();
        rampMesh.triangles = trianglesList.ToArray();
    }

}
