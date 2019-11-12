using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Block xMinusBlock, xPlusBlock, yMinusBlock, yPlusBlock, zMinusBlock, zPlusBlock;

    public Vector3 position;
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

    /*
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
    */

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
        int yDiv = ResolutionY / lowerYLength;
        int zDiv = ResolutionZ / lowerZLength;

        dataLevels[level] = new byte[lowerXLength, lowerYLength, lowerZLength];

        Dictionary<byte, int> counts = new Dictionary<byte, int>();

        for (int x = 0; x < lowerXLength; x++)
        {
            for (int y = 0; y < lowerYLength; y++)
            {
                for (int z = 0; z < lowerZLength; z++)
                {
                    counts.Clear();

                    int mostCommonVal = 0;
                    int mostCommonCount = int.MinValue;

                    for (int kernelX = x * xDiv; kernelX < x * xDiv + xDiv && kernelX < ResolutionX; kernelX++)
                    {
                        for (int kernelY = y * yDiv; kernelY < y * yDiv + yDiv && kernelY < ResolutionY; kernelY++)
                        {
                            for (int kernelZ = z * zDiv; kernelZ < z * zDiv + zDiv && kernelZ < ResolutionZ; kernelZ++)
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
                                }
                            }
                        }
                    }

                    dataLevels[level][x, y, z] = (byte)mostCommonVal;
                }
            }
        }
    }
}
