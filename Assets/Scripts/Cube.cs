using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// used for storeing uv's
/// </summary>
struct UV
{
    public Vector2[] uvs;
    public UV(float uv1, float uv2, float uv3, float uv4, float uv5, float uv6, float uv7, float uv8)
    {
        uvs = new [] { new Vector2(uv1, uv2), new Vector2(uv3, uv4), new Vector2(uv5, uv6),new Vector2(uv7, uv8) };
    }
}
public class Cube
{
    enum CubeFaces { BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK };
    public enum CubeType { GRASS, DIRT, WATER, STONE, SAND, AIR};
    public CubeType cType;

    GameObject parent;
    Vector3 position;

    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    private int[] triangles;
    public bool isSolid;
    WorldSegment owner;

    //vertex points
    Vector3 point0 = new Vector3(-0.5f, -0.5f, 0.5f),
            point1 = new Vector3(0.5f, -0.5f, 0.5f),
            point2 = new Vector3(0.5f, -0.5f, -0.5f),
            point3 = new Vector3(-0.5f, -0.5f, -0.5f),
            point4 = new Vector3(-0.5f, 0.5f, 0.5f),
            point5 = new Vector3(0.5f, 0.5f, 0.5f),
            point6 = new Vector3(0.5f, 0.5f, -0.5f),
            point7 = new Vector3(-0.5f, 0.5f, -0.5f);

    /// <summary>
    /// cube uv co-ordinates for the texture atlas
    /// </summary>
    UV[] cubeUVs =
    {
        /*GRASS TOP*/ 
        new UV(0f,0.9375f,0.0625f,0.9375f,0f,1f,0.0625f,1f),

        /*GRASS SIDES*/ 
        new UV(0.1875f,0.9375f,0.25f,0.9375f,0.1875f,1f,0.25f,1f),

         /*DIRT*/ 
        new UV(0.125f,0.9375f,0.1875f,0.9375f,0.125f,1f,0.1875f,1f),

        /*WATER*/ 
        new UV(0.9375f,0.125f,1f,0.125f,0.9375f,0.1875f,1f,0.1875f),

         /*STONE*/
        new UV(0.875f,0.75f,0.9375f,0.75f,0.875f,0.8125f,0.9375f,0.8125f),

        /*SAND*/ 
        new UV(0f,0.25f,0.0625f,0.25f,0f,0.3125f,0.0625f,0.3125f)
    };
   
    /// <summary>
    /// constructer 
    /// </summary>
    /// <param name="cube">the type of cube</param>
    /// <param name="pos">postion of the cube</param>
    /// <param name="par">the parent object</param>
    /// <param name="own">the segment it inside</param>
    public Cube(CubeType cube, Vector3 pos, GameObject par, WorldSegment own)
    {
        cType = cube;
        parent = par;
        position = pos;
        owner = own;
       
        if(cType == CubeType.WATER)
        {
            parent = own.waterSegment;
        }
        if(cType == CubeType.AIR || cType == CubeType.WATER)
        {
            isSolid = false;
        }
        else
        {
            isSolid = true;
        }       
    }
    /// <summary>
    /// creates a quad mesh
    /// </summary>
    /// <param name="face">the sides of the quads</param>
    void CreateMesh(CubeFaces face)
    {
        vertices = new Vector3[4];
        normals = new Vector3[4];
        uvs = new Vector2[4];
        triangles = new int[6];
        Mesh mesh = new Mesh();
        mesh.name = "Mesh" + face.ToString();

        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if (cType == CubeType.GRASS && face == CubeFaces.TOP)
        {
            uv00 = cubeUVs[0].uvs[0];
            uv10 = cubeUVs[0].uvs[1]; 
            uv01 = cubeUVs[0].uvs[2]; 
            uv11 = cubeUVs[0].uvs[3]; 
        }
        else if (cType == CubeType.GRASS && face == CubeFaces.BOTTOM)
        {
            uv00 = cubeUVs[(int)(CubeType.DIRT + 1)].uvs[0];
            uv10 = cubeUVs[(int)(CubeType.DIRT + 1)].uvs[1];
            uv01 = cubeUVs[(int)(CubeType.DIRT + 1)].uvs[2];
            uv11 = cubeUVs[(int)(CubeType.DIRT + 1)].uvs[3];
        }
        else
        {
            uv00 = cubeUVs[(int)(cType + 1)].uvs[0];
            uv10 = cubeUVs[(int)(cType + 1)].uvs[1];
            uv01 = cubeUVs[(int)(cType + 1)].uvs[2];
            uv11 = cubeUVs[(int)(cType + 1)].uvs[3];
        }
       
        uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
        triangles = new int[] { 3, 1, 0, 3, 2, 1 };
        switch (face)
        {
            case CubeFaces.BOTTOM:
                vertices = new Vector3[] { point0, point1, point2, point3 };
                normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                break;

            case CubeFaces.TOP:
                vertices = new Vector3[] { point7, point6, point5, point4 };
                normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };              
                break;

            case CubeFaces.LEFT:
                vertices = new Vector3[] { point7, point4, point0, point3 };
                normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };               
                break;

