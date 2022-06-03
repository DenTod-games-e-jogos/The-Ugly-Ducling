using UnityEngine;
using VoxelMaster;

public class GenerationExample : BaseGeneration
{
    public override short Generation(int x, int y, int z) // The generation action only takes coordinates, you must return a block ID. 
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

        if (y > height)
        {
            if (treeTrunk >= 0.75f && oceanBiome < 0.4f && desertBiome < 0.4f && height > 15 && y <= height + 5)
                return 5;
            else if (treeLeaves * Mathf.Clamp01(1 - Vector2.Distance(new Vector2(y, 0), new Vector2(height + 7, 0)) / 5f) >= 0.25f && treeTrunk <= 0.925f && oceanBiome < 0.4f && desertBiome < 0.4f && height > 15)
                return 6;
        }
        if (y <= height && y >= 0)
        {
            if (y == height && height > 2) // Grass or sand layer
            {
                if (oceanBiome >= 0.1f && height < 16)
                    return 3;
                else
                    return (desertBiome >= 0.5f ? (short)3 : (short)0);
            }
            else if (y >= height - 6 && height > 6) // Dirt or sand layer
            {
                return (desertBiome >= 0.5f ? (short)3 : (short)2);
            }
            else if (y > 0) // Stone layer
            {
                return 1;
            }
            else // Bedrock layer
            {
                return 4;
            }
        }
        else // Else there shall be nothing!
        {
            return -1;
        }
    }
}