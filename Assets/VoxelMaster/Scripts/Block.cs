using UnityEngine;

namespace VoxelMaster
{
    public class Block
    {
        /// <summary>
        /// The block ID.
        /// </summary>
        public short id { get; private set; }
        /// <summary>
        /// The block info, usually automatically set by VoxelTerrain after a BlockDictionary lookup.
        /// </summary>
        public BlockInfo blockInfo { get; private set; }

        private Chunk parent { get; set; }

        /// <summary>
        /// Creates a block (Please create blocks by using SetBlockID() from VoxelTerrain or Chunk.
        /// </summary>
        /// <param name="chunk">The chunk</param>
        /// <param name="id">The block ID</param>
        public Block(Chunk chunk, short id = 0)
        {
            this.id = id;
            this.parent = chunk;

            if (chunk != null && chunk.parent != null)
            {
                SetBlockInfo(chunk.parent.blockDictionary);
            }
            else
            {
                Debug.LogWarning("A block has been created using 'new Block()', please create block using VoxelTerrain.SetBlockID() instead.");
            }
        }

        private void SetBlockInfo(BlockDictionary blockDictionary)
        {
            for (int i = 0; i < blockDictionary.blocksInfo.Length; i++)
            {
                BlockInfo current = blockDictionary.blocksInfo[i];
                if (id == current.id)
                {
                    this.blockInfo = current;
                    return;
                }
            }
        }

        /// <summary>
        /// Returns the block name.
        /// </summary>
        /// <returns>The block name</returns>
        public override string ToString()
        {
            return (blockInfo != null ? blockInfo.blockName : "ERROR");
        }
    }
}