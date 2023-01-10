using UnityEngine;
using VoxelMaster;

public class MapGenerator : BaseGeneration
{
    [Header("Structure Placer Script")]
    [SerializeField]
    MapStructuresPlacer mapPlacer;
    
    [Header("Map Size and Parameters")]
    [SerializeField] 
    int nBiomes;
    int mapLimit = 100000;

    public int MapLimit { get => mapLimit; private set => mapLimit = value; }

    public int NBiomes { get => nBiomes; private set => nBiomes = value; }
    
    [SerializeField] 
    int frontier;

    public int Frontier { get => frontier; private set => frontier = value; }
    
    [SerializeField] 
    float noiseScale;

    [Header("Florest Parameters")]
    [SerializeField] 
    float treeHeight = 10;

    [Header("Purple Mangrove Parameters")]
    [SerializeField]
    float lakeSize = 10f;
    
    [SerializeField]
    float mangroveTreeHeight = 4f;

    float radio;

    float radio2;

    float startArea;

    float startArea2;

    float storehouseArea;

    float storehouseArea2;

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
    
    short pantanoGrass = 5;
    
    short planiceGrass = 7;

    short MangroveLake = 8;


    new public void Start()
    {
        endWall = planiceGrass;

        bioma1 = florestaGrass;
        
        bioma2 = pantanoGrass;
        
        bioma3 = desertoGrass;
        
        bioma4 = planiceGrass;
    }

