using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VoxelMaster
{
    [RequireComponent(typeof(VoxelTerrain))]
    public class VoxelGeneration : MonoBehaviour
    {
        public delegate short GenerationAction(int x, int y, int z);

        GenerationAction generationAction = null;

        [Header("Render settings")]
        [Tooltip("The camera to render with, set this with the main camera you are using")]
        public new Camera camera;

        [Space(5)]
        [Tooltip("The generation distance in units")]
        public float generationDistance = 128f;

        [Tooltip("Determines how much the camera has to move for the chunks to update generation, usually a ChunkSize's distance threshold is a good value.")]
        public float updateDistance = 16f;

        [Range(1, 64)]
        [Tooltip("The amount of chunks per frame to generate, lower values makes chunk generation slower, but less laggy, while high values makes generation faster, but laggier.")]
        public int chunksPerFrame = 4;

        [Header("Chunk settings")]
        [Tooltip("Destroy chunks that haven't been used.\nIf disabled, chunks are preserved indefinitely, allows faster rendering of already generated unused chunks, but will eat up memory.")]
        public bool destroyUnusedChunks = true;

        VoxelTerrain terrain;
        
        Vector3 curPos = Vector3.zero;

        long chunkFrame = 0;

        void Awake()
        {
            terrain = GetComponent<VoxelTerrain>();

            if (camera != null)
            {
                curPos = camera.transform.position;
            }

            TryAssign();
        }

        void Start()
        {
            if (generationAction != null && camera != null)
            {
                StartCoroutine(UpdateGeneration());
            }
        }

        void Update()
        {
            if (generationAction == null || camera == null)
            {
                TryAssign();

                return;
            }  

            if (Vector3.Distance(curPos, camera.transform.position) >= updateDistance)
            {
                curPos = camera.transform.position;
            }
        }

        void TryAssign()
        {
            if (generationAction == null)
            {
                BaseGeneration baseGen = GetComponent<BaseGeneration>();

                if (baseGen != null)
                {
                    generationAction = baseGen.Generation;
                }
            }

            if (camera == null)
            {
                camera = Camera.main;
            }
        }

        void Reset()
        {
            camera = Camera.main;
        }

        IEnumerator UpdateGeneration()
        {
            int ChunkSize;

            Vector3 lastPos = curPos + Vector3.one * updateDistance;
            
            Vector3[] chunkGrid;

            while (true)
            {
                if (lastPos != curPos)
                {
                    ChunkSize = terrain.ChunkSize;
                    
                    lastPos = curPos;

                    chunkGrid = GetChunkGrid(lastPos, ChunkSize);

                    for (int i = 0; i < chunkGrid.Length; i++)
                    {
                        if (lastPos != curPos)
                        {
                            break;
                        }

                        Vector3 v = chunkGrid[i];

                        bool chunkExists = terrain.ChunkExists(v);

                        if (!chunkExists)
                        {
                            GenerateChunk(v, ChunkSize);

                            chunkFrame++;

                            if (chunkFrame % chunksPerFrame == 0)
                            {
                                yield return null;
                            }
                        }
                    }
                }

                yield return null;
            }
        }

        void GenerateChunk(Vector3 v, int ChunkSize)
        {
            Block[,,] blocks = new Block[ChunkSize, ChunkSize, ChunkSize];
            
            bool isEmpty = true;

            Chunk c = terrain.CreateChunk(v);

            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        short id = generationAction(x + c.x, y + c.y, z + c.z);

                        if (id != -1)
                        {
                            isEmpty = false;

                            blocks[x, y, z] = new Block(c, id);
                        }
                    }
                }
            }

            c.gameObject.GetComponent<ChunkManager>().destroyUnused = destroyUnusedChunks;

            if (!isEmpty)
            {
                c.SetBlocks(blocks);
            }
        }

        public void SetGenerationAction(GenerationAction generationAction)
        {
            this.generationAction = generationAction;

            if (terrain != null)
            {
                terrain.Clear();
            }
        }

        public static float Remap(float value, float iMin, float iMax, float oMin, float oMax)
        {
            return Mathf.Lerp(oMin, oMax, Mathf.InverseLerp(iMin, iMax, value));
        }

        public static float Step(float a, float b)
        {
            return (a >= b ? 1f : 0f);
        }

        Vector3[] GetChunkGrid(Vector3 pos, int chunkSize)
        {
            List<Pair> grid = new List<Pair>();

            int lowX = ChunkRound(pos.x - generationDistance, chunkSize);
            
            int lowY = ChunkRound(pos.y - generationDistance, chunkSize);
            
            int lowZ = ChunkRound(pos.z - generationDistance, chunkSize);

            int highX = ChunkRound(pos.x + generationDistance, chunkSize);
            
            int highY = ChunkRound(pos.y + generationDistance, chunkSize);

            int highZ = ChunkRound(pos.z + generationDistance, chunkSize);

            for (int x = lowX; x <= highX; x += chunkSize)
            {
                for (int y = lowY; y <= highY; y += chunkSize)
                {
                    for (int z = lowZ; z <= highZ; z += chunkSize)
                    {
                        Vector3 v = new Vector3(x, y, z);

                        float distance = Vector3.Distance(v, pos);

                        if (distance <= generationDistance && !terrain.ChunkExists(v))
                        {
                            grid.Add(new Pair(distance, v));
                        }
                    }
                }
            }

            return grid.OrderBy(o => o.distance).Select(o => o.pos).ToArray();
        }
        static int ChunkRound(float v, int ChunkSize)
        {
            return Mathf.FloorToInt(v / ChunkSize) * ChunkSize;
        }

        struct Pair
        {
            public float distance;

            public Vector3 pos;

            public Pair(float distance, Vector3 pos)
            {
                this.distance = distance;

                this.pos = pos;
            }
        }
    }
}