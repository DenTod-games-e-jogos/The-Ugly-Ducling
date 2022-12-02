using UnityEngine;

namespace VoxelMaster
{
    public class Block
    {
        public short id { get; private set; }

        public BlockInfo blockInfo { get; private set; }

        Chunk parent { get; set; }

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

        public override string ToString()
        {
            return (blockInfo != null ? blockInfo.blockName : "ERROR");
        }
    }
}