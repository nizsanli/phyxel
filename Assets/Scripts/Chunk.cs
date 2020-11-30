using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public static int MAX_VOLUME = 512;

    public ushort[,,] typeGrid;
    public ushort[,,] colorGrid;
    public float[,,] isoGrid;

    public int indexX, indexY, indexZ;

    public Chunk xMinusChunk, xPlusChunk, yMinusChunk, yPlusChunk, zMinusChunk, zPlusChunk;

    public Chunk(int x, int y, int z)
    {
        indexX = x;
        indexY = y;
        indexZ = z;
    }

    #region Properties

    public int SizeX
    {
        get { return typeGrid.GetLength(0); }
    }

    public int SizeY
    {
        get { return typeGrid.GetLength(1); }
    }

    public int SizeZ
    {
        get { return typeGrid.GetLength(2); }
    }

    public int[] Size
    {
        get { return new int[] { typeGrid.GetLength(0), typeGrid.GetLength(1), typeGrid.GetLength(2) }; }
    }

    #endregion

    public void SetSize(int lengthX, int lengthY, int lengthZ)
    {
        typeGrid = new ushort[lengthX, lengthY, lengthZ];
        colorGrid = new ushort[lengthX, lengthY, lengthZ];
        isoGrid = new float[lengthX, lengthY, lengthZ];
    }

    public static int[] MaxVolumeDimensions(int boundsX, int boundsY, int boundsZ)
    {
        // x y z
        int[] sizeXYZ = { 1, 1, 1 };
        int maxVolume = System.Math.Min(MAX_VOLUME, boundsX * boundsY * boundsZ);

        while (sizeXYZ[0] * sizeXYZ[1] * sizeXYZ[2] < maxVolume)
        {
            sizeXYZ[0] = System.Math.Min(boundsX, sizeXYZ[0] * 2);
            sizeXYZ[1] = System.Math.Min(boundsY, sizeXYZ[1] * 2);
            sizeXYZ[2] = System.Math.Min(boundsZ, sizeXYZ[2] * 2);
        }

        return sizeXYZ;
    }
}
