using UnityEngine;

namespace VoxelMaster
{
    public class BasicCamera : MonoBehaviour
    {
        public bool locked = true;
        public float speed = 10f;
        public VoxelTerrain terrain;

        new Camera camera;
        float lookAnglesx;
        float lookAnglesy;

        void Start()
        {
            camera = GetComponent<Camera>();
        }

        void Update()
        {
            DoMouse();
            DoMovement();
            DoBlockBreaking();
        }

        void DoMouse()
        {
            bool finalLocked = (Input.GetKey(KeyCode.Tab) ? false : locked);
            Cursor.lockState = (finalLocked ? CursorLockMode.Locked : CursorLockMode.None);
            Cursor.visible = !finalLocked;

            lookAnglesx += Input.GetAxis("Mouse X");
            lookAnglesy += Input.GetAxis("Mouse Y");
            lookAnglesy = Mathf.Clamp(lookAnglesy, -89, 89);

            transform.eulerAngles = new Vector3(-lookAnglesy, lookAnglesx, 0);
        }

        void DoMovement()
        {
            Vector3 velocity = new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed);
            transform.Translate(velocity * Time.deltaTime);
        }

        void DoBlockBreaking()
        {
            if (Input.GetMouseButtonDown(0)) // Destroy block
            {
                Ray r = camera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
                RaycastHit hit;
                Physics.Raycast(r, out hit);

                if (hit.collider != null)
                {
                    Vector3 final = hit.point - (hit.normal * 0.5f);
                    terrain.RemoveBlockAt(final);
                    terrain.FastRefresh();
                }
            }
            else if (Input.GetMouseButtonDown(1)) // Add stone block
            {
                Ray r = camera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
                RaycastHit hit;
                Physics.Raycast(r, out hit);

                if (hit.collider != null)
                {
                    Vector3 final = hit.point + (hit.normal * 0.5f);
                    terrain.SetBlockID(final, 1);
                    terrain.FastRefresh();
                }
            }
        }

        void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 200, 20), "Voxel Generation")) Application.LoadLevel("Voxel Master Generation");
            if (GUI.Button(new Rect(10, 30, 200, 20), "Voxel Shapes")) Application.LoadLevel("Voxel Master Shapes");
            if (GUI.Button(new Rect(10, 50, 200, 20), "Voxel Helpers")) Application.LoadLevel("Voxel Master Helpers");
            if (GUI.Button(new Rect(10, 70, 200, 20), "Voxel Loading-Saving System")) Application.LoadLevel("Voxel Master Loading-Saving System");

            if (Application.loadedLevelName == "Voxel Master Loading-Saving System")
            {
                GUI.Label(new Rect(220, 30, 200, 30), "Press R to save");
            }
        }
    }
}