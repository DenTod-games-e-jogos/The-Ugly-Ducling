using UnityEngine;

namespace VoxelMaster
{
    [RequireComponent(typeof(VoxelGeneration))]
    public abstract class BaseGeneration : MonoBehaviour
    {
        public virtual short Generation(int x, int y, int z)
        {
            return -1;
        }
    }
}