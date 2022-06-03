using UnityEngine;
using System.Collections;

namespace VoxelMaster
{
    [System.Serializable]
    public class BlockTexture
    {
        /// <summary>
        /// The texture at the front of the block.
        /// </summary>
        public int front;
        /// <summary>
        /// The texture at the left of the block.
        /// </summary>
        public int left;
        /// <summary>
        /// The texture at the back of the block.
        /// </summary>
        public int back;
        /// <summary>
        /// The texture at the right of the block.
        /// </summary>
        public int right;
        /// <summary>
        /// The texture at the top of the block.
        /// </summary>
        public int top;
        /// <summary>
        /// The texture at the bottom of the block.
        /// </summary>
        public int bottom;
    }
}