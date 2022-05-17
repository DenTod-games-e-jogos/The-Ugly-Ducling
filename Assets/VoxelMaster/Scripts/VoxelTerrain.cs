using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace VoxelMaster
{
    public class VoxelTerrain : MonoBehaviour
    {
        // Don't try to manipulate these two as it might result in injury or death of but not limited to: Unity, Voxel Master, the universe, thank you!
        public static readonly Queue<UnityAction> MainThread = new Queue<UnityAction>(); 
        public static readonly Queue<UnityAction> ColliderBuffer = new Queue<UnityAction>();

        /// <summary>
        /// The size of every chunk in units, 16 is a good value.
        /// </summary>
        [Header("Main properties")]
        [Range(2, 16)]
        [Tooltip("The size of the chunk, 16 is a good value.")]
        public int ChunkSize = 16;
        [Space(10)]
        /// <summary>
        /// The render distance of the chunk.
        /// </summary>
        [Tooltip("The render distance of the chunk.")]
        public float renderDistance = 128f;
        /// <summary>
        /// The dispose distance of the chunk.
        /// </summary>
        [Tooltip("The dispose distance of the chunk, if chunks are further than this distance & wasn't modified in any way, it'll destroy it. Higher dispose distances means higher RAM usage.")]
        public float disposeDistance = 256f;
        /// <summary>
        /// Ignores the Y axis for distance checking, if enabled, it can avoid underneath & above chunks from suddenly dissapearing, render feels more "Minecraft-like"
        /// </summary>
        [Tooltip("Ignores the Y axis for distance checking, if enabled, it can avoid underneath & above chunks from suddenly dissapearing, render feels more \"Minecraft-like\"")]
        public bool ignoreYAxis = false;
        /// <summary>
        /// The interval between static batching operations (-1 to disable static batching)
        /// </summary>
        [Tooltip("The interval between static batching operations (-1 to disable static batching)")]
        public float staticBatchingInterval = 1f;
        /// <summary>
        /// Should we enable colliders for the chunk?
        /// </summary>
        [Tooltip("Should we enable colliders for the chunk?")]
        public bool enableColliders = true;
        /// <summary>
        /// The block dictionary to use.
        /// </summary>
        [Header("Block properties")]
        [Tooltip("The block dictionary to use. A block dictionary contains info about the blocks, their id, their textures and a few other informations.")]
        public BlockDictionary blockDictionary;
        /// <summary>
        /// The material to use along with its texture.
        /// </summary>
        [Tooltip("The material to use along with its texture.")]
        public Material material;
        /// <summary>
        /// The tiling of the material, for example, 16 will split the texture into 16x16 parts.
        /// </summary>
        [Tooltip("The tiling of the material, for example, 16 will split the texture into 16x16 parts.")]
        public int tiling = 16;
        /// <summary>
        /// The UV Padding of the textures.
        /// </summary>
        [Tooltip("The UV Padding of the textures, in deferred rendering you should leave this at 0, if you experience black lines at the edges of blocks, increase this value slightly until the problem is fixed.")]
        [Range(0, 0.4999f)]
        public float UVPadding = 0f;

        /// <summary>
        /// The dictionary of the chunks, please do not directly manipulate this dictionary, use the methods instead.
        /// </summary>
        public Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();
        /// <summary>
        /// Determines if the voxel terrain is dirty and needs refreshing.
        /// </summary>
        public bool dirty
        {
            get
            {
                return chunks.Values.Where(o => o.dirty == true).Count() > 0;
            }
        }
        /// <summary>
        /// Determines if the voxel terrain is dirty for batching.
        /// </summary>
        public bool batchDirty
        {
            get
            {
                bool modified = chunks.Count != lastBatch_chunkCount;
                return dirty || modified;
            }
        }

        private int lastBatch_chunkCount = 0;

        /// <summary>
        /// Determines if a chunk is found at the specified position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>True if a chunk is found, otherwise false.</returns>
        public bool ChunkExists(int x, int y, int z)
        {
            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;
            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk c = null; chunks.TryGetValue(new Vector3(x, y, z), out c);
            return c != null;
        }
        /// <summary>
        /// Determines if a chunk is found at the specified position.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>True if a chunk is found, otherwise false.</returns>
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
        /// <summary>
        /// Gets the chunk at the specified position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>A chunk, otherwise null if it isn't found.</returns>
        public Chunk FindChunk(int x, int y, int z)
        {
            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;
            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk c = null; chunks.TryGetValue(new Vector3(x, y, z), out c);
            return c;
        }
        /// <summary>
        /// Gets the chunk at the specified position.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>A chunk, otherwise null if it isn't found.</returns>
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
        /// <summary>
        /// Gets the chunk at the specified position, if it doesn't exist, it creates a new chunk.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>The found chunk, otherwise the newly created chunk.</returns>
        public Chunk FindOrCreateChunk(int x, int y, int z)
        {
            x = Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize;
            y = Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize;
            z = Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize;

            Chunk c = null; chunks.TryGetValue(new Vector3(x, y, z), out c);
            if (c != null)
                return c;

            Chunk newChunk = new Chunk(x, y, z, ChunkSize);
            AddChunk(newChunk);
            return newChunk;
        }
        /// <summary>
        /// Gets the chunk at the specified position, if it doesn't exist, it creates a new chunk.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>The found chunk, otherwise the newly created chunk.</returns>
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
                return c;

            Chunk newChunk = new Chunk(x, y, z, ChunkSize);
            AddChunk(newChunk);
            return newChunk;
        }
        /// <summary>
        /// Creates a new chunk at this position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>The newly created chunk.</returns>
        public Chunk CreateChunk(int x, int y, int z)
        {
            Chunk newChunk = new Chunk(x, y, z, ChunkSize);
            AddChunk(newChunk);
            return newChunk;
        }
        /// <summary>
        /// Creates a new chunk at this position.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>The newly created chunk</returns>
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
        /// <summary>
        /// Deletes a chunk at the specified position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        public void DeleteChunk(int x, int y, int z)
        {
            Chunk c = FindChunk(x, y, z);
            Destroy(c.gameObject);
            chunks.Remove(new Vector3(x, y, z));
        }
        /// <summary>
        /// Deletes a chunk at the specified position.
        /// </summary>
        /// <param name="pos">The coordinates</param>
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

        /// <summary>
        /// Clears the entire terrain.
        /// </summary>
        public void Clear()
        {
            Chunk[] chunksToRemove = chunks.Values.ToArray();
            for (int i = 0; i < chunksToRemove.Length; i++)
            {
                Chunk value = chunksToRemove[i];
                Destroy(value.gameObject.GetComponent<MeshFilter>().sharedMesh);
                Destroy(value.gameObject);
            }

            foreach (Transform t in transform) // Assure to remove every single chunk, even non registered ones.
                Destroy(t.gameObject);

            chunks.Clear();
        }
        /// <summary>
        /// Gets the block at the specified position
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>The block found at the specified position, otherwise null.</returns>
        public Block GetBlock(int x, int y, int z)
        {
            Chunk c = FindChunk(x, y, z);

            if (c == null)
                return null;

            Block b = c.GetBlock(x - c.x, y - c.y, z - c.z);
            return b;
        }
        /// <summary>
        /// Gets the block at the specified position
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>The block found at the specified position, otherwise null.</returns>
        public Block GetBlock(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            Chunk c = FindChunk(x, y, z);

            if (c == null)
                return null;

            Block b = c.GetBlock(x - c.x, y - c.y, z - c.z);
            return b;
        }
        /// <summary>
        /// Gets the block ID at the specified position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>The block ID found at the specified position, otherwise -1</returns>
        public short GetBlockID(int x, int y, int z)
        {
            Chunk c = FindChunk(x, y, z);

            if (c == null)
                return -1;

            short id = c.GetBlockID(x - c.x, y - c.y, z - c.z);
            return id;
        }
        /// <summary>
        /// Gets the block ID at the specified position.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>The block ID found at the specified position, otherwise -1</returns>
        public short GetBlockID(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            Chunk c = FindChunk(x, y, z);

            if (c == null)
                return -1;

            short id = c.GetBlockID(x - c.x, y - c.y, z - c.z);
            return id;
        }
        /// <summary>
        /// Sets the block ID at the specified position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <param name="id">The block ID</param>
        public void SetBlockID(int x, int y, int z, short id)
        {
            // Find the chunk, if it exists, add, else, create then add

            Chunk c = FindChunk(x, y, z);

            if (c == null)
            {
                if (id == -1)
                    return;

                c = new Chunk(Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize, Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize, Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize, ChunkSize);
                AddChunk(c);
            }

            c.SetBlockID(x - c.x, y - c.y, z - c.z, id);
        }
        /// <summary>
        /// Sets the block ID at the specified position.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <param name="id">The block ID</param>
        public void SetBlockID(Vector3 pos, short id)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            // Find the chunk, if it exists, add, else, create then add

            Chunk c = FindChunk(x, y, z);

            if (c == null)
            {
                if (id == -1)
                    return;

                c = new Chunk(Mathf.FloorToInt((float)x / ChunkSize) * ChunkSize, Mathf.FloorToInt((float)y / ChunkSize) * ChunkSize, Mathf.FloorToInt((float)z / ChunkSize) * ChunkSize, ChunkSize);
                AddChunk(c);
            }

            c.SetBlockID(x - c.x, y - c.y, z - c.z, id);
        }
        /// <summary>
        /// Removes the block at the specified position. Hint: It's a shortcut of doing "SetBlockID(x, y, z, -1);"
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        public void RemoveBlockAt(int x, int y, int z)
        {
            SetBlockID(x, y, z, -1);
        }
        /// <summary>
        /// Removes the block at the specified position. Hint: It's a shortcut of doing "SetBlockID(pos, -1);"
        /// </summary>
        /// <param name="pos">The coordinates</param>
        public void RemoveBlockAt(Vector3 pos)
        {
            SetBlockID(pos, -1);
        }

        /// <summary>
        /// Determines if a block exists at the specified position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>True if a block exists at the specified position, otherwise false.</returns>
        public bool ContainsBlock(int x, int y, int z)
        {
            return GetBlock(x, y, z) != null;
        }
        /// <summary>
        /// Determines if a block exists at the specified position.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>True if a block exists at the specified position, otherwise false.</returns>
        public bool ContainsBlock(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            return GetBlock(x, y, z) != null;
        }
        /// <summary>
        /// Determines if the block is visible at the specified position (Does not check if block exists)
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>True if the block can be seen, otherwise false.</returns>
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
        /// <summary>
        /// Determines if the block is visible at the specified position (Does not check if block exists)
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>True if the block can be seen, otherwise false.</returns>
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
        /// <summary>
        /// Determines if the block is the ground and therefore walkable at the specified position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>True if the block is the ground & walkable, otherwise false.</returns>
        public bool IsBlockGround(int x, int y, int z)
        {
            bool containsBlock = GetBlock(x, y, z) != null;
            bool top = GetBlock(x, y + 1, z) == null;

            return (containsBlock && top);
        }
        /// <summary>
        /// Determines if the block is the ground and therefore walkable at the specified position.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>True if the block is the ground & walkable, otherwise false.</returns>
        public bool IsBlockGround(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            bool containsBlock = GetBlock(x, y, z) != null;
            bool top = GetBlock(x, y + 1, z) == null;

            return (containsBlock && top);
        }
        /// <summary>
        /// Determines if the block can be used as a wall / is a wall.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>True if the block can be used as a wall / is a wall, otherwise false.</returns>
        public bool IsBlockWall(int x, int y, int z)
        {
            bool containsBlock = GetBlock(x, y, z) != null;
            bool left = GetBlock(x - 1, y, z) == null;
            bool right = GetBlock(x + 1, y, z) == null;
            bool forward = GetBlock(x, y, z + 1) == null;
            bool back = GetBlock(x, y, z - 1) == null;

            return (containsBlock && (left || right || forward || back));
        }
        /// <summary>
        /// Determines if the block can be used as a wall / is a wall.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>True if the block can be used as a wall / is a wall, otherwise false.</returns>
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
        /// <summary>
        /// Determines if the block can be used as ceiling / is a ceiling.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>True if the block can be used as ceiling / is a ceiling.</returns>
        public bool IsBlockCeiling(int x, int y, int z)
        {
            bool containsBlock = GetBlock(x, y, z) != null;
            bool bottom = GetBlock(x, y - 1, z) == null;

            return (containsBlock && bottom);
        }
        /// <summary>
        /// Determines if the block can be used as ceiling / is a ceiling.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <returns>True if the block can be used as ceiling / is a ceiling.</returns>
        public bool IsBlockCeiling(Vector3 pos)
        {
            int x = Mathf.FloorToInt(pos.x);
            int y = Mathf.FloorToInt(pos.y);
            int z = Mathf.FloorToInt(pos.z);

            bool containsBlock = GetBlock(x, y, z) != null;
            bool bottom = GetBlock(x, y - 1, z) == null;

            return (containsBlock && bottom);
        }

        /// <summary>
        /// Fills all blocks between position "a" and "b" with the specified block ID.
        /// </summary>
        /// <param name="a">First position</param>
        /// <param name="b">Second position</param>
        /// <param name="id">The block ID (-1 to remove)</param>
        public void Fill(Vector3 a, Vector3 b, short id)
        {
            int ax = Mathf.FloorToInt(a.x);
            int ay = Mathf.FloorToInt(a.y);
            int az = Mathf.FloorToInt(a.z);

            int bx = Mathf.FloorToInt(b.x);
            int by = Mathf.FloorToInt(b.y);
            int bz = Mathf.FloorToInt(b.z);

            bool xB = (ax < bx); // Micro-optimization, determine if we should go forward or backwards in advance.
            bool yB = (ay < by);
            bool zB = (az < bz);

            int xI = (xB ? 1 : -1); // Increments
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
        /// <summary>
        /// Fills a sphere of blocks at the specified position & radius.
        /// </summary>
        /// <param name="pos">The position</param>
        /// <param name="radius">The radius</param>
        /// <param name="id">The block ID to fill the sphere with, -1 would remove blocks</param>
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

        /// <summary>
        /// Gets the chunks within a certain range at the specified position.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <param name="distance">The range in units</param>
        /// <returns>An array of found chunks within the range at the specified position.</returns>
        public Chunk[] GetChunks(Vector3 pos, float distance)
        {
            List<Chunk> nearChunks = new List<Chunk>();

            Chunk[] values = chunks.Values.ToArray();
            for (int i = 0; i < values.Length; i++)
            {
                Chunk value = values[i];
                if (Vector3.Distance(pos, value.gameObject.transform.position) <= distance)
                    nearChunks.Add(value);
            }

            return nearChunks.ToArray();
        }
        /// <summary>
        /// Gets the chunks within a certain range at the specified position.
        /// </summary>
        /// <param name="pos">The coordinates</param>
        /// <param name="distance">The range in chunks</param>
        /// <returns>An array of found chunks within the range at the specified position.</returns>
        public Chunk[] GetChunks(Vector3 pos, int distance)
        {
            List<Chunk> nearChunks = new List<Chunk>();

            distance *= ChunkSize;

            Chunk[] values = chunks.Values.ToArray();
            for (int i = 0; i < values.Length; i++)
            {
                Chunk value = values[i];
                if (Vector3.Distance(pos, value.gameObject.transform.position) <= distance)
                    nearChunks.Add(value);
            }

            return nearChunks.ToArray();
        }

        /// <summary>
        /// Refreshes the whole terrain, to be used ONLY when you want to really force it, it is an expensive operation.
        /// </summary>
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
        /// <summary>
        /// Refreshes only the chunks that are considered "dirty" (Modified)
        /// </summary>
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

        /// <summary>
        /// Prepares all chunks for static batching, somewhat expensive operation, this method executes Batch() but only if the terrain is batch dirty.
        /// </summary>
        public void FastBatch()
        {
            if (batchDirty) Batch();
        }
        /// <summary>
        /// Prepares all chunks for static batching, somewhat expensive operation, use when you're done refreshing or on a regular basis.
        /// </summary>
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

        // LOADING/SAVING SYSTEM
        /// <summary>
        /// Gets a list of worlds that can be loaded.
        /// </summary>
        /// <returns>A list of worlds.</returns>
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
        /// <summary>
        /// Determines if a world with this name exists and can be loaded.
        /// </summary>
        /// <param name="name">The name of the world</param>
        /// <returns>True if the world exists and can be loaded, otherwise false.</returns>
        public static bool WorldExists(string name)
        {
            string fileName = Application.persistentDataPath + "\\" + name + ".vm";
            return File.Exists(fileName);
        }
        /// <summary>
        /// Removes a world from the hard drive.
        /// </summary>
        /// <param name="name">The name of the world</param>
        public static void RemoveWorld(string name)
        {
            string fileName = Application.persistentDataPath + "\\" + name + ".vm";
            File.Delete(fileName);
        }
        /// <summary>
        /// Loads a world and instantiates it into the scene (This method is blocking so its best used within the loading/saving sections of your game)
        /// </summary>
        /// <param name="name">The name of the world</param>
        /// <param name="refresh">Should we refresh the terrain when finished loading?</param>
        /// <returns>A VoxelTerrain containing the loaded world.</returns>
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
                        Destroy(GetComponent<VoxelGeneration>());

                    if (GetComponent<BaseGeneration>() != null)
                        Destroy(GetComponent<BaseGeneration>());

                    VoxelGeneration voxelGen = gameObject.AddComponent<VoxelGeneration>();
                    voxelGen.generationDistance = generationDistance;
                    voxelGen.updateDistance = updateDistance;
                    voxelGen.chunksPerFrame = chunksPerFrame;
                    voxelGen.destroyUnusedChunks = destroyUnusedChunks;

                    gameObject.AddComponent(System.Type.GetType(generationScriptName));
                }
            }

            if (refresh)
                FastRefresh();
        }
        /// <summary>
        /// Saves the world into a file (Final path = Application.persistentDataPath + "\\" + name + ".vm")
        /// </summary>
        /// <param name="name">The name of the world</param>
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
                        continue;

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
                                    continue;

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

        private void AddChunk(Chunk c)
        {
            if (chunks.ContainsKey(new Vector3(c.x, c.y, c.z)))
                return;

            c.parent = this;
            chunks.Add(new Vector3(c.x, c.y, c.z), c);
        }
        private void RemoveChunk(int x, int y, int z)
        {
            Chunk c = FindChunk(x, y, z);
            Destroy(c.gameObject);
            chunks.Remove(new Vector3(x, y, z));
        }

        float timer = 0f;

        // VOXEL TERRAIN BEHAVIOUR

        void Awake()
        {
            if (staticBatchingInterval == -1) return;
            timer = staticBatchingInterval;
        }

        void Update() // This is where threaded mesh generation & collider generation gets finalized, please don't touch it.
        {
            while (MainThread.Count > 0)
            {
                UnityAction action = MainThread.Dequeue();
                if (action != null) action();
            }

            while (ColliderBuffer.Count > 0)
            {
                UnityAction action = ColliderBuffer.Dequeue();
                if (action != null) action();
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