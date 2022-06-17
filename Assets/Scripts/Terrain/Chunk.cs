using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;

	public MeshFilter meshFilter;

	void Start () 
    {
		int vertexIndex = 0;

		List<Vector3> vertices = new List<Vector3> ();
		
        List<int> triangles = new List<int> ();
		
        List<Vector2> uvs = new List<Vector2> ();

		for (int p = 0; p < 6; p++) 
        { 
			for (int i = 0; i < 6; i++) 
            {
				int triangleIndex = VoxelData.Tris [p, i];

				vertices.Add (VoxelData.Vertex [triangleIndex]);
				
                triangles.Add (vertexIndex);

				uvs.Add (VoxelData.Uvs [i]);

				vertexIndex++;
			}
		}

		Mesh mesh = new Mesh ();

		mesh.vertices = vertices.ToArray ();
		
        mesh.triangles = triangles.ToArray ();
		
        mesh.uv = uvs.ToArray ();

		mesh.RecalculateNormals ();

		meshFilter.mesh = mesh;
	}
}