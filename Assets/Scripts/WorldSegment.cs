using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class WorldSegment 
{
    public Material cubeMaterial, waterMaterial;
    public Cube[,,] segmentData;
    public GameObject segment, waterSegment;
    public enum SegmentStatus {DRAW,DONE,KEEP};
    public SegmentStatus status;    
   
    /// <summary>
    /// builds the segment
    /// </summary>
    public void BuildSegment()
    {
        segmentData = new Cube[World.Instance.segmentSize, World.Instance.segmentSize, World.Instance.segmentSize];
        
        for (int z = 0; z < World.Instance.segmentSize; ++z)
        {
            for (int y = 0; y < World.Instance.segmentSize; ++y)
            {
                for (int x = 0; x < World.Instance.segmentSize; ++x)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    int worldX = (int)(x + segment.transform.position.x);
                    int worldY = (int)(y + segment.transform.position.y);
                    int worldZ = (int)(z + segment.transform.position.z);
                    
                    // uses noise functions to generate the terrain 
                    if (worldY <= Noise.Instance.GetSandHeight(worldX, worldZ))
                    {
                        segmentData[x, y, z] = new Cube(Cube.CubeType.SAND, pos, segment.gameObject, this);
                    }
                    else if (worldY <= Noise.Instance.GetStoneHeight(worldX, worldZ))
                    {
                        segmentData[x, y, z] = new Cube(Cube.CubeType.STONE, pos, segment.gameObject, this);
                    }
                    else if (worldY == Noise.Instance.GetDirtHeight(worldX, worldZ))
                    {
                        segmentData[x, y, z] = new Cube(Cube.CubeType.GRASS, pos, segment.gameObject, this);
                    }
                    else if (worldY <= Noise.Instance.GetDirtHeight(worldX, worldZ))
                    {
                        segmentData[x, y, z] = new Cube(Cube.CubeType.DIRT, pos, segment.gameObject, this);
                    }
                    else if (worldY <= World.Instance.waterHeight)
                    {
                        segmentData[x, y, z] = new Cube(Cube.CubeType.WATER, pos, waterSegment.gameObject, this);
                    }
                    else
                    {
                        segmentData[x, y, z] = new Cube(Cube.CubeType.AIR, pos, segment.gameObject, this);
                    }

                    if(segmentData[x,y,z].cType != Cube.CubeType.WATER && Noise.Instance.FractalBrownianMotion3D(worldX, worldY, worldZ) < 0.33f)
                    {
                        segmentData[x, y, z] = new Cube(Cube.CubeType.AIR, pos, segment.gameObject, this);
                    }
                    status = SegmentStatus.DRAW;
                }
            }
        }
    }
    /// <summary>
    /// Draws the Segment
    /// </summary>
    public void DrawSegment()
    {
        for (int z = 0; z < World.Instance.segmentSize; ++z)
        {
            for (int y = 0; y < World.Instance.segmentSize; ++y)
            {
                for (int x = 0; x < World.Instance.segmentSize; ++x)
                {
                    segmentData[x, y, z].Draw();
                }
            }
        }
        FuseQuads( segment, cubeMaterial);
        MeshCollider collider = segment.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = segment.transform.GetComponent<MeshFilter>().mesh;
        FuseQuads(waterSegment, waterMaterial);
    }

    /// <summary>
    /// bulid the segment when constructed
    /// </summary>
    /// <param name="position">position of the segment in the world</param>
    /// <param name="cubeMat">the material for the ground</param>
    /// <param name="waterMat">the marerial for the water</param>
    public WorldSegment (Vector3 position, Material cubeMat, Material waterMat)
    {
        segment = new GameObject(World.BuildSegmentName(position));
        waterSegment = new GameObject(World.BuildSegmentName(position) + "w");
        segment.transform.position = waterSegment.transform.position = position;
        cubeMaterial = cubeMat;
        waterMaterial = waterMat;
        BuildSegment();
    }      

   /// <summary>
   /// combines the meshes of the cube in to one segment 
   /// </summary>
    void FuseQuads( GameObject segmentObject, Material segmentMaterial)
    {        
        MeshFilter[] meshFilters = segmentObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }      
        MeshFilter mf = (MeshFilter)segmentObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();        
        mf.mesh.CombineMeshes(combine);      
        MeshRenderer renderer = segmentObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = segmentMaterial;       
        foreach (Transform quad in segmentObject.transform)
        {
           GameObject.Destroy(quad.gameObject);
        }
    }   
}
