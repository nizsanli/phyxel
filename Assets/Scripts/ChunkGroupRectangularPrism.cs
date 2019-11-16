using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGroupRectangularPrism : ChunkGroup
{
    public int unitsX, unitsY, unitsZ;

    protected override void Awake()
    {
        chunkSizeXYZ = Chunk.MaxVolumeDimensions(unitsX, unitsY, unitsZ);

        int[] numChunksXYZ =
        {
            Mathf.CeilToInt((float)unitsX / chunkSizeXYZ[0]),
            Mathf.CeilToInt((float)unitsY / chunkSizeXYZ[1]),
            Mathf.CeilToInt((float)unitsZ / chunkSizeXYZ[2])
        };

        chunks = new Chunk[numChunksXYZ[0], numChunksXYZ[1], numChunksXYZ[2]];

        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void Fill(Chunk chunk)
    {
        int[] resolutionXYZ = chunk.Size;

        resolutionXYZ[0] = (int)(Mathf.Min((chunk.indexX + 1) * resolutionXYZ[0], unitsX) - chunk.indexX * resolutionXYZ[0]);
        resolutionXYZ[1] = (int)(Mathf.Min((chunk.indexY + 1) * resolutionXYZ[1], unitsY) - chunk.indexY * resolutionXYZ[1]);
        resolutionXYZ[2] = (int)(Mathf.Min((chunk.indexZ + 1) * resolutionXYZ[2], unitsZ) - chunk.indexZ * resolutionXYZ[2]);

        for (int x = 0; x < resolutionXYZ[0]; x++)
        {
            for (int y = 0; y < resolutionXYZ[1]; y++)
            {
                for (int z = 0; z < resolutionXYZ[2]; z++)
                {
                    chunk.typeGrid[x, y, z] = 1;
                    chunk.colorGrid[x, y, z] = ushort.MaxValue;
                }
            }
        }
    }
}
