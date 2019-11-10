using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Block xMinusBlock, xPlusBlock, yMinusBlock, yPlusBlock, zMinusBlock, zPlusBlock;

    public byte levelOfDetail;
    public byte[][,,] voxelLevels;

    public byte[,,] Voxels
    {
        get { return voxelLevels[levelOfDetail]; }
    }

    public Vector3 Dimensions
    {
        get { return new Vector3(Voxels.GetLength(0), Voxels.GetLength(1), Voxels.GetLength(2)); }
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AllocateHighestDetail(int blockSizeX, int blockSizeY, int blockSizeZ)
    {
        int totalLevels = (int)Mathf.Log(Mathf.Max(blockSizeX, blockSizeY, blockSizeZ), 2) + 1;
        voxelLevels = new byte[totalLevels][,,];

        levelOfDetail = 0;
        voxelLevels[levelOfDetail] = new byte[blockSizeX, blockSizeY, blockSizeZ];
    }

    public void AllocateLowerDetail()
    {
        byte[,,] higherResolutionVoxels = Voxels;
        int higherXLength = higherResolutionVoxels.GetLength(0);
        int higherYLength = higherResolutionVoxels.GetLength(1);
        int higherZLength = higherResolutionVoxels.GetLength(2);

        int lowerXLength = Mathf.Clamp(higherXLength / 2, 1, int.MaxValue);
        int lowerYLength = Mathf.Clamp(higherYLength / 2, 1, int.MaxValue);
        int lowerZLength = Mathf.Clamp(higherZLength / 2, 1, int.MaxValue);

        voxelLevels[levelOfDetail + 1] = new byte[
            lowerXLength,
            lowerYLength,
            lowerZLength];

        levelOfDetail++;

        Dictionary<byte, byte> typeCounts = new Dictionary<byte, byte>();

        for (int x = 0; x < lowerXLength; x++)
        {
            for (int y = 0; y < lowerYLength; y++)
            {
                for (int z = 0; z < lowerZLength; z++)
                {
                    typeCounts.Clear();

                    int endX = System.Math.Min(x * 2 + 2, higherXLength);
                    int endY = System.Math.Min(y * 2 + 2, higherYLength);
                    int endZ = System.Math.Min(z * 2 + 2, higherZLength);

                    for (int xx = x * 2; xx < endX; xx++)
                    {
                        for (int yy = y * 2; yy < endY; yy++)
                        {
                            for (int zz = z * 2; zz < endZ; zz++)
                            {
                                byte val = higherResolutionVoxels[xx, yy, zz];

                                if (!typeCounts.ContainsKey(val))
                                    typeCounts[val] = 1;
                                else
                                    typeCounts[val]++;
                            }
                        }
                    }

                    int mostCommonValue = -1;
                    int commonCount = int.MinValue;
                    foreach (KeyValuePair<byte, byte> pair in typeCounts)
                    {
                        if (pair.Value > commonCount)
                        {
                            mostCommonValue = pair.Key;
                            commonCount = pair.Value;
                        }
                    }

                    Voxels[x, y, z] = (byte)mostCommonValue;
                }
            }
        }
    }
}
