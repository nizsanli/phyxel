using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGroup : MonoBehaviour
{
    public Block[,,] blocks;
    public int levelOfDetail;

    public Block blockPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AllocateBlocks(int numBlocksX, int numBlocksY, int numBlocksZ)
    {
        blocks = new Block[numBlocksX, numBlocksY, numBlocksZ];

        for (int x = 0; x < numBlocksX; x++)
        {
            for (int y = 0; y < numBlocksY; y++)
            {
                for (int z = 0; z < numBlocksZ; z++)
                {
                    Block block = Instantiate<Block>(
                        blockPrefab,
                        transform);

                    blocks[x, y, z] = block;
                }
            }
        }

        // set links to adjacent blocks
        for (int x = 0; x < numBlocksX; x++)
        {
            for (int y = 0; y < numBlocksY; y++)
            {
                for (int z = 0; z < numBlocksZ; z++)
                {
                    Block block = blocks[x, y, z];

                    // left
                    if (x > 0)
                        block.xMinusBlock = blocks[x - 1, y, z];
                    // right
                    if (x < blocks.GetLength(0) - 1)
                        block.xPlusBlock = blocks[x + 1, y, z];
                    // bottom
                    if (y > 0)
                        block.yMinusBlock = blocks[x, y - 1, z];
                    // top
                    if (y < blocks.GetLength(1) - 1)
                        block.yPlusBlock = blocks[x, y + 1, z];
                    // back
                    if (z > 0)
                        block.zMinusBlock = blocks[x, y, z - 1];
                    // front
                    if (z < blocks.GetLength(2) - 1)
                        block.zPlusBlock = blocks[x, y, z + 1];
                }
            }
        }
    }
}
