using UnityEngine;

namespace VoxelMaster
{
    public class ChunkManager : MonoBehaviour
    {
        /// <summary>
        /// Determines if this chunk will be destroyed when it is outside of the dispose distance.
        /// </summary>
        [HideInInspector]
        public bool destroyUnused = false;
        /// <summary>
        /// Determines if this chunk is dirty.
        /// </summary>
        [HideInInspector]
        public bool dirty = false;
        /// <summary>
        /// The update rate of the ChunkManager.
        /// </summary>
        [Tooltip("The update rate of the Chunk Manager, 0.5 is a good value.")]
        public float updateRate = 0.5f;

        [HideInInspector]
        /// <summary>
        /// The parent chunk.
        /// </summary>
        public Chunk parent;

        private static Camera _camera = null;
        private static new Camera camera
        {
            get
            {
                if (_camera == null)
                    _camera = Camera.main;

                return _camera;
            }
        }

        private Vector3 centeredPosition
        {
            get
            {
                return transform.position + (new Vector3(parent.size, parent.size, parent.size) / 2);
            }
        }
        float timer = 0f;
        bool wasVisible = false;

        void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                UpdateChunk();
                timer = updateRate;
            }
        }

        void UpdateChunk()
        {
            if (parent == null || parent.parent == null)
            {
                Debug.LogError("ChunkManager has been created without a chunk and/or terrain, please do not manually assign ChunkManager.");
                return;
            }

            if (camera == null)
            {
                Debug.LogWarning("There is no main camera avaliable for ChunkManager");
                return;
            }

            bool visible = (parent.parent.ignoreYAxis ? Vector3.Distance(new Vector3(camera.transform.position.x, 0, camera.transform.position.z), new Vector3(centeredPosition.x, 0, centeredPosition.z)) : Vector3.Distance(camera.transform.position, centeredPosition)) <= parent.parent.renderDistance;
            bool canDispose = (parent.parent.ignoreYAxis ? Vector3.Distance(new Vector3(camera.transform.position.x, 0, camera.transform.position.z), new Vector3(centeredPosition.x, 0, centeredPosition.z)) : Vector3.Distance(camera.transform.position, centeredPosition)) > parent.parent.disposeDistance;

            parent.visible = visible;

            if (visible)
            {
                wasVisible = true;
                return;
            }

            if (!dirty)
            {
                if (!visible && wasVisible && destroyUnused && canDispose)
                {
                    parent.Delete();
                }   
            }
        }
    }
}