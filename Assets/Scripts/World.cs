using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;


public class World : MonoBehaviour
{
    public GameObject Player;
    public Material groundAtlas; 
    public Material waterAtlas;
    public int segmentHeight = 16; 
    public int segmentSize = 16; 
    public int waterHeight = 60;
    private int playerRadius = 2; 
    public static Dictionary<string, WorldSegment> segments;
    public static List<string> toRemove = new List<string>();
    bool building = false;
    private static World instance;

    /// <summary>
    /// the singleton instance
    /// </summary>
    public static World Instance
    {
        get { if (instance == null) instance = GameObject.FindObjectOfType<World>();  return instance; }
    }

    public void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// creates a string out of the co-ordinates 
    /// </summary>
    public static string BuildSegmentName(Vector3 v)
    {
        return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
    }

    /// <summary>
    /// creates all the segments for the terrain and marks all the segment out of the players radius to be removed 
    /// </summary>
    IEnumerator BuildWorld()
    {             
        building = true;
        int posx = (int)Mathf.Floor(Player.transform.position.x / segmentSize);
        int posz = (int)Mathf.Floor(Player.transform.position.z / segmentSize);

        for (int z = -playerRadius; z <= playerRadius; z++)
        {
            for (int x = -playerRadius; x <= playerRadius; ++x)
            {
                for (int y = 0; y < segmentHeight; ++y)
                {                  
                    Vector3 segmentPosition = new Vector3((x+posx) * segmentSize, y * segmentSize, (posz+z) * segmentSize);
                    WorldSegment seg;
                    string segName = BuildSegmentName(segmentPosition);
                    if(segments.TryGetValue(segName, out seg))
                    {
                        seg.status = WorldSegment.SegmentStatus.KEEP;
                        break;
                    }
                    else
                    {
                        seg = new WorldSegment(segmentPosition, groundAtlas, waterAtlas);
                        seg.segment.transform.parent = this.transform;
                        seg.waterSegment.transform.parent = this.transform;
                        segments.Add(seg.segment.name, seg);                                               
                    }
                    yield return null;                  
                }
            }
        }

        foreach(KeyValuePair<string, WorldSegment> seg in segments)
        {
            if(seg.Value.status == WorldSegment.SegmentStatus.DRAW)
            {
                seg.Value.DrawSegment();
                seg.Value.status = WorldSegment.SegmentStatus.KEEP;
            }
            Vector2 playerPos2D = new Vector2(Player.transform.position.x, Player.transform.position.z);
            Vector2 segmentPos2D = new Vector2(seg.Value.segment.transform.position.x, seg.Value.segment.transform.position.z); 

            if (seg.Value.segment && Vector2.Distance(playerPos2D, segmentPos2D) > (playerRadius*2) * segmentSize)
            {
                toRemove.Add(seg.Key);
            }
            seg.Value.status = WorldSegment.SegmentStatus.DONE;
            yield return null;
        }
        Player.SetActive(true);
        RemoveOldWorldSegment();
        building = false;
    }

    /// <summary>
    /// removes segments that have been marked
    /// </summary>
    void RemoveOldWorldSegment()
    {
        for (int i = 0; i < toRemove.Count; i++)
        {
            string segName = toRemove[i];
            WorldSegment seg;
            if(segments.TryGetValue(segName, out seg))
            {
                Destroy(seg.segment);
                Destroy(seg.waterSegment);
                segments.Remove(segName);
            }
        }
        toRemove.Clear();
    }
   
    void Start()
    {
        Player.SetActive(false);
        segments = new Dictionary<string, WorldSegment>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        SaveManager.Load();        
        StartCoroutine(BuildWorld());        
    }
   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            SaveManager.Save();   
        }
        if (!building)
        {
            StartCoroutine(BuildWorld());
        }        
    }
}
