
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelColiderManager : MonoBehaviour
{
    [Header("Gerenciador de blocos para o mondo Voxel")]
    [SerializeField] GameObject voxelWorldManager;
    private World world;
    [Header("Boneco do Player")]
    [SerializeField] GameObject player;
    [Header("Para depuração, retirar depois")]
    [SerializeField] GameObject top;
    MeshCollider topCollider;
    [SerializeField] GameObject ground;
    MeshCollider groundCollider;
    [SerializeField] GameObject left;
    MeshCollider leftCollider;
    [SerializeField] GameObject right;
    MeshCollider rightCollider;
    [SerializeField] GameObject front;
    MeshCollider frontCollider;
    [SerializeField] GameObject back;
    MeshCollider backCollider;
    [SerializeField] bool canGoFront;
    [SerializeField] bool canGoBack;
    [SerializeField] bool canGoLeft;
    [SerializeField] bool canGoRight;
    [SerializeField] bool canGoTop;
    [SerializeField] bool canGoGround;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = player.transform.position;
        world = voxelWorldManager.GetComponent<World>();
        topCollider = top.GetComponent<MeshCollider>();
        groundCollider = ground.GetComponent<MeshCollider>();
        leftCollider = left.GetComponent<MeshCollider>();
        rightCollider = right.GetComponent<MeshCollider>();
        frontCollider = front.GetComponent<MeshCollider>();
        backCollider = back.GetComponent<MeshCollider>();
        Debug.Log(world);
    }

    // Update is called once per frame
    void Update()
    {
        MoveColiders();
        ManageColliders();
    }

    private void MoveColiders()
    {
        Vector3 newPosition = player.transform.position;
        newPosition.x = Mathf.Round(newPosition.x);
        newPosition.y = Mathf.Round(newPosition.y);
        newPosition.z = Mathf.Round(newPosition.z);
        transform.position = newPosition;
    }
    private void ManageColliders()
    {
        // Precisa ajustar a posição dos blocos para corrigir os colliders
        //Front Wall Detection
        if (world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + 1)))
        {
            canGoFront = false;
        }
        else
        {
            canGoFront = true;
        }
        if (canGoFront)
        {
            frontCollider.enabled = false;
        } else
        {
            frontCollider.enabled = true;
        }
        
        //Back Wall Detection
        if (world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - 1)))
        {
            canGoBack = false;
        }
        else
        {
            canGoBack = true;
        }
        if (canGoBack)
        {
            backCollider.enabled = false;
        } else
        {
            backCollider.enabled = true;
        }
        
        //Left Wall Detection
        if (world.CheckForVoxel(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z)))
        {
            canGoLeft = false;
        }
        else
        {
            canGoLeft = true;
        }
        if (canGoLeft)
        {
            leftCollider.enabled = false;
        } else
        {
            leftCollider.enabled = true;
        }
        
        //Right Wall Detection
        if (world.CheckForVoxel(new Vector3(transform.position.x + 1, transform.position.y, transform.position.z)))
        {
            canGoRight = false;
        }
        else
        {
            canGoRight = true;
        }
        if (canGoRight)
        {
            rightCollider.enabled = false;
        } else
        {
            rightCollider.enabled = true;
        }
        
        //Ceilling Detection
        if (world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z)))
        {
            canGoTop = false;
        }
        else
        {
            canGoTop = true;
        }
        if (canGoTop)
        {
            topCollider.enabled = false;
        } else
        {
            topCollider.enabled = true;
        }

        //Gound Detection
        if (world.CheckForVoxel(new Vector3(transform.position.x, transform.position.y - 1, transform.position.z)))
        {
            canGoGround = false;
        }
        else
        {
            canGoGround = true;
        }
        if (canGoGround)
        {
            groundCollider.enabled = false;
        } else
        {
            groundCollider.enabled = true;
        }
    }

}
