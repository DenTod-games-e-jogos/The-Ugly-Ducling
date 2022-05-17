using UnityEngine;
using System.Collections;
using VoxelMaster;

public class LoadingExample : MonoBehaviour
{
    public VoxelTerrain terrain;

    void Start()
    {
        if (VoxelTerrain.WorldExists("LoadingExample"))
        {
            terrain.LoadWorld("LoadingExample", false);
        }

        terrain.FastRefresh();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            terrain.SaveWorld("LoadingExample");
        }
    }
}