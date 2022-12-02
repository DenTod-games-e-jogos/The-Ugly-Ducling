using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace VoxelMaster
{
    [System.Serializable]
    public class Chunk
    {
        static int curID = 0;

        VoxelTerrain _parent;

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

        public GameObject gameObject { get; private set; }

        public int id { get; private set; }

        public int size { get; private set; }

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
                            if (blocks[x, y, z] != null)
                            {
                                blockCount++;
                            }
                        }
                    }
                }

                return blockCount;
            }
        }

        public int x { get; set; }

        public int y { get; set; }

        public int z { get; set; }

        public Vector3 pos
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }

        bool _visible = false;

        public bool visible
        {
            get
            {
                return _visible;
            }

            set
            {
                if (value == _visible)
                {
                    return;
                }

                _visible = value;

                renderer.enabled = _visible;

                if (_visible)
                {
                    FastRefresh();
                }
            }
        }

        public bool dirty { get; private set; }

        public bool needsBatching { get; set; }

        MeshRenderer renderer;
        
        MeshFilter meshFilter;
        
        MeshCollider collider;
        
        ChunkManager chunkManager;
        
        Mesh m;

        Block[,,] blocks;

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

        public void Delete()
        {
            if (parent == null)
            {
                Debug.LogError("This chunk has been created without a terrain, to delete it, you must do it manually", 
                gameObject);
                
                return;
            }

            if (m != null)
            {
                Object.Destroy(m);
            }

            parent.DeleteChunk(new Vector3(x, y, z));
        }

        public Block GetBlock(int x, int y, int z)
        {
            if (x < 0 || x >= size)
            {
                return null;
            }

            if (y < 0 || y >= size)
            {
                return null;
            }

            if (z < 0 || z >= size)
            {
                return null;
            }

            return blocks[x, y, z];
        }

        public short GetBlockID(int x, int y, int z)
        {
            if (x < 0 || x >= size)
            {
                return -1;
            }

            if (y < 0 || y >= size)
            {
                return -1;
            }

            if (z < 0 || z >= size)
            {
                return -1;
            }

            Block b = blocks[x, y, z];

            return (b != null ? b.id : (short)-1);
        }

        public void SetBlockID(int x, int y, int z, short id = 0)
        {
            if (x < 0 || x >= size)
            {
                return;
            }

            if (y < 0 || y >= size)
            {
                return;
            }

            if (z < 0 || z >= size)
            {
                return;
            }

            if (id == -1)
            {
                blocks[x, y, z] = null;
            }

            else
            {
                blocks[x, y, z] = new Block(this, id);
            }

            this.chunkManager.dirty = true;
            
            this.dirty = true;
        }

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

        public void FastRefresh()
        {
            if (this.dirty && this._visible)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            if (this.parent.material == null || this.parent.blockDictionary == null 
            || this.parent.blockDictionary.blocksInfo.Length <= 0)
            {
                Debug.LogError("The terrain does not contain a material or a block dictionary to render!", gameObject);
                
                return;
            }

            renderer.sharedMaterial = this.parent.material;

            List<Vector3> vertices = new List<Vector3>(this.count * 24);
            
            List<int> triangles = new List<int>(this.count * 36);
            
            List<Vector2> uv = new List<Vector2>(this.count * 24);

            Thread t = new Thread(() => GenerateMesh(vertices, triangles, uv));

            t.IsBackground = true;
            
            t.Start();
        }

        void GenerateMesh(List<Vector3> vertices, List<int> triangles, List<Vector2> uv)
        {
            int faceCount = 0;

            int o = 0;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int z = 0; z < size; z++)
                    {
                        if (!_visible)
                        {
                            this.dirty = true;
                            
                            return;
                        }

                        if (GetBlockID(x, y, z) == -1)
                        {
                            continue;
                        }

                        BlockInfo blockInfo = GetBlock(x, y, z).blockInfo;

                        o = vertices.Count;

                        Vector3 pos = new Vector3(x, y, z);

                        bool forward = GetBlockID(x, y, z - 1) == -1 || (!blockInfo.transparent && 
                        GetBlock(x, y, z - 1).blockInfo.transparent);
                        
                        bool back = GetBlockID(x, y, z + 1) == -1 || (!blockInfo.transparent && 
                        GetBlock(x, y, z + 1).blockInfo.transparent);
                        
                        bool left = GetBlockID(x - 1, y, z) == -1 || (!blockInfo.transparent && 
                        GetBlock(x - 1, y, z).blockInfo.transparent);
                        
                        bool right = GetBlockID(x + 1, y, z) == -1 || (!blockInfo.transparent && 
                        GetBlock(x + 1, y, z).blockInfo.transparent);
                        
                        bool top = GetBlockID(x, y + 1, z) == -1 || (!blockInfo.transparent && 
                        GetBlock(x, y + 1, z).blockInfo.transparent);
                        
                        bool bottom = GetBlockID(x, y - 1, z) == -1 || (!blockInfo.transparent && 
                        GetBlock(x, y - 1, z).blockInfo.transparent);

                        if (blockInfo == null)
                        {
                            Debug.LogError("Chunk while generating has detected an empty block info, do not forget to assign a block dictionary with an updated array of valid block infos in your VoxelTerrain", gameObject);
                            
                            return;
                        }

                        faceCount = 0;

                        if (forward)
                        {
                            vertices.AddRange(new Vector3[] 
                            {
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
                            vertices.AddRange(new Vector3[] 
                            {
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
                            vertices.AddRange(new Vector3[] 
                            {
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
                            vertices.AddRange(new Vector3[]
                            {
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
                            vertices.AddRange(new Vector3[]
                            {
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
                            vertices.AddRange(new Vector3[]
                            {
                                new Vector3(0, 0, 0) + pos,
                                new Vector3(1, 0, 0) + pos,
                                new Vector3(1, 0, 1) + pos,
                                new Vector3(0, 0, 1) + pos
                            });

                            uv.AddRange(GetUVs(blockInfo.blockTexture.bottom));
                            
                            faceCount++;
                        }

                        for (int i = 0; i < faceCount; i++)
                        {
                            int o2 = i * 4;

                            triangles.AddRange(new int[]
                            {
                                0+o+o2, 1+o+o2, 2+o+o2,
                                2+o+o2, 3+o+o2, 0+o+o2,
                            });
                        }
                    }
                }
            }

            VoxelTerrain.MainThread.Enqueue(() => FinishRefresh(vertices.ToArray(), triangles.ToArray(), uv.ToArray()));
        }
        void FinishRefresh(Vector3[] vertices, int[] triangles, Vector2[] uv)
        {
            if (m == null || meshFilter == null || collider == null)
            {
                return;
            }

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

        Chunk[] GetNeighborChunks(int x, int y, int z)
        {
            List<Chunk> nearChunks = new List<Chunk>(6);

            Chunk c;
            
            c = parent.FindChunk(x + 1, y, z);
            
            if (c != this) 
            { 
                nearChunks.Add(c); 
            }
            
            c = parent.FindChunk(x - 1, y, z);
            
            if (c != this) 
            { 
                nearChunks.Add(c); 
            }
            
            c = parent.FindChunk(x, y + 1, z);
            
            if (c != this) 
            { 
                nearChunks.Add(c); 
            }
            
            c = parent.FindChunk(x, y - 1, z);
            
            if (c != this) 
            { 
                nearChunks.Add(c); 
            }
            
            c = parent.FindChunk(x, y, z + 1);
            
            if (c != this) 
            { 
                nearChunks.Add(c); 
            }
            
            c = parent.FindChunk(x, y, z - 1);

            if (c != this) 
            { 
                nearChunks.Add(c); 
            }

            return nearChunks.ToArray();
        }
        Vector2[] GetUVs(int id)
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
                        float padding = parent.UVPadding / tiling;

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