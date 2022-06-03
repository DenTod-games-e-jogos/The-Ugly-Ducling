using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VoxelController : MonoBehaviour
{
    /*
     * the object which olds our player, first person controller, and camera
     */
    public Transform player;

    /*
     * hook to our renderer
     */
    private VoxelRenderer map;

    /*
     * we save our old player position to save on unecessary calls
     */
    private Vector3 oldPos;

    // Use this for initialization
    void Start()
    {
        // get a hook to the voxel renderer script
        map = GameObject.Find("Voxel").GetComponent<VoxelRenderer>();

        /*
         * set the player just above the terrain at their given location
         * so they do not fall through the world
         */
        player.position = new Vector3(player.position.x, map.getHeight(player.position) + 2, player.position.z);

        /*
         * generate the nearby map chunks to start
         */
        map.genChunks(player.position);

        /*
         * save the player location
         */
        oldPos = player.position;
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * cast a ray to find out where on the terrain the mouse cursor is hovering over
         */
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit);

        /*
         * check to see if we stepped over a tile boundary
         * we do this to reduce the number of calls made
         */
        if ((int)player.position.x != (int)oldPos.x || (int)player.position.z != (int)oldPos.z)
        {
            // generate nearby map chunks as needed
            map.genChunks(player.position);
        }

        /*
         * see if we are mousing over the terrain
         */
        if (hit.transform != null)// && hit.transform.gameObject.tag == "Mesh")
        {
            // left mouse click
            if (Input.GetMouseButtonUp(0))
            {
                // delete the block we clicked on
                map.deleteBlock(hit);
            }
            // right mouse click
            else if (Input.GetMouseButtonUp(1))
            {
                // create a block adjacent to the block face we clicked on
                map.addBlock(hit);
            }
        }

        // save our old player position
        oldPos = player.position;
    }
}