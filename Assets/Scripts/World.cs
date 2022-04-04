using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static Vector3 worldDimensions = new Vector3(10, 10, 10);
    
    public static Vector3 chunkDimensions = new Vector3(10, 10, 10);

    public GameObject chunkPrefab;

    void Start()
    {
        StartCoroutine(BuildWorld());
    }

    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int y = 0; y < worldDimensions.y; y++)
            {
                for (int x = 0; x < worldDimensions.x; x++)
                {
                    GameObject chunk = Instantiate(chunkPrefab);

                    Vector3 position = new Vector3(x * chunkDimensions.x, y * chunkDimensions.y, z * chunkDimensions.z);
                    
                    chunk.GetComponent<Chunk>().BuildChunk();
                    
                    yield return null;
                }
            }
        }
    }
}