    public override short Generation(int x, int y, int z)
    {
        float height = 0f;

        float mountainContrib = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 150f, z / 150f), 0.33f, 0.66f, 0, 1) * 40f;

        float desertContrib = 0f;

        float oceanContrib = 0f;

        float detailContrib = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 20f, z / 20f), 0, 1, -1, 1) * 5f;

        float detailMult = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 30f, z / 30f), 0.33f, 0.66f, 0, 1);

        float mountainBiome = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 100f, z / 100f), 0.33f, 0.66f, 0, 1);

        float desertBiome = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 300f, z / 300f), 0.33f, 0.66f, 0, 1) 
        * VoxelGeneration.Remap(Mathf.PerlinNoise(x / 25f, z / 25f), 0.33f, 0.66f, 0.95f, 1.05f);

        float oceanBiome = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 500f, z / 500f), 0.33f, 0.66f, 0, 1);

        float mountainFinal = (mountainContrib * mountainBiome) + (detailContrib * detailMult) + 20;

        float desertFinal = (desertContrib * desertBiome) + (detailContrib * detailMult) + 20;

        float oceanFinal = (oceanContrib * oceanBiome);

        height = Mathf.Lerp(mountainFinal, desertFinal, desertBiome);

        height = Mathf.Lerp(height, oceanFinal, oceanBiome);

        height = Mathf.Floor(height);

        radio = (x * x) + (z * z);

        radio2 = Mathf.Sqrt(radio);

        if (radio2 > mapLimit)
        {
            return EndWall(x, y, z, height);
        }

        if ((radio2 <= mapLimit) && (radio2 > mapLimit - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return EndWall(x, y, z, height);
            }

            else if (y == 0)
            {
                return Bioma4(x, y, z, height);
            }
        }

        if ((radio2 <= mapLimit - frontier) && (radio2 > (3 * mapLimit / 4) + frontier))
        {
            return Bioma4(x, y, z, height);
        }

        if ((radio2 <= (3 * mapLimit / 4) + frontier) && (radio2 > (3 * mapLimit / 4) - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return Bioma4(x, y, z, height);
            }

            else
            {
                return Bioma3(x, y, z, height);
            }
        }

        if ((radio2 <= (3 * mapLimit / 4) - frontier) && (radio2 > (2 * mapLimit / 4) + frontier))
        {
            return Bioma3(x, y, z, height);
        }

        if ((radio2 <= (2 * mapLimit / 4) + frontier) && (radio2 > (2 * mapLimit / 4) - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return Bioma3(x, y, z, height);
            }

            else
            {
                return Bioma2(x, y, z, height);
            }
        }

        if ((radio2 <= (2 * mapLimit / 4) - frontier) && (radio2 > (1 * mapLimit / 4) + frontier))
        {
            return Bioma2(x, y, z, height);
        }

        if ((radio2 <= (1 * mapLimit / 4) + frontier) && (radio2 > (1 * mapLimit / 4) - frontier))
        {
            if (Mathf.PerlinNoise(x / noiseScale, z / noiseScale) >= 0.5f)
            {
                return Bioma2(x, y, z, height);
            }

            else
            {
                return Bioma1(x, y, z, height);
            }
        }

        if (radio2 <= (1 * mapLimit / 4) - frontier)
        {
            return Bioma1(x, y, z, height);
        }

        return air;
    }

    short Bioma1(int x, int y, int z, float height)
    {
        float treeTrunk = Mathf.PerlinNoise(x / 0.3543f, z / 0.3543f);

        float treeLeaves = Mathf.PerlinNoise(x / 5f, z / 5f);

        float florestArea = Mathf.PerlinNoise(x, z);

        if (y > height)
        {
            // Cria uma área para o start
            //startArea = ((x * x) - mapPlacer.StartPoint.x) + ((z * z) - mapPlacer.StartPoint.z);
            Vector2 startArea1 = new Vector2(mapPlacer.StartPoint.x - startArea / 2, mapPlacer.StartPoint.z - startArea / 2);
            Vector2 startArea2 = new Vector2(mapPlacer.StartPoint.x + startArea / 2, mapPlacer.StartPoint.z + startArea / 2);

            //startArea2 = Mathf.Sqrt(startArea);

            //if (startArea2 < mapPlacer.StartAreaRadius)
            if (((x > startArea1.x) && (x < startArea2.x)) && ((z > startArea1.y) && (z < startArea2.y)))
            {
                return air;
            }

            // Cria uma área para o colocar o Storehause
            //storehouseArea = ((x * x) - mapPlacer.StorehouseLocation.x) + ((z * z) - mapPlacer.StorehouseLocation.z);
            Vector2 storehouseArea1 = new Vector2(mapPlacer.StorehouseLocation.x - mapPlacer.StorehouseSize / 2, mapPlacer.StorehouseLocation.z - mapPlacer.StorehouseSize / 2);
            Vector2 storehouseArea2 = new Vector2(mapPlacer.StorehouseLocation.x + mapPlacer.StorehouseSize / 2, mapPlacer.StorehouseLocation.z + mapPlacer.StorehouseSize / 2);

            //storehouseArea2 = Mathf.Sqrt(storehouseArea);

            //if (storehouseArea2 < mapPlacer.StorehouseSize)
            if (((x > storehouseArea1.x) && (x < storehouseArea2.x)) && ((z > storehouseArea1.y) && (z < storehouseArea2.y)))
            {
                return air;
            }
            
            if (florestArea >= 0.25f)
            {
                if (treeTrunk >= 0.75f && height > 0 && y <= height + treeHeight)
                {
                    return desertoDirty;
                }

                else if (treeLeaves * Mathf.Clamp01(1 - Vector2.Distance(new Vector2(y, 0),
                new Vector2(height + treeHeight+2, 0)) / 5f) >= 0.25f && treeTrunk <= 0.925f)
                {
                    return florestaDirty;
                }

                else
                {
                    return air;
                }
            }

            else
            {
                return air;
            }
        }

        return bioma1;
    }

    short Bioma2(int x, int y, int z, float height)
    {
        float treeTrunk = Mathf.PerlinNoise(x / 0.3543f, z / 0.3543f);

        float treeLeaves = Mathf.PerlinNoise(x / 1.0f, z / 1.0f);

        float florestArea = Mathf.PerlinNoise(x, z);

        // Define local do lago
        if ((x > mapPlacer.LakeLocal.x - mapPlacer.LakeSize) &&
            (x < mapPlacer.LakeLocal.x + mapPlacer.LakeSize) &&
            (z > mapPlacer.LakeLocal.z - mapPlacer.LakeSize) &&
            (z < mapPlacer.LakeLocal.z + mapPlacer.LakeSize))
        {
            if (y <= height - 1)
            {
                return bioma2;
            }
            if (y <= height)
            {
                return bioma3;
            }
            if (y > height)
            {
                return air;
            }
        }

        if (y > height)
        {
            if (florestArea >= 0.0f)
            {
                if (treeTrunk >= .92f && height > 0 && y <= height + mangroveTreeHeight)
                {
                    return desertoDirty;
                }

                for (int i = x-2; i<= x+2; i++)
                {
                    for (int j = z-2; j <= z + 2; j++)
                    {
                        if (Mathf.PerlinNoise(i / 0.3543f, j / 0.3543f) >= .92f)
                        {
                            if(y > (height + mangroveTreeHeight) && y < (height + mangroveTreeHeight + 5))
                            {
                                return florestaDirty;
                            }
                        }
                    }
                }

                return air;
            }

            else
            {
                return air;
            }
        }

        return bioma2;
    }

    short Bioma3(int x, int y, int z, float height)
    {
        if (y > height)
        {
            return air;
        }

        return bioma3;
    }

    short Bioma4(int x, int y, int z, float height)
    {
        if (y > height)
        {
            return air;
        }

        return bioma4;
    }

    short EndWall(int x, int y, int z, float height)
    {
        if (y != 0)
        {
            if (x * x + z * z < (mapLimit / nBiomes) * (mapLimit / nBiomes) + 1)
            {
                return 0;
            }

            if (x * x + z * z < (2 * mapLimit / nBiomes) * (2 * mapLimit / nBiomes) + 1)
            {
                return 2;
            }

            if (x * x + z * z < (3 * mapLimit / nBiomes) * (3 * mapLimit / nBiomes) + 1)
            {
                return 5;
            }

            if (x * x + z * z < (4 * mapLimit / nBiomes) * (4 * mapLimit / nBiomes) + 1)
            {
                return 7;
            }
        }
        
        return -1;
    }
}