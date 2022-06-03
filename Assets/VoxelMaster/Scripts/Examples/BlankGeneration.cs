// Use this script as a template for your custom generation scripts.
// Generation scripts like these no longer assign themselves the VoxelGeneration component, it is the VoxelGeneration component that automatically gets the script thanks to polymorphism!

using UnityEngine;
using VoxelMaster; // You must import this

public class BlankGeneration : BaseGeneration // Don't forget to inherit "BaseGeneration"
{
    public override short Generation(int x, int y, int z) // Don't forget "override"
    {
        // Write your generation code here, it must return a block ID (short)
        return -1; // This technically returns nothing, consider it as air.
    }
}