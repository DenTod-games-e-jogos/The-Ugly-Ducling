using UnityEngine;
using VoxelMaster;

[RequireComponent(typeof(VoxelTerrain))]
public class ShapesExample : MonoBehaviour
{
    VoxelTerrain terrain;

    void Awake()
    {
        terrain = GetComponent<VoxelTerrain>(); // Get the terrain
        Generate();
    }

    void Generate()
    {
        terrain.Fill(new Vector3(-10, 0, -10), new Vector3(10, 1, 10), 1); // Fill a zone.
        terrain.Sphere(new Vector3(0, 10, 0), 10, 1); // Create a sphere
        terrain.Sphere(new Vector3(5, 15, -5), 5, -1); // Carve in that last sphere

        terrain.FastRefresh(); // Refresh!
    }
}