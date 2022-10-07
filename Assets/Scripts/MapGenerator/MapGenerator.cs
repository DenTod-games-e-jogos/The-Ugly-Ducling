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
    
    //short florestaRock = 4;
    
    short pantanoGrass = 5;
    
    //short pantanoDirty = 6;
    
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
        // Maths ahead! A lot of perlin noise mixed together to make some cool generation!

        float height = 0f;

        // Height data, regardless of biome
        float mountainContrib = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 150f, z / 150f), 0.33f, 0.66f, 0, 1) * 40f;
        float desertContrib = 0f;
        float oceanContrib = 0f;
        float detailContrib = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 20f, z / 20f), 0, 1, -1, 1) * 5f;

        // Biomes
        float detailMult = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 30f, z / 30f), 0.33f, 0.66f, 0, 1);
        float mountainBiome = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 100f, z / 100f), 0.33f, 0.66f, 0, 1);
        float desertBiome = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 300f, z / 300f), 0.33f, 0.66f, 0, 1) * VoxelGeneration.Remap(Mathf.PerlinNoise(x / 25f, z / 25f), 0.33f, 0.66f, 0.95f, 1.05f);
        float oceanBiome = VoxelGeneration.Remap(Mathf.PerlinNoise(x / 500f, z / 500f), 0.33f, 0.66f, 0, 1);

        // Add biome contrib
        float mountainFinal = (mountainContrib * mountainBiome) + (detailContrib * detailMult) + 20;
        float desertFinal = (desertContrib * desertBiome) + (detailContrib * detailMult) + 20;
        float oceanFinal = (oceanContrib * oceanBiome);

        // Final contrib
        height = Mathf.Lerp(mountainFinal, desertFinal, desertBiome); // Decide between mountain biome or desert biome
        height = Mathf.Lerp(height, oceanFinal, oceanBiome); // Decide between the previous biome or ocean biome (aka ocean biome overrides all biomes)

        height = Mathf.Floor(height);

        // Trees!
        float treeTrunk = Mathf.PerlinNoise(x / 0.3543f, z / 0.3543f);
        float treeLeaves = Mathf.PerlinNoise(x / 5f, z / 5f);

//        if (y > height)
//        {
//            if (treeTrunk >= 0.75f && oceanBiome < 0.4f && desertBiome < 0.4f && height > 15 && y <= height + 5)
//                return desertoDirty;
//            else if (treeLeaves * Mathf.Clamp01(1 - Vector2.Distance(new Vector2(y, 0), new Vector2(height + 7, 0)) / 5f) >= 0.25f && treeTrunk <= 0.925f && oceanBiome < 0.4f && desertBiome < 0.4f && height > 15)
//                return florestaDirty;
//        }

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

        if (y > height)
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
                return Bioma1(x, y, z, height);
            }
        }

        if (radio2 <= (1 * mapLimit / 4) - frontier)
        {
            return Bioma1(x, y, z, height);
        }

        return air;
    }

    private short Bioma1(int x, int y, int z, float height)
    {
        float treeTrunk = Mathf.PerlinNoise(x / 0.3543f, z / 0.3543f);
        float treeLeaves = Mathf.PerlinNoise(x / 5f, z / 5f);
        if (y > height)
        {
          //if (treeTrunk >= 0.75f && oceanBiome < 0.4f && desertBiome < 0.4f && height > 15 && y <= height + 5)
            if (treeTrunk >= 0.75f && height > 15 && y <= height + 5)
                return desertoDirty;
          //else if (treeLeaves * Mathf.Clamp01(1 - Vector2.Distance(new Vector2(y, 0), new Vector2(height + 7, 0)) / 5f) >= 0.25f && treeTrunk <= 0.925f && oceanBiome < 0.4f && desertBiome < 0.4f && height > 15)
            else if (treeLeaves * Mathf.Clamp01(1 - Vector2.Distance(new Vector2(y, 0), new Vector2(height + 7, 0)) / 5f) >= 0.25f && treeTrunk <= 0.925f && height > 15)
                return florestaDirty;
        }

        return bioma1;
    }
    //        if (y > height)
    //        {
    //            if (treeTrunk >= 0.75f && oceanBiome < 0.4f && desertBiome < 0.4f && height > 15 && y <= height + 5)
    //                return desertoDirty;
    //            else if (treeLeaves * Mathf.Clamp01(1 - Vector2.Distance(new Vector2(y, 0), new Vector2(height + 7, 0)) / 5f) >= 0.25f && treeTrunk <= 0.925f && oceanBiome < 0.4f && desertBiome < 0.4f && height > 15)
    //                return florestaDirty;
    //        }

}