            case CubeFaces.RIGHT:
                vertices = new Vector3[] { point5, point6, point2, point1 };
                normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };               
                break;

            case CubeFaces.FRONT:
                vertices = new Vector3[] { point4, point5, point1, point0 };
                normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };               
                break;

            case CubeFaces.BACK:
                vertices = new Vector3[] { point6, point7, point3, point2 };
                normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };               
                break;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        GameObject quad = new GameObject("Quad");
        quad.transform.position = position;
        quad.transform.parent = parent.transform;
        MeshFilter meshfilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshfilter.mesh = mesh;
    }
    /// <summary>
    /// returns value between 0 and 15
    /// </summary>
    /// <param name="i">the index of the cube</param>
    int ConvertCubeIndexToLocal(int i)
    {
        if (i == -1)
        {
            i = World.Instance.segmentSize - 1;
        }
        else if (i == World.Instance.segmentSize)
        {
            i = 0;
            return i;
        }
        return i;
    }

    /// <summary>
    /// returns weather a cube at a given co-ordenate is soild
    /// </summary>
   
    public bool LocalCubeIsSolid(int x, int y, int z)
    {
        Cube[,,] segments; 

        if (x < 0 || x >= World.Instance.segmentSize || y < 0 || y >= World.Instance.segmentSize || z < 0 || z >= World.Instance.segmentSize)
        {
            Vector3 adjacentSegmentPos = this.parent.transform.position + new Vector3((x - (int)position.x) * World.Instance.segmentSize, (y - (int)position.y) * World.Instance.segmentSize, (z - (int)position.z) * World.Instance.segmentSize);
            string nName = World.BuildSegmentName(adjacentSegmentPos);
            x = ConvertCubeIndexToLocal(x);
            y = ConvertCubeIndexToLocal(y);
            z = ConvertCubeIndexToLocal(z);

            WorldSegment adjSegment;
            if(World.segments.TryGetValue(nName, out adjSegment))
            {
                segments = adjSegment.segmentData;
            }
            else
            {
                return false;
            }
        }
        else
            segments = owner.segmentData;
        try
        {
            bool solid = segments[x, y, z].isSolid || segments[x,y,z].cType == cType;
            return solid;
        }
        catch(System.IndexOutOfRangeException) { }

        return false;
    }

    /// <summary>
    /// Draws each side of the cube 
    /// </summary>
    public void Draw()
    {
        if (cType == CubeType.AIR)
        {
            return;
        }
        if(!LocalCubeIsSolid((int)position.x,(int)position.y,(int)position.z + 1) && cType != CubeType.WATER)
        {
            CreateMesh(CubeFaces.FRONT);
        }
        if (!LocalCubeIsSolid((int)position.x, (int)position.y, (int)position.z - 1) && cType != CubeType.WATER)
        {
            CreateMesh(CubeFaces.BACK);
        }
        if (!LocalCubeIsSolid((int)position.x, (int)position.y + 1, (int)position.z ))
        {
            CreateMesh(CubeFaces.TOP);
        }
        if (!LocalCubeIsSolid((int)position.x, (int)position.y - 1, (int)position.z))
        {
            CreateMesh(CubeFaces.BOTTOM);
        }
        if (!LocalCubeIsSolid((int)position.x - 1, (int)position.y, (int)position.z) && cType != CubeType.WATER)
        {
            CreateMesh(CubeFaces.LEFT);
        }
        if (!LocalCubeIsSolid((int)position.x + 1, (int)position.y, (int)position.z) && cType != CubeType.WATER)
        {
            CreateMesh(CubeFaces.RIGHT);
        }
    }
}
