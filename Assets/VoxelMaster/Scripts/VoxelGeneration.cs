using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VoxelMaster
{
    [RequireComponent(typeof(VoxelTerrain))]
    public class VoxelGeneration : MonoBehaviour
    {
        public delegate short GenerationAction(int x, int y, int z); // This delegate will help you to make procedural generation for your terrain.

        private GenerationAction generationAction = null;

        /// <summary>
        /// The camera used for the generation, helps determining where we should generate chunks and so on. Set with MainCamera by default.
        /// </summary>
        [Header("Render settings")]
        [Tooltip("The camera to render with, set this with the main camera you are using")]
        public new Camera camera;
        /// <summary>
        /// The generation distance of the generation, chunks virtually farther than this distance won't get generated.
        /// </summary>
        [Space(5)]
        [Tooltip("The generation distance in units")]
        public float generationDistance = 128f;
        /// <summary>
        /// Determines how much in units the camera has to move for the chunks to update their generation, usually a ChunkSize's distance threshold is a good value.
        /// </summary>
        [Tooltip("Determines how much the camera has to move for the chunks to update generation, usually a ChunkSize's distance threshold is a good value.")]
        public float updateDistance = 16f;
        /// <summary>
        /// The amount of chunks per frame to generate, lower values makes chunk generation slower, but less laggy, while high values makes generation faster, but laggier.
        /// </summary>
        [Range(1, 64)]
        [Tooltip("The amount of chunks per frame to generate, lower values makes chunk generation slower, but less laggy, while high values makes generation faster, but laggier.")]
        public int chunksPerFrame = 4;
        /// <summary>
        /// Determines if chunks further than dispose distance & that weren't modified in any way should be destroyed in order to save memory.
        /// </summary>
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
                curPos = camera.transform.position;

            TryAssign();
        }

        void Start()
        {
            if (generationAction != null && camera != null)
                StartCoroutine(UpdateGeneration());
        }

        void Update()
        {
            if (generationAction == null || camera == null) // Don't render if generation algorithm is absent or that camera doesn't exist.
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
                    generationAction = baseGen.Generation;
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

        IEnumerator UpdateGeneration() // Takes cares of generating chunks, rendering them, etc...
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
                        if (lastPos != curPos) break;

                        Vector3 v = chunkGrid[i];
                        bool chunkExists = terrain.ChunkExists(v);

                        if (!chunkExists)
                        {
                            GenerateChunk(v, ChunkSize);
                            chunkFrame++;

                            if (chunkFrame % chunksPerFrame == 0)
                                yield return null; // Important part that minimizes lag.
                        }
                    }
                }

                yield return null;
            }
        }

        void GenerateChunk(Vector3 v, int ChunkSize)
        {
            Block[,,] blocks = new Block[ChunkSize, ChunkSize, ChunkSize];
            bool isEmpty = true; // Important variable to determine if the whole chunk we are generating is empty, if it is, don't bother filling & refreshing the chunk.
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
                c.SetBlocks(blocks);
        }

        /// <summary>
        /// Sets the procedural generation action to generate the terrain with.
        /// </summary>
        /// <param name="generationAction">A function that will take care of the generation per coordinate, Format: "short generationAction(x, y, z), see GenerationExample.cs"</param>
        public void SetGenerationAction(GenerationAction generationAction)
        {
            this.generationAction = generationAction;
            if (terrain != null) terrain.Clear();
        }

        /// <summary>
        /// Remaps a value from a range to another.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="iMin">The input range minimum</param>
        /// <param name="iMax">The input range maximum</param>
        /// <param name="oMin">The output range minimum</param>
        /// <param name="oMax">The output range maximum</param>
        /// <returns>The remapped value.</returns>
        public static float Remap(float value, float iMin, float iMax, float oMin, float oMax) // A remap function to help you in your procedural generation.
        {
            return Mathf.Lerp(oMin, oMax, Mathf.InverseLerp(iMin, iMax, value));
        }
        /// <summary>
        /// Steps A >= B
        /// </summary>
        /// <param name="a">The first value</param>
        /// <param name="b">The second value</param>
        /// <returns>1 if A >= B, else 0</returns>
        public static float Step(float a, float b) // A step function to help you in your procedural generation.
        {
            return (a >= b ? 1f : 0f);
        }

        private Vector3[] GetChunkGrid(Vector3 pos, int chunkSize)
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
        private static int ChunkRound(float v, int ChunkSize)
        {
            return Mathf.FloorToInt(v / ChunkSize) * ChunkSize;
        }

        private struct Pair
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