using UnityEngine;
using VoxelMaster;

public class MapGenerator : BaseGeneration
{
    [SerializeField] 
    int mapLimit;
    
    [SerializeField] 
    int nBiomes;
    
    [SerializeField] 
    int frontier;
    
    [SerializeField] 
    float noiseScale;

    float radio;

    float radio2;
    
    short endWall = 7;
    
    short bioma1 = 2;
    
    short bioma2 = 5;
    
    short bioma3 = 0;
    
    short bioma4 = 7;

    short air = -1;
    
    short desertoGrass = 0;
    
    short desertoDirty = 1;
    
    short florestaGrass = 2;
    
    short florestaDirty = 3;
    
    short florestaRock = 4;
    
    short pantanoGrass = 5;
    
    short pantanoDirty = 6;
    
    short planiceGrass = 7;

    void start()
    {
        endWall = planiceGrass;

        bioma1 = florestaGrass;
        
        bioma2 = pantanoGrass;
        
        bioma3 = desertoGrass;
        
        bioma4 = planiceGrass;
    }

    public override short Generation(int x, int y, int z)
    {
        radio = (x * x) + (z * z);

        radio2 = Mathf.Sqrt(radio);

        if (radio2 > mapLimit)
        {
            return endWall;
        }

        if ((radio2 <= mapLimit) && (radio2 > mapLimit - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return planiceGrass;
            }

            else if (y == 0)
            {
                return bioma4;
            }
        }

        if (y != 0)
        {
            return air;
        }

        if ((radio2 <= mapLimit - frontier) && (radio2 > (3 * mapLimit / 4) + frontier))
        {
            return bioma4;
        }

        if ((radio2 <= (3 * mapLimit / 4) + frontier) && (radio2 > (3 * mapLimit / 4) - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return bioma4;
            }

            else
            {
                return bioma3;
            }
        }

        if ((radio2 <= (3 * mapLimit / 4) - frontier) && (radio2 > (2 * mapLimit / 4) + frontier))
        {
            return bioma3;
        }

        if ((radio2 <= (2 * mapLimit / 4) + frontier) && (radio2 > (2 * mapLimit / 4) - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return bioma3;
            }

            else
            {
                return bioma2;
            }
        }

        if ((radio2 <= (2 * mapLimit / 4) - frontier) && (radio2 > (1 * mapLimit / 4) + frontier))
        {
            return bioma2;
        }

        if ((radio2 <= (1 * mapLimit / 4) + frontier) && (radio2 > (1 * mapLimit / 4) - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return bioma2;
            }

            else
            {
                return bioma1;
            }
        }

        if (radio2 <= (1 * mapLimit / 4) - frontier)
        {
            return bioma1;
        }

        return air;
    }
}