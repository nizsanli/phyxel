using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public byte[,,] voxels;

    public int xLength, yLength, zLength;
    public Block left, right, bottom, top, back, front;

    private void Awake()
    {
        voxels = new byte[xLength, yLength, zLength];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Block Fill(byte val, int total)
    {
        int t = 0;
        for (int x = 0; x < voxels.GetLength(0); x++)
        {
            for (int y = 0; y < voxels.GetLength(1); y++)
            {
                for (int z = 0; z < voxels.GetLength(2); z++)
                {
                    if (t >= total)
                        return this;

                    voxels[x, y, z] = val;
                    t++;
                }
            }
        }

        return this;
    }

    public Block FillRandom(int xBlock, int yBlock, int zBlock, int xVol, int yVol, int zVol)
    {
        for (int x = 0; x < voxels.GetLength(0) && x + xBlock * xLength < xVol; x++)
        {
            for (int y = 0; y < voxels.GetLength(1) && y + yBlock * yLength < yVol; y++)
            {
                for (int z = 0; z < voxels.GetLength(2) && z + zBlock * zLength < zVol; z++)
                {
                    voxels[x, y, z] = (byte)Random.Range(1, 256);
                }
            }
        }

        return this;
    }

    public void SetResolution(int xLen, int yLen, int zLen)
    {
        xLength = xLen;
        yLength = yLen;
        zLength = zLen;
    }
}
