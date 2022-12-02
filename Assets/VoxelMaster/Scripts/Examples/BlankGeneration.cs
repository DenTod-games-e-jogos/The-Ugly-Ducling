using VoxelMaster;

public class BlankGeneration : BaseGeneration
{
    public override short Generation(int x, int y, int z)
    {
        return -1;
    }
}