using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour 
{
    public int seed;
    
    public BiomeAttributes biome;

    public Transform player;
    
    public Vector3 spawnPosition;

    public Material material;
    
    public Material transparentMaterial;

    public BlockType[] blocktypes;

    Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();

    public ChunkCoord playerChunkCoord;
    
    ChunkCoord playerLastChunkCoord;

    List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();

    List<Chunk> chunksToUpdate = new List<Chunk>();

    bool applyingModifications = false;

    Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    public GameObject debugScreen;

    void Start() 
    {
        Random.InitState(seed);

        spawnPosition = new Vector3((VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f, 
        VoxelData.ChunkHeight - 50f, (VoxelData.WorldSizeInChunks * VoxelData.ChunkWidth) / 2f);
        
        GenerateWorld();
        
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    void Update() 
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);

        if (!playerChunkCoord.Equals(playerLastChunkCoord))
        {
            CheckViewDistance();
        }

        if (modifications.Count > 0 && !applyingModifications)
        {
            StartCoroutine(ApplyModifications());
        }

        if (chunksToCreate.Count > 0)
        {
            CreateChunk();
        }

        if (chunksToUpdate.Count > 0)
        {
            UpdateChunks();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            debugScreen.SetActive(!debugScreen.activeSelf);
        }
    }

    void GenerateWorld () 
    {
        for (int x = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; 
        x < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; x++) 
        {
            for (int z = (VoxelData.WorldSizeInChunks / 2) - VoxelData.ViewDistanceInChunks; 
            z < (VoxelData.WorldSizeInChunks / 2) + VoxelData.ViewDistanceInChunks; z++) 
            {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, true);

                activeChunks.Add(new ChunkCoord(x, z));
            }
        }

        while (modifications.Count > 0) 
        {
            VoxelMod v = modifications.Dequeue();

            ChunkCoord c = GetChunkCoordFromVector3(v.position);

            if (chunks[c.x, c.z] == null) 
            {
                chunks[c.x, c.z] = new Chunk(c, this, true);

                activeChunks.Add(c);
            }

            chunks[c.x, c.z].modifications.Enqueue(v);

            if (!chunksToUpdate.Contains(chunks[c.x, c.z]))
            {
                chunksToUpdate.Add(chunks[c.x, c.z]);
            }
        }

        for (int i = 0; i < chunksToUpdate.Count; i++) 
        {
            chunksToUpdate[0].UpdateChunk();

            chunksToUpdate.RemoveAt(0);
        }

        player.position = spawnPosition;
    }

    void CreateChunk () 
    {
        ChunkCoord c = chunksToCreate[0];
        
        chunksToCreate.RemoveAt(0);
        
        activeChunks.Add(c);
        
        chunks[c.x, c.z].Init();
    }

    void UpdateChunks () 
    {
        bool updated = false;
        
        int index = 0;

        while (!updated && index < chunksToUpdate.Count - 1) 
        {
            if (chunksToUpdate[index].isVoxelMapPopulated) 
            {
                chunksToUpdate[index].UpdateChunk();

                chunksToUpdate.RemoveAt(index);
                
                updated = true;
            } 
            
            else
            {
                index++;
            }
        }
    }

    IEnumerator ApplyModifications () 
    {
        applyingModifications = true;

        int count = 0;

        while (modifications.Count > 0) 
        {
            VoxelMod v = modifications.Dequeue();

            ChunkCoord c = GetChunkCoordFromVector3(v.position);

            if (chunks[c.x, c.z] == null) 
            {
                chunks[c.x, c.z] = new Chunk(c, this, true);
                
                activeChunks.Add(c);
            }

            chunks[c.x, c.z].modifications.Enqueue(v);

            if (!chunksToUpdate.Contains(chunks[c.x, c.z]))
            {
                chunksToUpdate.Add(chunks[c.x, c.z]);
            }

            count++;

            if (count > 200) 
            {
                count = 0;

                yield return null;
            }
        }

        applyingModifications = false;
    }

    ChunkCoord GetChunkCoordFromVector3 (Vector3 pos) 
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);

        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        
        return new ChunkCoord(x, z);
    }

    public Chunk GetChunkFromVector3 (Vector3 pos) 
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);

        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        
        return chunks[x, z];
    }

    void CheckViewDistance () 
    {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);

        playerLastChunkCoord = playerChunkCoord;

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x++) 
        {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++) 
            {
                if (IsChunkInWorld (new ChunkCoord (x, z))) 
                {
                    if (chunks[x, z] == null) 
                    {
                        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this, false);
                        
                        chunksToCreate.Add(new ChunkCoord(x, z));
                    }  
                    
                    else if (!chunks[x, z].isActive) 
                    {
                        chunks[x, z].isActive = true;
                    }

                    activeChunks.Add(new ChunkCoord(x, z));
                }

                for (int i = 0; i < previouslyActiveChunks.Count; i++) 
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }

        foreach (ChunkCoord c in previouslyActiveChunks)
        {
            chunks[c.x, c.z].isActive = false;
        }
    }

    public bool CheckForVoxel (Vector3 pos) 
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
        {
            return false;
        }

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
        {
            return blocktypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isSolid;
        }

        return blocktypes[GetVoxel(pos)].isSolid;
    }

    public bool CheckIfVoxelTransparent (Vector3 pos) 
    {
        ChunkCoord thisChunk = new ChunkCoord(pos);

        if (!IsChunkInWorld(thisChunk) || pos.y < 0 || pos.y > VoxelData.ChunkHeight)
        {
            return false;
        }

        if (chunks[thisChunk.x, thisChunk.z] != null && chunks[thisChunk.x, thisChunk.z].isVoxelMapPopulated)
        {
            return blocktypes[chunks[thisChunk.x, thisChunk.z].GetVoxelFromGlobalVector3(pos)].isTransparent;
        }

        return blocktypes[GetVoxel(pos)].isTransparent;
    }


    public byte GetVoxel (Vector3 pos) 
    {
        int yPos = Mathf.FloorToInt(pos.y);

        if (!IsVoxelInWorld(pos))
        {
            return 0;
        }

        if (yPos == 0)
        {
            return 1;
        }

        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * 
        Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;
        
        byte voxelValue = 0;

        if (yPos == terrainHeight)
        {
            voxelValue = 3;
        }

        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
        {
            voxelValue = 5;
        }

        else if (yPos > terrainHeight)
        {
            return 0;
        }

        else
        {
            voxelValue = 2;
        }

        if (voxelValue == 2) 
        {
            foreach (Lode lode in biome.lodes) 
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                {
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                    {
                        voxelValue = lode.blockID;
                    }
                }
            }
        }

        if (yPos == terrainHeight) 
        {
            if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.treeZoneScale) > biome.treeZoneThreshold) 
            {
                if (Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.treePlacementScale) > 
                biome.treePlacementThreshold) 
                {
                    Structure.MakeTree(pos, modifications, biome.minTreeHeight, biome.maxTreeHeight);
                }
            }
        }

        return voxelValue;
    }

    bool IsChunkInWorld (ChunkCoord coord) 
    {
        if (coord.x > 0 && coord.x < VoxelData.WorldSizeInChunks - 1 && coord.z > 0 
        && coord.z < VoxelData.WorldSizeInChunks - 1)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    bool IsVoxelInWorld (Vector3 pos) 
    {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight 
        && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}

[System.Serializable]
public class BlockType 
{
    public string blockName;
    
    public bool isSolid;
    
    public bool isTransparent;

    public Sprite icon;

    [Header("Texture Values")]
    public int backFaceTexture;

    public int frontFaceTexture;
    
    public int topFaceTexture;
    
    public int bottomFaceTexture;
    
    public int leftFaceTexture;
    
    public int rightFaceTexture;

    public int GetTextureID (int faceIndex) 
    {
        switch (faceIndex) 
        {
            case 0:
            {
                return backFaceTexture;
            }

            case 1:
            {
                return frontFaceTexture;
            }

            case 2:
            {
                return topFaceTexture;
            }

            case 3:
            {
                return bottomFaceTexture;
            }

            case 4:
            {
                return leftFaceTexture;
            }

            case 5:
            {
                return rightFaceTexture;
            }

            default:
            {
                MonoBehaviour.print("Error in GetTextureID; invalid face index");

                return 0;
            }
        }
    }
}

public class VoxelMod 
{
    public Vector3 position;
    
    public byte id;

    public VoxelMod () 
    {
        position = new Vector3();

        id = 0;
    }

    public VoxelMod (Vector3 _position, byte _id) 
    {
        position = _position;

        id = _id;
    }
}