using UnityEngine;
using System.Collections;

namespace VoxelMaster
{
    [CreateAssetMenu]
    public class BlockDictionary : ScriptableObject
    {
        /// <summary>
        /// The block info array
        /// </summary>
        public BlockInfo[] blocksInfo;
    }
}