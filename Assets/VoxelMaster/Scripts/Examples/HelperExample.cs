using UnityEngine;
using UnityEngine.UI;
using VoxelMaster;

public class HelperExample : MonoBehaviour
{
    public VoxelTerrain terrain;
    public Text infoText;

    new Camera camera;

    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        Ray r = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        RaycastHit hit;

        if (Physics.Raycast(r, out hit))
        {
            Vector3 pos = hit.point - (hit.normal / 2);
            bool isGround = terrain.IsBlockGround(pos);
            bool isWall = terrain.IsBlockWall(pos);
            bool isCeiling = terrain.IsBlockCeiling(pos);
            infoText.text = string.Format("Is Ground: {0}\nIs Wall: {1}\nIs Ceiling: {2}", isGround, isWall, isCeiling);
        }
        else
        {
            infoText.text = string.Format("Is Ground: {0}\nIs Wall: {1}\nIs Ceiling: {2}", "?", "?", "?");
        }
    }
}