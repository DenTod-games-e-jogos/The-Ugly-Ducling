using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HeightmapRenderer : MonoBehaviour
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
     * The size of 1 mesh (larger terrains are made up of multiple meshes)
     * NOTE: a single mesh can contain 65,000 maximum vertices.  
     * A 100x100 tile mesh requires 100x100x4 = 40,000 unique vertices
     * Thus we set of size of 100 to stay safely below the vertex limit
     */ 
    private static int meshSize = 100;

    /*
     * array to hold our terrain height data
     * in a more complex setup this would likely be an array of structs 
     * instead of an array of floats to hold additional data
     */
    private float[,] terrain;

    /*
     * array to hold all of our map chunks
     * each map chunk is an individual mesh
     */
    private GameObject[,] chunks;

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
     * render distance as measured in size of mesh chunks
     */
    public int renderDistance = 2;

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
         * setup our map chunk array
         */ 
        chunks = new GameObject[terrainSize / meshSize, terrainSize / meshSize];

        /*
         * get min/max heights from our terrain array
         */ 
        findTerrainMinMax();
    }

    /*
     * generate map chunks near the given vector
     */ 
    public void genChunks(Vector3 pos)
    {
        // visible radius (measured in mesh sizes)
        int radius = renderDistance * meshSize;

        // chunk size
        int cSize = terrainSize / meshSize;

        /*
         * loop through all the map chunks
         * disable ones outside our viewing radius
         * enable and/or create chunks within our viewing radius
         */
        for (int cx = 0; cx < cSize; cx++)
        {
            for (int cz = 0; cz < cSize; cz++)
            {
                Vector3 chunkPrime = new Vector3(cx*meshSize + meshSize/2, pos.y, cz*meshSize + meshSize/2);

                if (Vector3.Distance(pos, chunkPrime) < radius)
                {
                    // create a new mesh chunk
                    if (chunks[cx, cz] == null)
                        createMesh(cx, cz);

                    // show the chunk
                    chunks[cx, cz].SetActive(true);
                }
                else
                {
                    // hide the chunk
                    if (chunks[cx, cz] != null)
                    {
                        chunks[cx, cz].SetActive(false);
                    }
                }
            }
        }
    }

    /*
     * creates a new game object and mesh
     */ 
    private void createMesh(int cx, int cz)
    {
        // create an empty game object
        GameObject go = new GameObject();

        // assign it to the chunk array
        chunks[cx, cz] = go;

        // name it so we can find it easily in the hierarchy
        go.name = "Mesh: " + cx + ", " + cz;

        // add a tag for easy reference later
        //go.tag = "Mesh";

        // add needed componenents  (mesh filter, mesh renderer, and mesh collider)
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshCollider>();

        // create a new mesh
        mesh = new Mesh();

        // generate the mesh data
        fillMesh(cx, 0, cz, chunks[cx, cz], chunks[cx, cz].GetComponent<MeshCollider>(), mesh);
    }

    /*
     * Divide each terrain mesh into 5 submeshes
     * based on elevation
     */ 
    private void determineSubMesh(int offset, float y)
    {
        float delta = (maxHeight - minHeight) / 5;
        
        if(y < delta + minHeight)
            assignTris(triangles0, offset);
        else if(y < delta*2 + minHeight)
            assignTris(triangles1, offset);
        else if (y < delta*3 + minHeight)
            assignTris(triangles2, offset);
        else if (y < delta*4 + minHeight)
            assignTris(triangles3, offset);
        else
            assignTris(triangles4, offset);
    }

    /*
     * modifies the vertex at pos by delta
     */ 
    public void modifyVertex(Vector3 pos, float delta)
    {
        // round to the nearest vertex
        int x = Mathf.RoundToInt(pos.x);
        int z = Mathf.RoundToInt(pos.z);

        // adjust the vertex
        terrain[x, z] += delta;

        // deal with rounding errors
        terrain[x, z] = (float)System.Math.Round(terrain[x, z], 1);

        // find which meshes need to be rebuilt
        findMeshes(x, z);

        /*
        // determine which mesh this vertex is in
        int cx = x / meshSize;
        int cz = z / meshSize;
        fillMesh(cx, 0, cz, chunks[cx, cz], chunks[cx, cz].GetComponent<MeshCollider>(), chunks[cx, cz].GetComponent<MeshFilter>().mesh);
        */
    }

    /*
     * generates the mesh data and assigns it to game object
     */ 
    private void fillMesh(int cx, int cy, int cz, GameObject go, MeshCollider collider, Mesh mesh)
    {
        // the total number of textured quads we are creating
        int quads = meshSize * meshSize;

        // clear the mesh arrays
        clearArrays();

        // create new arrays
        createArrays(quads);

        // keep track of our current location when assigning data to the arrays
        int offset = 0;

        /*
         * fill the mesh arrays
         */ 
        for (int x = 0; x < meshSize; x++)
        {
            for (int z = 0; z < meshSize; z++)
            {
                // set verts
                verts[offset * 4] = new Vector3(x, terrain[x + cx * meshSize, z + 1 + cz * meshSize], z + 1);
                verts[offset * 4 + 1] = new Vector3(x + 1, terrain[x + 1 + cx * meshSize, z + 1 + cz * meshSize], z + 1);
                verts[offset * 4 + 2] = new Vector3(x, terrain[x + cx * meshSize, z + cz * meshSize], z);
                verts[offset * 4 + 3] = new Vector3(x + 1, terrain[x + 1 + cx * meshSize, z + cz * meshSize], z);

                // set triangles
                determineSubMesh(offset, terrain[x + cx * meshSize, z + cz * meshSize]);

                // set normals and UVs
                setNormals(offset);
                setUVs(offset);

                offset++;
            }
        }

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
        go.transform.localPosition = new Vector3(cx * meshSize, 0, cz * meshSize);
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

        terrainSize = (int)Mathf.Sqrt(fi.Length/4);

        BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open));

        if(reader == null)
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

    /*
     * determine which meshes need to be rebuilt
     */
    private void findMeshes(int x, int z)
    {
        // convert coordinates to chunk coordinates
        int cx = x / meshSize;
        int cz = z / meshSize;

        // regen the chunk we are on
        rebuildMesh(cx, cz);

        /*
         * check to see if we need to rebuild nearby chunks
         */
        if (x > 0 && x <= terrainSize - meshSize && x % meshSize == 0)
        {
            rebuildMesh(cx - 1, cz);
        }
        if (x > 0 && x <= terrainSize - meshSize && x % meshSize == meshSize - 1)
        {
            rebuildMesh(cx + 1, cz);
        }
        if (z > 0 && z <= terrainSize - meshSize && z % meshSize == 0)
        {
            rebuildMesh(cx, cz - 1);
        }
        if (z > 0 && z <= terrainSize - meshSize && z % meshSize == meshSize - 1)
        {
            rebuildMesh(cx, cz + 1);
        }
    }

    /*
     * the mesh chunk has been modified and needs to be rebuilt
     */
    private void rebuildMesh(int cx, int cz)
    {
        // retrieve the mesh chunk
        GameObject go = chunks[cx, cz];

        // create a new mesh
        mesh = new Mesh();

        fillMesh(cx, 0, cz, go, go.GetComponent<MeshCollider>(), mesh);
    }
}
