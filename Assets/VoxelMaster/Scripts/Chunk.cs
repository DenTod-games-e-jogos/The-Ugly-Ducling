using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading;

namespace VoxelMaster
{
    [System.Serializable]
    public class Chunk
    {
        private static int curID = 0;

        private VoxelTerrain _parent;
        /// <summary>
        /// The voxel terrain attached to this chunk.
        /// </summary>
        public VoxelTerrain parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
                if (this.gameObject != null)
                {
                    this.gameObject.transform.parent = value.transform;
                    this.gameObject.transform.position = new Vector3(x, y, z);
                }
            }
        }

        /// <summary>
        /// The attached Game Object to this chunk.
        /// </summary>
        public GameObject gameObject { get; private set; }
        /// <summary>
        /// The chunk ID
        /// </summary>
        public int id { get; private set; }
        /// <summary>
        /// The size of the chunk.
        /// </summary>
        public int size { get; private set; }
        /// <summary>
        /// The amount of blocks in this chunk.
        /// </summary>
        public int count
        {
            get
            {
                int blockCount = 0;
                for (int x = 0; x < blocks.GetLength(0); x++)
                {
                    for (int y = 0; y < blocks.GetLength(1); y++)
                    {
                        for (int z = 0; z < blocks.GetLength(2); z++)
                        {
                            if (blocks[x, y, z] != null) blockCount++;
                        }
                    }
                }
                return blockCount;
            }
        }
        /// <summary>
        /// The X coordinate of the chunk.
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// The Y coordinate of the chunk.
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// The Z coordinate of the chunk.
        /// </summary>
        public int z { get; set; }
        /// <summary>
        /// The position of the chunk.
        /// </summary>
        public Vector3 pos
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }

        private bool _visible = false;
        /// <summary>
        /// Determines if the chunk is visible.
        /// </summary>
        public bool visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (value == _visible)
                    return;

                _visible = value;

                renderer.enabled = _visible;

                if (_visible)
                    FastRefresh();
            }
        }

        /// <summary>
        /// Gets if the chunk is dirty or modified since it was last refreshed.
        /// </summary>
        public bool dirty { get; private set; }
        /// <summary>
        /// Determines if the chunk needs batching.
        /// </summary>
        public bool needsBatching { get; set; }

        private MeshRenderer renderer;
        private MeshFilter meshFilter;
        private MeshCollider collider;
        private ChunkManager chunkManager;
        private Mesh m;

        private Block[,,] blocks;

        /// <summary>
        /// Creates a new chunk without its terrain. (Please create chunks using the terrain instead)
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <param name="size">The size of the chunk</param>
        public Chunk(int x, int y, int z, int size)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.size = size;

            id = curID; curID++;
            gameObject = new GameObject("CHUNK_" + id);
            gameObject.isStatic = true;
            dirty = false;

            blocks = new Block[size, size, size];

            m = new Mesh();
            m.Optimize();
            m.MarkDynamic();

            meshFilter = gameObject.AddComponent<MeshFilter>();
            renderer = gameObject.AddComponent<MeshRenderer>();
            chunkManager = gameObject.AddComponent<ChunkManager>();
            chunkManager.parent = this;
            collider = gameObject.AddComponent<MeshCollider>();
        }

        /// <summary>
        /// Clears the whole chunk.
        /// </summary>
        public void Clear()
        {
            for (int x = 0; x < blocks.GetLength(0); x++)
            {
                for (int y = 0; y < blocks.GetLength(1); y++)
                {
                    for (int z = 0; z < blocks.GetLength(2); z++)
                    {
                        blocks[x, y, z] = null;
                    }
                }
            }

            this.chunkManager.dirty = true;
            this.dirty = true;
        }
        /// <summary>
        /// Deletes the chunk.
        /// </summary>
        public void Delete()
        {
            if (parent == null)
            {
                Debug.LogError("This chunk has been created without a terrain, to delete it, you must do it manually", gameObject);
                return;
            }

            if (m != null)
                Object.Destroy(m); // Destroy the mesh to save memory.

            parent.DeleteChunk(new Vector3(x, y, z));
        }

        /// <summary>
        /// Gets the block at the specified position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>The block found at the specified position, otherwise null.</returns>
        public Block GetBlock(int x, int y, int z)
        {
            if (x < 0 || x >= size) // If outside bounds, return null
                return null;
            if (y < 0 || y >= size)
                return null;
            if (z < 0 || z >= size)
                return null;

            return blocks[x, y, z];
        }
        /// <summary>
        /// Gets the block ID at the specified position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <returns>The block ID at the specified position, otherwise -1</returns>
        public short GetBlockID(int x, int y, int z)
        {
            if (x < 0 || x >= size) // If outside bounds, return -1
                return -1;
            if (y < 0 || y >= size)
                return -1;
            if (z < 0 || z >= size)
                return -1;

            Block b = blocks[x, y, z];
            return (b != null ? b.id : (short)-1);
        }
        /// <summary>
        /// Sets the block ID at the specified position.
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <param name="z">The Z coordinate</param>
        /// <param name="id">The block ID</param>
        public void SetBlockID(int x, int y, int z, short id = 0)
        {
            if (x < 0 || x >= size) // If outside bounds, just ignore.
                return;
            if (y < 0 || y >= size)
                return;
            if (z < 0 || z >= size)
                return;

            if (id == -1)
                blocks[x, y, z] = null;
            else
                blocks[x, y, z] = new Block(this, id);

            this.chunkManager.dirty = true;
            this.dirty = true;
        }
        /// <summary>
        /// Set the blocks of this chunk to the specified array.
        /// </summary>
        /// <param name="blocks">The block array</param>
        public void SetBlocks(Block[,,] blocks)
        {
            if (blocks.GetLength(0) != size || blocks.GetLength(1) != size || blocks.GetLength(2) != size)
            {
                Debug.LogError("Chunk SetBlocks is invalid in size, please provide a 3D array of blocks with the same size as the chunk.", gameObject);
                return;
            }

            this.blocks = blocks;
            this.dirty = true;
        }

        /// <summary>
        /// Refreshes this chunk if it is dirty.
        /// </summary>
        public void FastRefresh()
        {
            if (this.dirty && this._visible)
                Refresh();
        }
        /// <summary>.
        /// Refreshes this chunk
        /// </summary>
        public void Refresh()
        {
            if (this.parent.material == null || this.parent.blockDictionary == null || this.parent.blockDictionary.blocksInfo.Length <= 0)
            {
                Debug.LogError("The terrain does not contain a material or a block dictionary to render!", gameObject);
                return;
            }

            renderer.sharedMaterial = this.parent.material;

            //int faceCount = 0;
            //int o = 0;

            // Generate chunk

            List<Vector3> vertices = new List<Vector3>(this.count * 24);
            List<int> triangles = new List<int>(this.count * 36);
            List<Vector2> uv = new List<Vector2>(this.count * 24);

            Thread t = new Thread(() => GenerateMesh(vertices, triangles, uv));
            t.IsBackground = true;
            t.Start();
        }

        private void GenerateMesh(List<Vector3> vertices, List<int> triangles, List<Vector2> uv)
        {
            int faceCount = 0;
            int o = 0;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        // INITIALIZATION STUFF

                        if (!_visible)
                        {
                            this.dirty = true;
                            return; // Abort mesh construction if not visible whilst construction
                        }

                        if (GetBlockID(x, y, z) == -1)
                            continue;

                        BlockInfo blockInfo = GetBlock(x, y, z).blockInfo;

                        o = vertices.Count; // offset
                        Vector3 pos = new Vector3(x, y, z);

                        bool forward = GetBlockID(x, y, z - 1) == -1 || (!blockInfo.transparent && GetBlock(x, y, z - 1).blockInfo.transparent);
                        bool back = GetBlockID(x, y, z + 1) == -1 || (!blockInfo.transparent && GetBlock(x, y, z + 1).blockInfo.transparent);
                        bool left = GetBlockID(x - 1, y, z) == -1 || (!blockInfo.transparent && GetBlock(x - 1, y, z).blockInfo.transparent);
                        bool right = GetBlockID(x + 1, y, z) == -1 || (!blockInfo.transparent && GetBlock(x + 1, y, z).blockInfo.transparent);
                        bool top = GetBlockID(x, y + 1, z) == -1 || (!blockInfo.transparent && GetBlock(x, y + 1, z).blockInfo.transparent);
                        bool bottom = GetBlockID(x, y - 1, z) == -1 || (!blockInfo.transparent && GetBlock(x, y - 1, z).blockInfo.transparent);

                        if (blockInfo == null)
                        {
                            Debug.LogError("Chunk while generating has detected an empty block info, do not forget to assign a block dictionary with an updated array of valid block infos in your VoxelTerrain", gameObject);
                            return;
                        }

                        faceCount = 0;

                        // VERTICES

                        if (forward)
                        {
                            vertices.AddRange(new Vector3[] {
                            //FORWARD FACE
                            new Vector3(0, 1, 0) + pos,
                            new Vector3(1, 1, 0) + pos,
                            new Vector3(1, 0, 0) + pos,
                            new Vector3(0, 0, 0) + pos
                            });
                            uv.AddRange(GetUVs(blockInfo.blockTexture.front));
                            faceCount++;
                        }
                        if (left)
                        {
                            vertices.AddRange(new Vector3[] {
                            //LEFT FACE
                            new Vector3(0, 1, 1) + pos,
                            new Vector3(0, 1, 0) + pos,
                            new Vector3(0, 0, 0) + pos,
                            new Vector3(0, 0, 1) + pos
                            });
                            uv.AddRange(GetUVs(blockInfo.blockTexture.left));
                            faceCount++;
                        }
                        if (back)
                        {
                            vertices.AddRange(new Vector3[] {
                            //BACK FACE
                            new Vector3(1, 1, 1) + pos,
                            new Vector3(0, 1, 1) + pos,
                            new Vector3(0, 0, 1) + pos,
                            new Vector3(1, 0, 1) + pos
                            });
                            uv.AddRange(GetUVs(blockInfo.blockTexture.back));
                            faceCount++;
                        }
                        if (right)
                        {
                            vertices.AddRange(new Vector3[]{
                            //RIGHT FACE
                            new Vector3(1, 1, 0) + pos,
                            new Vector3(1, 1, 1) + pos,
                            new Vector3(1, 0, 1) + pos,
                            new Vector3(1, 0, 0) + pos
                            });
                            uv.AddRange(GetUVs(blockInfo.blockTexture.right));
                            faceCount++;
                        }
                        if (top)
                        {
                            vertices.AddRange(new Vector3[]{
                            //TOP FACE
                            new Vector3(0, 1, 1) + pos,
                            new Vector3(1, 1, 1) + pos,
                            new Vector3(1, 1, 0) + pos,
                            new Vector3(0, 1, 0) + pos
                            });
                            uv.AddRange(GetUVs(blockInfo.blockTexture.top));
                            faceCount++;
                        }
                        if (bottom)
                        {
                            vertices.AddRange(new Vector3[]{
                            //BOTTOM FACE
                            new Vector3(0, 0, 0) + pos,
                            new Vector3(1, 0, 0) + pos,
                            new Vector3(1, 0, 1) + pos,
                            new Vector3(0, 0, 1) + pos
                            });
                            uv.AddRange(GetUVs(blockInfo.blockTexture.bottom));
                            faceCount++;
                        }

                        // TRIANGLES

                        for (int i = 0; i < faceCount; i++)
                        {
                            int o2 = i * 4;
                            triangles.AddRange(new int[]{
                                0+o+o2, 1+o+o2, 2+o+o2,
                                2+o+o2, 3+o+o2, 0+o+o2,
                            });
                        }
                    }
                }
            }

            VoxelTerrain.MainThread.Enqueue(() => FinishRefresh(vertices.ToArray(), triangles.ToArray(), uv.ToArray())); // Queue mesh to be uploaded to Chunk.
        }
        private void FinishRefresh(Vector3[] vertices, int[] triangles, Vector2[] uv)
        {
            if (m == null || meshFilter == null || collider == null) return;

            m.Clear();

            m.vertices = vertices;
            m.triangles = triangles;
            m.uv = uv;

            m.RecalculateNormals();

            meshFilter.sharedMesh = m;

            if (parent.enableColliders)
            {
                VoxelTerrain.ColliderBuffer.Enqueue(() =>
                {
                    collider.sharedMesh = null;
                    collider.sharedMesh = m;
                });
            }

            this.dirty = false;
            this.needsBatching = true;
        }

        private Chunk[] GetNeighborChunks(int x, int y, int z)
        {
            List<Chunk> nearChunks = new List<Chunk>(6);

            Chunk c;
            c = parent.FindChunk(x + 1, y, z);
            if (c != this) { nearChunks.Add(c); }
            c = parent.FindChunk(x - 1, y, z);
            if (c != this) { nearChunks.Add(c); }
            c = parent.FindChunk(x, y + 1, z);
            if (c != this) { nearChunks.Add(c); }
            c = parent.FindChunk(x, y - 1, z);
            if (c != this) { nearChunks.Add(c); }
            c = parent.FindChunk(x, y, z + 1);
            if (c != this) { nearChunks.Add(c); }
            c = parent.FindChunk(x, y, z - 1);
            if (c != this) { nearChunks.Add(c); }

            return nearChunks.ToArray();
        }
        private Vector2[] GetUVs(int id)
        {
            Vector2[] uv = new Vector2[4];
            float tiling = this.parent.tiling;

            int id2 = id + 1;
            float o = 1f / tiling;
            int i = 0;

            for (int y = 0; y < tiling; y++)
            {
                for (int x = 0; x < tiling; x++)
                {
                    i++;

                    if (i == id2)
                    {
                        float padding = parent.UVPadding / tiling; // Adding a little padding to prevent UV bleeding (to fix)
                        uv[0] = new Vector2(x / tiling + padding, 1f - (y / tiling) - padding);
                        uv[1] = new Vector2(x / tiling + o - padding, 1f - (y / tiling) - padding);
                        uv[2] = new Vector2(x / tiling + o - padding, 1f - (y / tiling + o) + padding);
                        uv[3] = new Vector2(x / tiling + padding, 1f - (y / tiling + o) + padding);

                        return uv;
                    }
                }
            }

            uv[0] = Vector2.zero;
            uv[1] = Vector2.zero;
            uv[2] = Vector2.zero;
            uv[3] = Vector2.zero;

            return uv;
        }
    }
}