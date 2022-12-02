using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace VoxelMaster
{
    public class VoxelTerrain : MonoBehaviour
    {
        public static readonly Queue<UnityAction> MainThread = new Queue<UnityAction>(); 

        public static readonly Queue<UnityAction> ColliderBuffer = new Queue<UnityAction>();

        [Header("Main properties")]
        [Range(2, 16)]
        [Tooltip("The size of the chunk, 16 is a good value.")]
        public int ChunkSize = 16;

        [Space(10)]
        [Tooltip("The render distance of the chunk.")]
        public float renderDistance = 128f;

        [Tooltip("The dispose distance of the chunk, if chunks are further than this distance & wasn't modified in any way, it'll destroy it. Higher dispose distances means higher RAM usage.")]
        public float disposeDistance = 256f;

        [Tooltip("Ignores the Y axis for distance checking, if enabled, it can avoid underneath & above chunks from suddenly dissapearing, render feels more \"Minecraft-like\"")]
        public bool ignoreYAxis = false;

        [Tooltip("The interval between static batching operations (-1 to disable static batching)")]
        public float staticBatchingInterval = 1f;

        [Tooltip("Should we enable colliders for the chunk?")]
        public bool enableColliders = true;

        [Header("Block properties")]
        [Tooltip("The block dictionary to use. A block dictionary contains info about the blocks, their id, their textures and a few other informations.")]
        public BlockDictionary blockDictionary;

        [Tooltip("The material to use along with its texture.")]
        public Material material;

        [Tooltip("The tiling of the material, for example, 16 will split the texture into 16x16 parts.")]
        public int tiling = 16;

        [Tooltip("The UV Padding of the textures, in deferred rendering you should leave this at 0, if you experience black lines at the edges of blocks, increase this value slightly until the problem is fixed.")]
        [Range(0, 0.4999f)]
        public float UVPadding = 0f;

        public Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();

        public bool dirty
        {
            get
            {
                return chunks.Values.Where(o => o.dirty == true).Count() > 0;
            }
        }

        public bool batchDirty
        {
            get
            {
                bool modified = chunks.Count != lastBatch_chunkCount;

                return dirty || modified;
            }
        }

        int lastBatch_chunkCount = 0;

        public bool ChunkExists(int x, int y, int z)
        {
            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;
            
            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk c = null; chunks.TryGetValue(new Vector3(x, y, z), out c);

            return c != null;
        }

        public bool ChunkExists(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);
            
            int z = Mathf.FloorToInt(pos.z);

            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;
            
            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk c = null; chunks.TryGetValue(new Vector3(x, y, z), out c);

            return c != null;
        }

        public Chunk FindChunk(int x, int y, int z)
        {
            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;
            
            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk c = null; chunks.TryGetValue(new Vector3(x, y, z), out c);

            return c;
        }

        public Chunk FindChunk(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);
            
            int z = Mathf.FloorToInt(pos.z);

            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;
            
            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk c = null; chunks.TryGetValue(new Vector3(x, y, z), out c);

            return c;
        }

        public Chunk FindOrCreateChunk(int x, int y, int z)
        {
            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;

            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk c = null; chunks.TryGetValue(new Vector3(x, y, z), out c);

            if (c != null)
            {
                return c;
            }

            Chunk newChunk = new Chunk(x, y, z, ChunkSize);
            
            AddChunk(newChunk);

            return newChunk;
        }

        public Chunk FindOrCreateChunk(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);
            
            int z = Mathf.FloorToInt(pos.z);

            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;

            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk c = null; chunks.TryGetValue(new Vector3(x, y, z), out c);

            if (c != null)
            {
                return c;
            }

            Chunk newChunk = new Chunk(x, y, z, ChunkSize);
            
            AddChunk(newChunk);

            return newChunk;
        }

        public Chunk CreateChunk(int x, int y, int z)
        {
            Chunk newChunk = new Chunk(x, y, z, ChunkSize);
            
            AddChunk(newChunk);

            return newChunk;
        }

        public Chunk CreateChunk(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);
            
            int z = Mathf.FloorToInt(pos.z);

            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;
            
            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk newChunk = new Chunk(x, y, z, ChunkSize);
            
            AddChunk(newChunk);

            return newChunk;
        }

        public void DeleteChunk(int x, int y, int z)
        {
            Chunk c = FindChunk(x, y, z);

            Destroy(c.gameObject);
            
            chunks.Remove(new Vector3(x, y, z));
        }

        public void DeleteChunk(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);
            
            int z = Mathf.FloorToInt(pos.z);

            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;
            
            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk c = FindChunk(x, y, z);
            
            Destroy(c.gameObject);

            chunks.Remove(new Vector3(x, y, z));
        }
        public void Clear()
        {
            Chunk[] chunksToRemove = chunks.Values.ToArray();
            
            for (int i = 0; i < chunksToRemove.Length; i++)
            {
                Chunk value = chunksToRemove[i];
                
                Destroy(value.gameObject.GetComponent<MeshFilter>().sharedMesh);

                Destroy(value.gameObject);
            }

            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }

            chunks.Clear();
        }

        public Block GetBlock(int x, int y, int z)
        {
            Chunk c = FindChunk(x, y, z);

            if (c == null)
            {
                return null;
            }

            Block b = c.GetBlock(x - c.x, y - c.y, z - c.z);

            return b;
        }

        public Block GetBlock(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);

            int y = Mathf.FloorToInt(pos.y);
            
            int z = Mathf.FloorToInt(pos.z);

            Chunk c = FindChunk(x, y, z);

            if (c == null)
            {
                return null;
            }

            Block b = c.GetBlock(x - c.x, y - c.y, z - c.z);

            return b;
        }

        public short GetBlockID(int x, int y, int z)
        {
            Chunk c = FindChunk(x, y, z);

            if (c == null)
            {
                return -1;
            }

            short id = c.GetBlockID(x - c.x, y - c.y, z - c.z);

            return id;
        }

        public short GetBlockID(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);

            int z = Mathf.FloorToInt(pos.z);

            Chunk c = FindChunk(x, y, z);

            if (c == null)
            {
                return -1;
            }

            short id = c.GetBlockID(x - c.x, y - c.y, z - c.z);

            return id;
        }

        public void SetBlockID(int x, int y, int z, short id)
        {
            Chunk c = FindChunk(x, y, z);

            if (c == null)
            {
                if (id == -1)
                {
                    return;
                }

                c = new Chunk(Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize, 
                Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize, 
                Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize, ChunkSize);
                
                AddChunk(c);
            }

            c.SetBlockID(x - c.x, y - c.y, z - c.z, id);
        }

        public void SetBlockID(Vector3 pos, short id)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);

            int z = Mathf.FloorToInt(pos.z);

            Chunk c = FindChunk(x, y, z);

            if (c == null)
            {
                if (id == -1)
                {
                    return;
                }

                c = new Chunk(Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize, 
                Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize, Mathf.FloorToInt((float)z / ChunkSize) * 
                ChunkSize, ChunkSize);
                
                AddChunk(c);
            }

            c.SetBlockID(x - c.x, y - c.y, z - c.z, id);
        }

        public void RemoveBlockAt(int x, int y, int z)
        {
            SetBlockID(x, y, z, -1);
        }

        public void RemoveBlockAt(Vector3 pos)
        {
            SetBlockID(pos, -1);
        }

        public bool ContainsBlock(int x, int y, int z)
        {
            return GetBlock(x, y, z) != null;
        }

        public bool ContainsBlock(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);

            int z = Mathf.FloorToInt(pos.z);

            return GetBlock(x, y, z) != null;
        }

        public bool IsBlockVisible(int x, int y, int z)
        {
            bool top = GetBlock(x, y + 1, z) == null;
            
            bool bottom = GetBlock(x, y - 1, z) == null;
            
            bool left = GetBlock(x - 1, y, z) == null;
            
            bool right = GetBlock(x + 1, y, z) == null;
            
            bool forward = GetBlock(x, y, z + 1) == null;

            bool back = GetBlock(x, y, z - 1) == null;

            return (top || bottom || left || right || forward || back);
        }

        public bool IsBlockVisible(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);
            
            int z = Mathf.FloorToInt(pos.z);

            bool top = GetBlock(x, y + 1, z) == null;
            
            bool bottom = GetBlock(x, y - 1, z) == null;
            
            bool left = GetBlock(x - 1, y, z) == null;
            
            bool right = GetBlock(x + 1, y, z) == null;
            
            bool forward = GetBlock(x, y, z + 1) == null;

            bool back = GetBlock(x, y, z - 1) == null;

            return (top || bottom || left || right || forward || back);
        }

        public bool IsBlockGround(int x, int y, int z)
        {
            bool containsBlock = GetBlock(x, y, z) != null;

            bool top = GetBlock(x, y + 1, z) == null;

            return (containsBlock && top);
        }

        public bool IsBlockGround(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);
            
            int z = Mathf.FloorToInt(pos.z);

            bool containsBlock = GetBlock(x, y, z) != null;

            bool top = GetBlock(x, y + 1, z) == null;

            return (containsBlock && top);
        }

        public bool IsBlockWall(int x, int y, int z)
        {
            bool containsBlock = GetBlock(x, y, z) != null;
            
            bool left = GetBlock(x - 1, y, z) == null;
            
            bool right = GetBlock(x + 1, y, z) == null;
            
            bool forward = GetBlock(x, y, z + 1) == null;

            bool back = GetBlock(x, y, z - 1) == null;

            return (containsBlock && (left || right || forward || back));
        }

        public bool IsBlockWall(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);
            
            int z = Mathf.FloorToInt(pos.z);

            bool containsBlock = GetBlock(x, y, z) != null;
            
            bool left = GetBlock(x - 1, y, z) == null;
            
            bool right = GetBlock(x + 1, y, z) == null;
            
            bool forward = GetBlock(x, y, z + 1) == null;

            bool back = GetBlock(x, y, z - 1) == null;

            return (containsBlock && (left || right || forward || back));
        }

        public bool IsBlockCeiling(int x, int y, int z)
        {
            bool containsBlock = GetBlock(x, y, z) != null;

            bool bottom = GetBlock(x, y - 1, z) == null;

            return (containsBlock && bottom);
        }

        public bool IsBlockCeiling(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            
            int y = Mathf.FloorToInt(pos.y);
            
            int z = Mathf.FloorToInt(pos.z);

            bool containsBlock = GetBlock(x, y, z) != null;

            bool bottom = GetBlock(x, y - 1, z) == null;

            return (containsBlock && bottom);
        }

        public void Fill(Vector3 a, Vector3 b, short id)
        {
            int ax = Mathf.FloorToInt(a.x);

            int ay = Mathf.FloorToInt(a.y);
            
            int az = Mathf.FloorToInt(a.z);

            int bx = Mathf.FloorToInt(b.x);
            
            int by = Mathf.FloorToInt(b.y);
            
            int bz = Mathf.FloorToInt(b.z);

            bool xB = (ax < bx);

            bool yB = (ay < by);
            
            bool zB = (az < bz);

            int xI = (xB ? 1 : -1);

            int yI = (yB ? 1 : -1);
            
            int zI = (zB ? 1 : -1);

            for (int x = ax; (xB ? x < bx : x > bx); x += xI)
            {
                for (int y = ay; (yB ? y < by : y > by); y += yI)
                {
                    for (int z = az; (zB ? z < bz : z > bz); z += zI)
                    {
                        SetBlockID(x, y, z, id);
                    }
                }
            }
        }

        public void Sphere(Vector3 pos, int radius, short id)
        {
            if (radius <= 0)
            {
                Debug.LogError("Attemped creating a sphere but the radius is too small (<= 0)", gameObject);

                return;
            }

            int posx = Mathf.FloorToInt(pos.x);

            int posy = Mathf.FloorToInt(pos.y);
            
            int posz = Mathf.FloorToInt(pos.z);

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (int z = -radius; z <= radius; z++)
                    {
                        if (Vector3.Distance(pos, pos + new Vector3(x, y, z)) < radius)
                        {
                            SetBlockID(posx + x, posy + y, posz + z, id);
                        }
                    }
                }
            }
        }

        public Chunk[] GetChunks(Vector3 pos, float distance)
        {
            List<Chunk> nearChunks = new List<Chunk>();

            Chunk[] values = chunks.Values.ToArray();

            for (int i = 0; i < values.Length; i++)
            {
                Chunk value = values[i];

                if (Vector3.Distance(pos, value.gameObject.transform.position) <= distance)
                {
                    nearChunks.Add(value);
                }
            }

            return nearChunks.ToArray();
        }

        public Chunk[] GetChunks(Vector3 pos, int distance)
        {
            List<Chunk> nearChunks = new List<Chunk>();

            distance *= ChunkSize;

            Chunk[] values = chunks.Values.ToArray();

            for (int i = 0; i < values.Length; i++)
            {
                Chunk value = values[i];
                if (Vector3.Distance(pos, value.gameObject.transform.position) <= distance)
                {
                    nearChunks.Add(value);
                }
            }

            return nearChunks.ToArray();
        }

        public void Refresh()
        {
            if (this.material == null || this.blockDictionary == null || this.blockDictionary.blocksInfo.Length <= 0)
            {
                Debug.LogError("The terrain does not contain a material or a block dictionary to render!", gameObject);

                return;
            }

            Chunk[] dirtyChunks = chunks.Values.ToArray();

            for (int i = 0; i < dirtyChunks.Length; i++)
            {
                dirtyChunks[i].Refresh();
            }
        }

        public void FastRefresh()
        {
            if (this.material == null || this.blockDictionary == null || this.blockDictionary.blocksInfo.Length <= 0)
            {
                Debug.LogError("The terrain does not contain a material or a block dictionary to render!", gameObject);

                return;
            }

            Chunk[] dirtyChunks = chunks.Values.ToArray();

            for (int i = 0; i < dirtyChunks.Length; i++)
            {
                dirtyChunks[i].FastRefresh();
            }
        }

        public void FastBatch()
        {
            if (batchDirty) Batch();
        }

        public void Batch()
        {
            lastBatch_chunkCount = chunks.Count;
            
            Chunk[] nonBatchedChunks = chunks.Values.Where(o => o.needsBatching).ToArray();

            if (nonBatchedChunks.Length > 0)
            {
                StaticBatchingUtility.Combine(nonBatchedChunks.Select(o => o.gameObject).ToArray(), gameObject);

                for (int i = 0; i < nonBatchedChunks.Length; i++)
                {
                    nonBatchedChunks[i].needsBatching = false;
                }
            }
        }

        public static string[] GetWorldList()
        {
            string rootPath = Application.persistentDataPath;
            
            string[] worldPaths = Directory.GetFiles(rootPath, "*.vm");

            for (int i = 0; i < worldPaths.Length; i++)
            {
                worldPaths[i] = Path.GetFileNameWithoutExtension(worldPaths[i]);
            }

            return worldPaths;
        }

        public static bool WorldExists(string name)
        {
            string fileName = Application.persistentDataPath + "\\" + name + ".vm";

            return File.Exists(fileName);
        }

        public static void RemoveWorld(string name)
        {
            string fileName = Application.persistentDataPath + "\\" + name + ".vm";

            File.Delete(fileName);
        }

        public void LoadWorld(string name, bool refresh = true)
        {
            string fileName = Application.persistentDataPath + "\\" + name + ".vm";
            
            if (!File.Exists(fileName))
            {
                Debug.LogError("World does not exist");
                
                return;
            }

            using (BinaryReader bR = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                int ChunkSize = bR.ReadInt32();
                
                float renderDistance = bR.ReadSingle();
                
                float disposeDistance = bR.ReadSingle();
                
                bool ignoreYAxis = bR.ReadBoolean();
                
                float staticBatchingInterval = bR.ReadSingle();
                
                bool enableColliders = bR.ReadBoolean();
                
                int tiling = bR.ReadInt32();
                
                string blockDictionaryName = bR.ReadString();
                
                string materialName = bR.ReadString();

                bool containsCustomGeneration = bR.ReadBoolean();
                
                float generationDistance = 0;
                
                float updateDistance = 0;
                
                int chunksPerFrame = 0;
                
                bool destroyUnusedChunks = false;
                
                string generationScriptName = string.Empty;

                if (containsCustomGeneration)
                {
                    generationDistance = bR.ReadSingle();
                    
                    updateDistance = bR.ReadSingle();
                    
                    chunksPerFrame = bR.ReadInt32();
                    
                    destroyUnusedChunks = bR.ReadBoolean();
                    
                    generationScriptName = bR.ReadString();
                }

                BlockDictionary blockDictionary = null;
                
                BlockDictionary[] blockDictionaries = Resources.FindObjectsOfTypeAll<BlockDictionary>();

                for (int i = 0; i < blockDictionaries.Length; i++)
                {
                    BlockDictionary bd = blockDictionaries[i];

                    if (bd.name == blockDictionaryName)
                    {
                        blockDictionary = bd;

                        break;
                    }
                }

                if (blockDictionary == null)
                {
                    Debug.LogError("Block Dictionary could not be found.");

                    return;
                }

                Material material = null;
                
                Material[] materials = Resources.FindObjectsOfTypeAll<Material>();

                for (int i = 0; i < materials.Length; i++)
                {
                    Material mat = materials[i];

                    if (mat.name == materialName)
                    {
                        material = mat;

                        break;
                    }
                }

                if (material == null)
                {
                    Debug.LogError("Block Dictionary could not be found.");

                    return;
                }

                this.Clear();

                this.ChunkSize = ChunkSize;
                
                this.renderDistance = renderDistance;
                
                this.disposeDistance = disposeDistance;
                
                this.ignoreYAxis = ignoreYAxis;
                
                this.staticBatchingInterval = staticBatchingInterval;
                
                this.enableColliders = enableColliders;
                
                this.tiling = tiling;
                
                this.blockDictionary = blockDictionary;
                
                this.material = material;

                while (bR.BaseStream.Position != bR.BaseStream.Length)
                {
                    int blockCount = bR.ReadInt32();
                    
                    float chunkX = bR.ReadSingle();
                    
                    float chunkY = bR.ReadSingle();
                    
                    float chunkZ = bR.ReadSingle();

                    Chunk c = CreateChunk(new Vector3(chunkX, chunkY, chunkZ));

                    for (int i = 0; i < blockCount; i++)
                    {
                        int x = bR.ReadInt32();

                        int y = bR.ReadInt32();
                        
                        int z = bR.ReadInt32();
                        
                        short id = bR.ReadInt16();
                        
                        c.SetBlockID(x, y, z, id);
                    }
                }

                if (containsCustomGeneration)
                {
                    if (GetComponent<VoxelGeneration>() != null)
                    {
                        Destroy(GetComponent<VoxelGeneration>());
                    }

                    if (GetComponent<BaseGeneration>() != null)
                    {
                        Destroy(GetComponent<BaseGeneration>());
                    }

                    VoxelGeneration voxelGen = gameObject.AddComponent<VoxelGeneration>();
                    
                    voxelGen.generationDistance = generationDistance;
                    
                    voxelGen.updateDistance = updateDistance;
                    
                    voxelGen.chunksPerFrame = chunksPerFrame;
                    
                    voxelGen.destroyUnusedChunks = destroyUnusedChunks;

                    gameObject.AddComponent(System.Type.GetType(generationScriptName));
                }
            }

            if (refresh)
            {
                FastRefresh();
            }
        }

        public void SaveWorld(string name)
        {
            string fileName = Application.persistentDataPath + "\\" + name + ".vm";

            using (BinaryWriter bW = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                bW.Write(this.ChunkSize);

                bW.Write(this.renderDistance);
                
                bW.Write(this.disposeDistance);
                
                bW.Write(this.ignoreYAxis);
                
                bW.Write(this.staticBatchingInterval);
                
                bW.Write(this.enableColliders);
                
                bW.Write(this.tiling);
                
                bW.Write(this.blockDictionary.name);
                
                bW.Write(this.material.name);

                BaseGeneration baseGen = GetComponent<BaseGeneration>();
                
                bool containsCustomGeneration = (baseGen != null);
                
                bW.Write(containsCustomGeneration);

                if (containsCustomGeneration)
                {
                    VoxelGeneration voxelGeneration = GetComponent<VoxelGeneration>();

                    bW.Write(voxelGeneration.generationDistance);
                    
                    bW.Write(voxelGeneration.updateDistance);
                    
                    bW.Write(voxelGeneration.chunksPerFrame);
                    
                    bW.Write(voxelGeneration.destroyUnusedChunks);

                    bW.Write(baseGen.GetType().Name);
                }

                for (int i = 0; i < chunks.Count; i++)
                {
                    Vector3 cPos = chunks.Keys.ElementAt(i);

                    Chunk c = chunks[cPos];

                    if (!c.gameObject.GetComponent<ChunkManager>().dirty)
                    {
                        continue;
                    }

                    int blockCount = c.count;

                    bW.Write(blockCount);
                    
                    bW.Write(cPos.x);
                    
                    bW.Write(cPos.y);
                    
                    bW.Write(cPos.z);

                    for (int x = 0; x < c.size; x++)
                    {
                        for (int y = 0; y < c.size; y++)
                        {
                            for (int z = 0; z < c.size; z++)
                            {
                                short id = c.GetBlockID(x, y, z);

                                if (id == -1)
                                {
                                    continue;
                                }

                                bW.Write(x);

                                bW.Write(y);
                                
                                bW.Write(z);
                                
                                bW.Write(id);
                            }
                        }
                    }
                }

                bW.Flush();

                bW.Close();
            }
        }

        void AddChunk(Chunk c)
        {
            if (chunks.ContainsKey(new Vector3(c.x, c.y, c.z)))
            {
                return;
            }

            c.parent = this;

            chunks.Add(new Vector3(c.x, c.y, c.z), c);
        }

        void RemoveChunk(int x, int y, int z)
        {
            Chunk c = FindChunk(x, y, z);
            
            Destroy(c.gameObject);

            chunks.Remove(new Vector3(x, y, z));
        }

        float timer = 0f;

        void Awake()
        {
            if (staticBatchingInterval == -1)
            {
                return;
            }

            timer = staticBatchingInterval;
        }

        void Update()
        {
            while (MainThread.Count > 0)
            {
                UnityAction action = MainThread.Dequeue();

                if (action != null)
                {
                    action();
                }
            }

            while (ColliderBuffer.Count > 0)
            {
                UnityAction action = ColliderBuffer.Dequeue();

                if (action != null)
                {
                    action();
                }
            }

            if (staticBatchingInterval >= 0)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }

                else
                {
                    FastBatch();

                    timer = staticBatchingInterval;
                }
            }
        }

        void OnValidate()
        {
            disposeDistance = Mathf.Clamp(disposeDistance, renderDistance, Mathf.Infinity);
        }
    }
}