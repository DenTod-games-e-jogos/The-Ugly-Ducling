using UnityEngine;
using System.Collections;

namespace VoxelMaster
{
    [CreateAssetMenu]
    public class BlockInfo : ScriptableObject
    {
        /// <summary>
        /// The associated block ID.
        /// </summary>
        [Space(10)]
        public short id = 0;
        /// <summary>
        /// The name of the block.
        /// </summary>
        public string blockName = "Default";
        [Tooltip("Determines if the block has some form of transparency in its texture, this is used to prevent opaque blocks from being hidden behind transparent blocks.")]
        /// <summary>
        /// Determines if the block has some form of transparency in its texture, this is used to prevent opaque blocks from being hidden behind transparent blocks.
        /// </summary>
        public bool transparent = false;
        [Tooltip("The durability of the block, this value is arbitrary and isn't used anywhere in the examples, but you can use this value as a time multiplier when you are breaking blocks.")]
        /// <summary>
        /// The durability of the block, this value is arbitrary and isn't used anywhere in the examples, but you can use this value as a time multiplier when you are breaking blocks.
        /// </summary>
        public float durability = 1.0f;
        /// <summary>
        /// The texture(s) of the block.
        /// </summary>
        [Space(10)]
        public BlockTexture blockTexture = null;

        void OnValidate()
        {
            id = (short)Mathf.Clamp(id, 0, short.MaxValue);
            durability = Mathf.Clamp(durability, 0, Mathf.Infinity);
        }
    }
}