using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGroupRectangularPrism : ChunkGroup
{
    public int unitsX, unitsY, unitsZ;

    private void Awake()
    {
        chunkSizeXYZ = Chunk.MaxVolumeDimensions(unitsX, unitsY, unitsZ);

        int[] numBlocksXYZ =
        {
            Mathf.CeilToInt(unitsX / chunkSizeXYZ[0]),
            Mathf.CeilToInt(unitsY / chunkSizeXYZ[1]),
            Mathf.CeilToInt(unitsZ / chunkSizeXYZ[2])
        };

        chunks = new Chunk[numBlocksXYZ[0], numBlocksXYZ[1], numBlocksXYZ[2]];

        allocationQueue = new Queue<Chunk>();
        for (int x = 0; x < numBlocksXYZ[0]; x++)
        {
            for (int y = 0; y < numBlocksXYZ[1]; y++)
            {
                for (int z = 0; z < numBlocksXYZ[2]; z++)
                {
                    Chunk chunk = new Chunk();
                    chunks[x, y, z] = chunk;

                    allocationQueue.Enqueue(chunk);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Fill(Chunk chunk)
    {
        int[] resolutionXYZ = chunk.MaxResolution;

        resolutionXYZ[0] = (int)(Mathf.Min((chunk.position.x + 1) * resolutionXYZ[0], unitsX) - (chunk.position.x + 1) * resolutionXYZ[0]);
        resolutionXYZ[1] = (int)(Mathf.Min((chunk.position.y + 1) * resolutionXYZ[1], unitsY) - (chunk.position.y + 1) * resolutionXYZ[1]);
        resolutionXYZ[2] = (int)(Mathf.Min((chunk.position.z + 1) * resolutionXYZ[2], unitsZ) - (chunk.position.z + 1) * resolutionXYZ[2]);

        for (int x = 0; x < resolutionXYZ[0]; x++)
        {
            for (int y = 0; y < resolutionXYZ[1]; y++)
            {
                for (int z = 0; x < resolutionXYZ[2]; z++)
                {
                    chunk.typeGrid[x, y, z] = 1;
                    chunk.colorGrid[x, y, z] = ushort.MaxValue;
                }
            }
        }
    }

    protected override void Render(Chunk chunk)
    {
        int[] resolutionXYZ = chunk.MaxResolution;

        

        for (int x = 0; x < resolutionXYZ[0]; x++)
        {
            for (int y = 0; y < resolutionXYZ[1]; y++)
            {
                for (int z = 0; x < resolutionXYZ[2]; z++)
                {

                }
            }
        }
    }
}
