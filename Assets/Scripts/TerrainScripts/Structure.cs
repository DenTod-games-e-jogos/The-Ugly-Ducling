using System.Collections.Generic;
using UnityEngine;

public static class Structure 
{
    public static void MakeTree (Vector3 Position, Queue<VoxelMod> Q, int MinTrunkHeight, int MaxTrunkHeight) 
    {
        int Height = (int)(MaxTrunkHeight * Noise.Get2DPerlin(new Vector2(Position.x, Position.z), 250f, 3f));

        if (Height < MinTrunkHeight)
        {
            Height = MinTrunkHeight;
        }

        for (int i = 1; i < Height; i++)
        {
            Q.Enqueue(new VoxelMod(new Vector3(Position.x, Position.y + i, Position.z), 6));
        }
        
        for (int x = -3; x < 4; x++) 
        {
            for (int y = 0; y < 7; y++) 
            {
                for (int z = -3; z < 4; z++) 
                {
                    Q.Enqueue(new VoxelMod(new Vector3(Position.x + x, 
                    Position.y + Height + y, Position.z + z), 11));
                }
            }
        }
    }

    public static void MakeHouse(Vector3 Position, Queue<VoxelMod> Q, int MinTrunkHeight, int MaxTrunkHeight)
    {
        int Height = (int)(MaxTrunkHeight * Noise.Get2DPerlin(new Vector2(Position.x, Position.z), 300f, 5f));

        if (Height < MinTrunkHeight)
        {
            Height = MinTrunkHeight;
        }

        for (int i = 1; i < Height; i++)
        {
            Q.Enqueue(new VoxelMod(new Vector3(Position.x, Position.y + i, Position.z), 10));
        }
        
        for (int x = -10; x < 4; x++) 
        {
            for (int y = 2; y < 7; y++) 
            {
                for (int z = -10; z < 4; z++) 
                {
                    Q.Enqueue(new VoxelMod(new Vector3(Position.x + x, 
                    Position.y + Height + y, Position.z + z), 15));
                }
            }
        }
    }
}