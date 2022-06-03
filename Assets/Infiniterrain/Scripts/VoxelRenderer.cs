using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class VoxelRenderer : MonoBehaviour
{
    /*
     * The materials we wish to use in our meshes
     */
    public Material material0;
    public Material material1;
    public Material material2;
    public Material material3;
    public Material material4;

    /*
     * public enum for setting the state of each of our blocks
     */ 
    public enum BlockState { occupied, empty };

    /*
     * array to hold our mesh vertices
     */
    private Vector3[] verts;

    /*
     * arrays to hold our triangle data
     * NOTE: we need 1 array PER submesh
     * and we need 1 submesh for each material we wish to display
     * thus the number of triangle arrays must each the number of materials used
     */
    private int[] triangles0;
    private int[] triangles1;
    private int[] triangles2;
    private int[] triangles3;
    private int[] triangles4;

    /*
     * arrays to hold our normal and unit vectors
     */
    private Vector3[] normals;
    private Vector2[] uvs;

    /*
     * Our terrain is of terrainSize*terrainSize
     */
    private int terrainSize;

    /*
     * the size of 1 mesh (larger terrains are made up of multiple meshes)
     * NOTE: a single mesh can contain 65,000 maximum vertices.  
     * Thus we set a chunk size of 32x32x32 to stay safely below the vertex limit
     */
    private static int chunkSize = 32;

    /*
     * 3d array to hold our block data
     */
    private Block[,,] blocks;

    /*
     * array to hold our terrain height data
     * in a more complex setup this would likely be an array of structs 
     * instead of an array of floats to hold additional data
     */
    private float[,] terrain;

    /*
     * 3d array to hold all of our map chunks
     * each map chunk is an individual mesh
     * each chunk is 32x32x32 blocks in size
     */
    private GameObject[,,] chunks;

    /*
     * reference to our global mesh object used when generating mesh chunks
     */
    private Mesh mesh;

    /*
     * vars for splitting our heightmap into multiple levels 
     * for use with submshes.
     */
    private float minHeight;
    private float maxHeight;

    /*
     * height of our world in chunks
     */
    private int heightInChunks;

    /*
     * render distance as measure in size of chunks
     */
    public int renderDistance = 8;

    void Start()
    {
        /*
         * Use when we are loading map data from the terrain generation script
         */
        terrain = TerrainGenerator.getTerrain();
        terrainSize = (int)Mathf.Sqrt(terrain.Length);

        /*
         * use instead when we are loading map data from disk
         */
        //loadFromDisk("map.dat");

        /*
         * get min/max heights from our terrain array
         */
        findTerrainMinMax();

        heightInChunks = (int)maxHeight / chunkSize + 1;

        /* 
         * setup the size of our block array
         */
        blocks = new Block[terrainSize, heightInChunks*chunkSize, terrainSize];

        /*
         * populate our block array with the state for each block
         */
        setBlockStates(terrainSize, heightInChunks*chunkSize, terrainSize);

        /*
         * setup our map chunk array
         */
        chunks = new GameObject[terrainSize / chunkSize, heightInChunks, terrainSize / chunkSize];
    }

    /*
     * generate map chunks near the given vector
     */
    public void genChunks(Vector3 pos)
    {
        // visible radius (measured in mesh chunk sizes)
        int radius = renderDistance * chunkSize;

        // chunk size
        int cSize = terrainSize / chunkSize;

        /*
         * loop through all the map chunks
         * disable ones outside our viewing radius
         * enable and/or create chunks within our viewing radius
         */
        for (int cx = 0; cx < cSize; cx++)
        {
            for (int cy = 0; cy < heightInChunks; cy++)
            {
                for (int cz = 0; cz < cSize; cz++)
                {
                    Vector3 chunkPrime = new Vector3(cx * chunkSize + chunkSize / 2, cy * chunkSize + chunkSize / 2, cz * chunkSize + chunkSize / 2);

                    if (Vector3.Distance(pos, chunkPrime) < radius)
                    {
                        // create a new mesh chunk
                        if (chunks[cx, cy, cz] == null)
                            createMesh(cx, cy, cz);

                        // show the chunk
                        chunks[cx, cy, cz].SetActive(true);
                    }
                    else
                    {
                        // hide the chunk
                        if (chunks[cx, cy, cz] != null)
                            chunks[cx, cy, cz].SetActive(false);
                    }
                }
            }
        }
    }

    /*
     * creates a new game object and mesh
     */
    private void createMesh(int cx, int cy, int cz)
    {
        // create an empty game object
        GameObject go = new GameObject();

        // assign it to the chunk array
        chunks[cx, cy, cz] = go;

        // name it so we can find it easily in the hierarchy
        go.name = "Mesh: " + cx + ", " + cy + ", " + cz;

        // add a tag for easy reference later
        //go.tag = "Mesh";

        // add needed componenents  (mesh filter, mesh renderer, and mesh collider)
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshCollider>();

        // create a new mesh
        mesh = new Mesh();

        // generate the mesh data
        fillMesh(cx, cy, cz, chunks[cx, cy, cz], chunks[cx, cy, cz].GetComponent<MeshCollider>(), mesh);
    }

    /*
     * Divide each terrain mesh into 5 submeshes
     * based on elevation
     */
    private void determineSubMesh(int offset, float y)
    {
        float delta = (maxHeight - minHeight) / 5;

        if (y < delta + minHeight)
            assignTris(triangles0, offset);
        else if (y < delta * 2 + minHeight)
            assignTris(triangles1, offset);
        else if (y < delta * 3 + minHeight)
            assignTris(triangles2, offset);
        else if (y < delta * 4 + minHeight)
            assignTris(triangles3, offset);
        else
            assignTris(triangles4, offset);
    }

    /*
     * generates the mesh data and assigns it to game object
     */
    private void fillMesh(int cx, int cy, int cz, GameObject go, MeshCollider collider, Mesh mesh)
    {
        // calculate the number of textured quads we need to render in the current chunk
        int quads = quadCount(cx, cy, cz);

        // clear the mesh arrays
        clearArrays();

        // create new arrays
        createArrays(quads);

        // keep track of our current location when assigning data to the arrays
        //int offset = 0;

        /*
         * fill the mesh arrays
         */
        setupQuads(cx, cy, cz);

        /*
         * always clear the mesh before assigning new data
         */
        mesh.Clear();

        // number of materials (submeshes) in this mesh
        mesh.subMeshCount = 5;

        // always assign the vertices first
        mesh.vertices = verts;

        // assign the triangles
        mesh.SetTriangles(triangles0, 0);
        mesh.SetTriangles(triangles1, 1);
        mesh.SetTriangles(triangles2, 2);
        mesh.SetTriangles(triangles3, 3);
        mesh.SetTriangles(triangles4, 4);

        // set the normals and UVs
        mesh.normals = normals;
        mesh.uv = uvs;

        // let unity recalc the normals for us
        mesh.RecalculateNormals();

        // assign the newly populated mesh to our chunk
        go.GetComponent<MeshFilter>().mesh = mesh;

        // create an array to hold our materials
        Material[] temp = new Material[5];
        temp[0] = material0;
        temp[1] = material1;
        temp[2] = material2;
        temp[3] = material3;
        temp[4] = material4;

        // assign the materials array to our chunk
        go.GetComponent<MeshRenderer>().materials = temp;

        /*
         * BUG: if we don't null the collider before assigning
         * a new one it won't update the mesh collider
         */
        collider.sharedMesh = null;

        // update the collider
        collider.sharedMesh = mesh;

        // recalc the bounds of the collider
        mesh.RecalculateBounds();

        // move this chunk to its proper place in the world
        go.transform.localPosition = new Vector3(cx * chunkSize, cy*chunkSize, cz * chunkSize);
    }

    /*
     * assign the normals
     */
    private void setNormals(int offset)
    {
        normals[offset * 4] = -Vector3.forward;
        normals[offset * 4 + 1] = -Vector3.forward;
        normals[offset * 4 + 2] = -Vector3.forward;
        normals[offset * 4 + 3] = -Vector3.forward;
    }

    /*
     * assign the UVs
     */
    private void setUVs(int offset)
    {
        uvs[offset * 4] = new Vector2(0.0f, 1.0f);
        uvs[offset * 4 + 1] = new Vector2(1.0f, 1.0f);
        uvs[offset * 4 + 2] = new Vector2(0.0f, 0.0f);
        uvs[offset * 4 + 3] = new Vector2(1.0f, 0.0f);
    }

    /*
     * assign the triangles into the supplied triangle array
     */
    private void assignTris(int[] tris, int offset)
    {
        tris[offset * 6] = 0 + offset * 4;
        tris[offset * 6 + 1] = 1 + offset * 4;
        tris[offset * 6 + 2] = 2 + offset * 4;
        tris[offset * 6 + 3] = 2 + offset * 4;
        tris[offset * 6 + 4] = 1 + offset * 4;
        tris[offset * 6 + 5] = 3 + offset * 4;
    }

    /*
     * Compute the distance between two points (vectors) in 3d space
     */
    public float distance(Vector3 p1, Vector3 p2)
    {
        return Vector3.Distance(p1, p2);
        //return Mathf.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y) + (p1.z - p2.z) * (p1.z - p2.z));
    }

    /*
     * loads our terrain data from disk
     */
    private void loadFromDisk(string fileName)
    {
        FileInfo fi = new System.IO.FileInfo(fileName);

        terrainSize = (int)Mathf.Sqrt(fi.Length / 4);

        BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open));

        if (reader == null)
        {
            Debug.Log("Failed to open file: " + fileName);
            return;
        }

        terrain = new float[terrainSize, terrainSize];

        for (int x = 0; x < terrainSize; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                terrain[x, z] = reader.ReadSingle();
            }
        }

        reader.Close();
    }

    /*
     * determine the min and max heights in the terrain array
     * useful for splitting meshes into submeshes based on elevation
     * for more aesthetic rendering
     */
    private void findTerrainMinMax()
    {
        minHeight = terrain[0, 0];
        maxHeight = terrain[0, 0];

        // brute force our way there
        for (int x = 0; x < terrainSize; x++)
        {
            for (int z = 0; z < terrainSize; z++)
            {
                if (terrain[x, z] < minHeight)
                    minHeight = terrain[x, z];

                if (terrain[x, z] > maxHeight)
                    maxHeight = terrain[x, z];
            }
        }
    }

    /*
    * clear the arrays
    * we are reusing the arrays
    * because the garbage collector is lazy
    */
    private void clearArrays()
    {
        if (verts != null)
            System.Array.Clear(verts, 0, verts.Length);
        if (normals != null)
            System.Array.Clear(normals, 0, normals.Length);
        if (uvs != null)
            System.Array.Clear(uvs, 0, uvs.Length);
        if (triangles0 != null)
            System.Array.Clear(triangles0, 0, triangles0.Length);
        if (triangles1 != null)
            System.Array.Clear(triangles1, 0, triangles1.Length);
        if (triangles2 != null)
            System.Array.Clear(triangles2, 0, triangles2.Length);
        if (triangles3 != null)
            System.Array.Clear(triangles3, 0, triangles3.Length);
        if (triangles4 != null)
            System.Array.Clear(triangles4, 0, triangles4.Length);
    }

    /*
     * setup our mesh arrays
     */
    private void createArrays(int quads)
    {
        verts = new Vector3[quads * 4];

        triangles0 = new int[quads * 6];
        triangles1 = new int[quads * 6];
        triangles2 = new int[quads * 6];
        triangles3 = new int[quads * 6];
        triangles4 = new int[quads * 6];

        normals = new Vector3[quads * 4];
        uvs = new Vector2[quads * 4];
    }

    /*
     * returns the height of the nearest vertex
     */
    public float getHeight(Vector3 pos)
    {
        return terrain[Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z)];
    }

    private void setBlockStates(int bx, int by, int bz)
    {
        // assign the heightmap from our terrain generator
        for (int x = 0; x < bx; x++)
        {
            for (int y = 0; y < by; y++)
            {
                for (int z = 0; z < bz; z++)
                {
                    if ((int)terrain[x, z] >= y)
                        blocks[x, y, z].state = BlockState.occupied;
                    else
                        blocks[x, y, z].state = BlockState.empty;
                }
            }
        }
    }

    /*
     * loop through the current chunk and determine how many
     * faces (quads) are exposed and need to be rendered
     */ 
    private int quadCount(int cx, int cy, int cz)
    {
        int quads = 0;

        // count to see how many faces are exposed
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (blocks[x + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                    {
                        if (x > 0 && blocks[(x - 1) + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize].state == BlockState.occupied)
                            quads++;
                        if (x < chunkSize - 1 && blocks[(x + 1) + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize].state == BlockState.occupied)
                            quads++;
                        if (y > 0 && blocks[x + cx * chunkSize, (y - 1) + cy * chunkSize, z + cz * chunkSize].state == BlockState.occupied)
                            quads++;
                        if (y < chunkSize - 1 && blocks[x + cx * chunkSize, (y + 1) + cy * chunkSize, z + cz * chunkSize].state == BlockState.occupied)
                            quads++;
                        if (z > 0 && blocks[x + cx * chunkSize, y + cy * chunkSize, (z - 1) + cz * chunkSize].state == BlockState.occupied)
                            quads++;
                        if (z < chunkSize - 1 && blocks[x + cx * chunkSize, y + cy * chunkSize, (z + 1) + cz * chunkSize].state == BlockState.occupied)
                            quads++;
                    }
                    else
                    {
                        // chunk edge cases
                        if (y == chunkSize - 1 && y + cy * chunkSize < chunkSize * chunkSize - 1 && blocks[x + cx * chunkSize, y + 1 + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                            quads++;

                        if (x == 0 && x + cx * chunkSize > 0 && blocks[x - 1 + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                            quads++;

                        if (x == chunkSize - 1 && x + cx * chunkSize < chunkSize * chunkSize - 1 && blocks[x + 1 + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                            quads++;

                        if (z == 0 && z + cz * chunkSize > 0 && blocks[x + cx * chunkSize, y + cy * chunkSize, z - 1 + cz * chunkSize].state == BlockState.empty)
                            quads++;

                        if (z == chunkSize - 1 && z + cz * chunkSize < chunkSize * chunkSize - 1 && blocks[x + cx * chunkSize, y + cy * chunkSize, z + 1 + cz * chunkSize].state == BlockState.empty)
                            quads++;
                    }
                }
            }
        }

        return quads;
    }

    /*
     * loops through the mesh chunk and determines which faces (quads) are visible
     * then assigns the mesh verts+tris+normals+UVs for each visible face (quad)
     */ 
    private void setupQuads(int cx, int cy, int cz)
    {
        int offset = 0;

        // fill the mesh arrays
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (blocks[x + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize].state == BlockState.occupied)
                    {
                        //BlockType t = blocks[x + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize];
                        int height = y + cy * chunkSize;

                        // render the bottom facing up (floor)
                        if (y > 0 && blocks[x + cx * chunkSize, y - 1 + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                        {
                            verts[offset * 4] = new Vector3(x, y, z + 1);
                            verts[offset * 4 + 1] = new Vector3(x + 1, y, z + 1);
                            verts[offset * 4 + 2] = new Vector3(x, y, z);
                            verts[offset * 4 + 3] = new Vector3(x + 1, y, z);

                            windRight(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }
                        // render the top facing down (cieling)
                        if (y < chunkSize - 1 && blocks[x + cx * chunkSize, y + 1 + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                        {
                            verts[offset * 4] = new Vector3(x, y + 1, z + 1);
                            verts[offset * 4 + 1] = new Vector3(x + 1, y + 1, z + 1);
                            verts[offset * 4 + 2] = new Vector3(x, y + 1, z);
                            verts[offset * 4 + 3] = new Vector3(x + 1, y + 1, z);

                            windLeft(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }
                        // side
                        if (x > 0 && blocks[x - 1 + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                        {
                            verts[offset * 4] = new Vector3(x, y, z + 1);
                            verts[offset * 4 + 1] = new Vector3(x, y + 1, z + 1);
                            verts[offset * 4 + 2] = new Vector3(x, y, z);
                            verts[offset * 4 + 3] = new Vector3(x, y + 1, z);

                            windLeft(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }
                        // side
                        if (x < chunkSize - 1 && blocks[x + 1 + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                        {
                            verts[offset * 4] = new Vector3(x + 1, y, z + 1);
                            verts[offset * 4 + 1] = new Vector3(x + 1, y + 1, z + 1);
                            verts[offset * 4 + 2] = new Vector3(x + 1, y, z);
                            verts[offset * 4 + 3] = new Vector3(x + 1, y + 1, z);

                            windRight(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }
                        // side
                        if (z > 0 && blocks[x + cx * chunkSize, y + cy * chunkSize, z - 1 + cz * chunkSize].state == BlockState.empty)
                        {
                            verts[offset * 4] = new Vector3(x + 1, y, z);
                            verts[offset * 4 + 1] = new Vector3(x + 1, y + 1, z);
                            verts[offset * 4 + 2] = new Vector3(x, y, z);
                            verts[offset * 4 + 3] = new Vector3(x, y + 1, z);

                            windRight(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }
                        // side
                        if (z < chunkSize - 1 && blocks[x + cx * chunkSize, y + cy * chunkSize, z + 1 + cz * chunkSize].state == BlockState.empty)
                        {
                            verts[offset * 4] = new Vector3(x + 1, y, z + 1);
                            verts[offset * 4 + 1] = new Vector3(x + 1, y + 1, z + 1);
                            verts[offset * 4 + 2] = new Vector3(x, y, z + 1);
                            verts[offset * 4 + 3] = new Vector3(x, y + 1, z + 1);

                            windLeft(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }
                        /*
                         * edges of the chunk
                         */
                        if (y == chunkSize - 1 && y + cy * chunkSize < chunkSize * heightInChunks - 1 && blocks[x + cx * chunkSize, y + 1 + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                        {
                            // render the top of the cube facing up at the top of the world
                            verts[offset * 4] = new Vector3(x, y + 1, z + 1);
                            verts[offset * 4 + 1] = new Vector3(x + 1, y + 1, z + 1);
                            verts[offset * 4 + 2] = new Vector3(x, y + 1, z);
                            verts[offset * 4 + 3] = new Vector3(x + 1, y + 1, z);

                            windLeft(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }          
                        if (x == 0 && x + cx * chunkSize > 0 && blocks[x - 1 + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                        {
                            verts[offset * 4] = new Vector3(x, y, z + 1);
                            verts[offset * 4 + 1] = new Vector3(x, y + 1, z + 1);
                            verts[offset * 4 + 2] = new Vector3(x, y, z);
                            verts[offset * 4 + 3] = new Vector3(x, y + 1, z);

                            windLeft(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }
                        if (x == chunkSize - 1 && x + cx * chunkSize < terrainSize - 1 && blocks[x + 1 + cx * chunkSize, y + cy * chunkSize, z + cz * chunkSize].state == BlockState.empty)
                        {
                            verts[offset * 4] = new Vector3(x + 1, y, z + 1);
                            verts[offset * 4 + 1] = new Vector3(x + 1, y + 1, z + 1);
                            verts[offset * 4 + 2] = new Vector3(x + 1, y, z);
                            verts[offset * 4 + 3] = new Vector3(x + 1, y + 1, z);

                            windRight(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }
                        if (z == 0 && z + cz * chunkSize > 0 && blocks[x + cx * chunkSize, y + cy * chunkSize, z - 1 + cz * chunkSize].state == BlockState.empty)
                        {
                            verts[offset * 4] = new Vector3(x + 1, y, z);
                            verts[offset * 4 + 1] = new Vector3(x + 1, y + 1, z);
                            verts[offset * 4 + 2] = new Vector3(x, y, z);
                            verts[offset * 4 + 3] = new Vector3(x, y + 1, z);

                            windRight(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }
                        if (z == chunkSize - 1 && z + cz * chunkSize < terrainSize - 1 && blocks[x + cx * chunkSize, y + cy * chunkSize, z + 1 + cz * chunkSize].state == BlockState.empty)
                        {
                            verts[offset * 4] = new Vector3(x + 1, y, z + 1);
                            verts[offset * 4 + 1] = new Vector3(x + 1, y + 1, z + 1);
                            verts[offset * 4 + 2] = new Vector3(x, y, z + 1);
                            verts[offset * 4 + 3] = new Vector3(x, y + 1, z + 1);

                            windLeft(offset, height);
                            setNormals(offset);
                            setUVs(offset);
                            offset++;
                        }
                    }
                }
            }
        }
    }

    /*
     * determine which triangle array to assign to
     */ 
    private void windLeft(int offset, int y)
    {
        float delta = (float)maxHeight / 5;

        if (y < delta + minHeight)
            leftTriangles(triangles0, offset);
        else if (y < delta * 2 + minHeight)
            leftTriangles(triangles1, offset);
        else if (y < delta * 3 + minHeight)
            leftTriangles(triangles2, offset);
        else if (y < delta * 4 + minHeight)
            leftTriangles(triangles3, offset);
        else
            leftTriangles(triangles4, offset);
    }

    /*
     * determine which triangle array to assign to
     */ 
    private void windRight(int offset, int y)
    {
        float delta = (float)maxHeight / 5;

        if (y < delta + minHeight)
            rightTriangles(triangles0, offset);
        else if (y < delta * 2 + minHeight)
            rightTriangles(triangles1, offset);
        else if (y < delta * 3 + minHeight)
            rightTriangles(triangles2, offset);
        else if (y < delta * 4 + minHeight)
            rightTriangles(triangles3, offset);
        else
            rightTriangles(triangles4, offset);
    }

    /*
     * wind the triangles left
     */ 
    private void leftTriangles(int[] tris, int offset)
    {
        tris[offset * 6] = 0 + offset * 4;
        tris[offset * 6 + 1] = 1 + offset * 4;
        tris[offset * 6 + 2] = 2 + offset * 4;
        tris[offset * 6 + 3] = 2 + offset * 4;
        tris[offset * 6 + 4] = 1 + offset * 4;
        tris[offset * 6 + 5] = 3 + offset * 4;
    }

    /*
     * wind the triangles right
     */ 
    private void rightTriangles(int[] tris, int offset)
    {
        tris[offset * 6] = 0 + offset * 4;
        tris[offset * 6 + 1] = 2 + offset * 4;
        tris[offset * 6 + 2] = 3 + offset * 4;
        tris[offset * 6 + 3] = 3 + offset * 4;
        tris[offset * 6 + 4] = 1 + offset * 4;
        tris[offset * 6 + 5] = 0 + offset * 4;
    }

    /*
     * user is attempting to delete a block
     * find the block to delete, then update the mesh
     */
    public void deleteBlock(RaycastHit hit)
    {
        // determine which face we hit
        Vector3 normal = hit.normal;

        // truncated hit values
        int tx = (int)hit.point.x;
        int ty = (int)hit.point.y;
        int tz = (int)hit.point.z;

        // adjust the block position
        if (normal.x > 0)
            tx -= 1;
        else if (normal.y > 0)
            ty -= 1;
        else if (normal.z > 0)
            tz -= 1;

        // set the block to null
        blocks[tx, ty, tz].state = BlockState.empty;

        // find all meshes that need to be rebuilt
        findMeshes(tx, ty, tz);
    }

    /*
     * user wants to add a block
     * determine where to put it, update the array
     * and rebuild the mesh
     */
    public void addBlock(RaycastHit hit)
    {
        // determine which face we hit
        Vector3 normal = hit.normal;

        // truncated hit values
        int tx = (int)hit.point.x;
        int ty = (int)hit.point.y;
        int tz = (int)hit.point.z;

        // adjust the block position
        if (normal.x < 0)
            tx -= 1;
        else if (normal.y < 0)
            ty -= 1;
        else if (normal.z < 0)
            tz -= 1;

        // set the block to occupied
        blocks[tx, ty, tz].state = BlockState.occupied;

        // find all meshes that need to be rebuilt
        findMeshes(tx, ty, tz);
    }

    /*
     * determine which meshes need to be rebuilt
     */
    private void findMeshes(int x, int y, int z)
    {
        // convert map coordinates to chunk coordinates
        int cx = x / chunkSize;
        int cy = y / chunkSize;
        int cz = z / chunkSize;

        // regen the chunk we are on
        rebuildMesh(cx, cy, cz);

        /*
         * check to see if we need to rebuild nearby chunks
         */
        if (x > 0 && x <= terrainSize - chunkSize && x % chunkSize == 0)
        {
            rebuildMesh(cx-1, cy, cz);
        }
        if (x > 0 && x <= terrainSize - chunkSize && x % chunkSize == chunkSize - 1)
        {
            rebuildMesh(cx + 1, cy, cz);
        }
        if (y > 0 && y <= heightInChunks*chunkSize - chunkSize && y % chunkSize == 0)
        {
            rebuildMesh(cx, cy - 1, cz);
        }
        if (y > 0 && y <= terrainSize - chunkSize && y % chunkSize == chunkSize - 1)
        {
            rebuildMesh(cx, cy + 1, cz);
        }
        if (z > 0 && z <= terrainSize - chunkSize && z % chunkSize == 0)
        {
            rebuildMesh(cx, cy, cz - 1);
        }
        if (z > 0 && z <= terrainSize - chunkSize && z % chunkSize == chunkSize - 1)
        {
            rebuildMesh(cx, cy, cz + 1);
        }
    }

    /*
     * the mesh chunk has been modified and needs to be rebuilt
     */
    private void rebuildMesh(int cx, int cy, int cz)
    {
        // retrieve the mesh chunk
        GameObject go = chunks[cx, cy, cz];

        // create a new mesh
        mesh = new Mesh();

        fillMesh(cx, cy, cz, go, go.GetComponent<MeshCollider>(), mesh);
    }
}
