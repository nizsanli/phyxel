using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public byte[,,] voxels;
    public byte[,,] lodVoxels;

    public int xLength, yLength, zLength;
    public Block left, right, bottom, top, back, front;
    public byte lodMesh;

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

    public Block SetLod(byte lod)
    {
        lodMesh = lod;

        if (lod == 0)
        {
            return this;
        }

        lodVoxels = new byte[xLength / lodMesh, yLength / lodMesh, zLength / lodMesh];

        for (int x = 0; x < lodVoxels.GetLength(0); x++)
        {
            for (int y = 0; y < lodVoxels.GetLength(1); y++)
            {
                for (int z = 0; z < lodVoxels.GetLength(2); z++)
                {
                    float avgVal = 0f;
                    int tot = 0;
                    for (int lodX = 0; lodX < 1 + lodMesh; lodX++)
                    {
                        for (int lodY = 0; lodY < 1 + lodMesh; lodY++)
                        {
                            for (int lodZ = 0; lodZ < 1 + lodMesh; lodZ++)
                            {
                                byte val = voxels[x + lodX, y + lodY, z + lodZ];
                                if (val > 0)
                                {
                                    avgVal += val;
                                    tot++;
                                }
                                
                            }
                        }
                    }

                    if (tot >= lodMesh * .5f)
                    {
                        avgVal /= tot;
                        lodVoxels[x, y, z] = (byte)avgVal;
                    }
                    else
                    {
                        lodVoxels[x, y, z] = 0;
                    }
                    
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
