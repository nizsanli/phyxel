using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Block xMinusBlock, xPlusBlock, yMinusBlock, yPlusBlock, zMinusBlock, zPlusBlock;

    public byte levelOfDetail;
    public byte[][,,] dataLevels;

    public byte[,,] Data
    {
        get { return dataLevels[levelOfDetail]; }
    }

    public int LengthX
    {
        get { return Data.GetLength(0); }
    }
    public int LengthY
    {
        get { return Data.GetLength(1); }
    }
    public int LengthZ
    {
        get { return Data.GetLength(2); }
    }

    public int ResolutionX
    {
        get { return dataLevels[0].GetLength(0); }
    }

    public int ResolutionY
    {
        get { return dataLevels[0].GetLength(1); }
    }

    public int ResolutionZ
    {
        get { return dataLevels[0].GetLength(2); }
    }

    public byte[,,] HighestResolutionData
    {
        get { return dataLevels[0]; }
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

    public void SetResolution(int xLength, int yLength, int zLength)
    {
        int totalLevels = (int)Mathf.Log(Mathf.Max(xLength, yLength, zLength), 2) + 1;
        dataLevels = new byte[totalLevels][,,];

        dataLevels[0] = new byte[xLength, yLength, zLength];
    }

    public void AddDetailLevel(int level)
    {
        if (dataLevels[level] != null)
            throw new System.Exception("Level " + level + " already allocated");

        int div = (int)Mathf.Pow(2, level);
        int lowerXLength = Mathf.Clamp(ResolutionX / div, 1, ResolutionX);
        int lowerYLength = Mathf.Clamp(ResolutionY / div, 1, ResolutionY);
        int lowerZLength = Mathf.Clamp(ResolutionZ / div, 1, ResolutionZ);

        int xDiv = ResolutionX / lowerXLength;
        int yDiv = ResolutionX / lowerXLength;
        int zDiv = ResolutionX / lowerXLength;

        dataLevels[level] = new byte[lowerXLength, lowerYLength, lowerZLength];

        Dictionary<byte, byte> counts = new Dictionary<byte, byte>();

        for (int x = 0; x < lowerXLength; x++)
        {
            for (int y = 0; y < lowerYLength; y++)
            {
                for (int z = 0; z < lowerZLength; z++)
                {
                    counts.Clear();

                    int mostCommonVal = 0;
                    int mostCommonCount = int.MinValue;

                    for (int kernelX = x * xDiv; kernelX < x * xDiv + xDiv; kernelX++)
                    {
                        for (int kernelY = y * yDiv; kernelY < y * yDiv + yDiv; kernelY++)
                        {
                            for (int kernelZ = z * zDiv; kernelZ < z * zDiv + zDiv; kernelZ++)
                            {
                                byte val = dataLevels[0][kernelX, kernelY, kernelZ];

                                if (!counts.ContainsKey(val))
                                {
                                    counts[val] = 0;
                                }

                                counts[val]++;

                                if (counts[val] > mostCommonCount)
                                {
                                    mostCommonVal = val;
                                    mostCommonCount = counts[val];
                                }
                            }
                        }
                    }

                    dataLevels[level][x, y, z] = (byte)mostCommonVal;
                }
            }
        }
    }

    /*
    public void AllocateHighestDetail(int blockSizeX, int blockSizeY, int blockSizeZ)
    {
        int totalLevels = (int)Mathf.Log(Mathf.Max(blockSizeX, blockSizeY, blockSizeZ), 2) + 1;
        dataLevels = new byte[totalLevels][,,];

        levelOfDetail = 0;
        dataLevels[levelOfDetail] = new byte[blockSizeX, blockSizeY, blockSizeZ];
    }

    public void AllocateLowerDetail()
    {
        byte[,,] higherResolutionVoxels = Data;
        int higherXLength = higherResolutionVoxels.GetLength(0);
        int higherYLength = higherResolutionVoxels.GetLength(1);
        int higherZLength = higherResolutionVoxels.GetLength(2);

        int lowerXLength = Mathf.Clamp(higherXLength / 2, 1, int.MaxValue);
        int lowerYLength = Mathf.Clamp(higherYLength / 2, 1, int.MaxValue);
        int lowerZLength = Mathf.Clamp(higherZLength / 2, 1, int.MaxValue);

        dataLevels[levelOfDetail + 1] = new byte[
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

                    Data[x, y, z] = (byte)mostCommonValue;
                }
            }
        }
    }
    */
}
