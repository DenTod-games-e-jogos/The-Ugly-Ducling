using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelMaster;

public class MapGenerator : BaseGeneration
{
    [SerializeField] int mapLimit;
    [SerializeField] int nBiomes;

    void start()
    {
    }
    public override short Generation(int x, int y, int z) // The generation action only takes coordinates, you must return a block ID. 
    {
        if (x*x + z*z > mapLimit*mapLimit+1)
        {
            return 7;
        }

        if (y != 0)
        {
            return -1;
        }

        if (x*x + z*z < (mapLimit/nBiomes)*(mapLimit/nBiomes)+1)
        {
            return 0;
        }

        if (x*x + z*z < (2*mapLimit/nBiomes)*(2*mapLimit/nBiomes)+1)
        {
            return 2;
        }

        if (x*x + z*z < (3*mapLimit/nBiomes)*(3*mapLimit/nBiomes)+1)
        {
            return 5;
        }

        if (x*x + z*z < (4*mapLimit/nBiomes)*(4*mapLimit/nBiomes)+1)
        {
            return 7;
        }
        return -1;
    }
}
