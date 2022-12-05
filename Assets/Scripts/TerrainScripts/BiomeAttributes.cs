using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "MinecraftTutorial/Biome Attribute")]
public class BiomeAttributes : ScriptableObject 
{
    public string biomeName;

    public int solidGroundHeight;

    public int terrainHeight;
    
    public float terrainScale;

    [Header("Trees")]
    public float treeZoneScale = 1.3f;

    [Header("Houses")]
    public float houseZoneScale = 2.5f;
    
    [Range(0.1f, 1f)]
    public float treeZoneThreshold = 0.6f;

    public float treePlacementScale = 15f;
    
    [Range(0.1f, 1f)]
    public float treePlacementThreshold = 0.8f;

    [Range(0.5f, 2.5f)]
    public float houseZoneThreshold = 0.5f;

    public float housePlacementScale = 20f;

    [Range(0.5f, 1.5f)]
    public float housePlacementThreshold = 1.0f;

    public int maxTreeHeight = 12;
    
    public int minTreeHeight = 5;

    public int maxHouseHeight = 15;

    public int minHouseHeight = 8;

    public Lode[] lodes;
}

[System.Serializable]
public class Lode 
{
    public string nodeName;
    
    public byte blockID;
    
    public int minHeight;
    
    public int maxHeight;
    
    public float scale;
    
    public float threshold;
    
    public float noiseOffset;
}