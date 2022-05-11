using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    public GameObject Tree;

    void Start()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                Instantiate(Tree, new Vector3(x, -0.5f, z), Quaternion.identity);
            }
        }
    }
}