using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class BlockGenerator : MonoBehaviour
{
    int Height = 512;

    int Width = 16;

    Vector2[] NewUV;

    Vector3[] NewVertices;

    Vector3[] Normals;

    int[] NewTriangles;

    Mesh mesh;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        mesh.Clear();

        mesh.vertices = NewVertices;

        mesh.uv = NewUV;

        mesh.triangles = NewTriangles;
    }
}