using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // links to adjacent blocks
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
        int totalLevels = (int)Mathf.Log(Mathf.Min(blockSizeX, blockSizeY, blockSizeZ), 2) + 1;
        voxelLevels = new byte[totalLevels][,,];

        levelOfDetail = 0;
        voxelLevels[levelOfDetail] = new byte[blockSizeX, blockSizeY, blockSizeZ];
    }

    public void AllocateLowerDetail()
    {
        byte[,,] higherResolutionVoxels = Voxels;

        int lowerXLength = higherResolutionVoxels.GetLength(0) / 2;
        int lowerYLength = higherResolutionVoxels.GetLength(1) / 2;
        int lowerZLength = higherResolutionVoxels.GetLength(2) / 2;

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

                    for (int xx = 0; xx < 2; xx++)
                    {
                        for (int yy = 0; yy < 2; yy++)
                        {
                            for (int zz = 0; zz < 2; zz++)
                            {
                                byte val = higherResolutionVoxels[x * 2 + xx, y * 2 + yy, z * 2 + zz];

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
