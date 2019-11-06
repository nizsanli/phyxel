using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phob : MonoBehaviour
{
    public Block[,,] blocks;

    public int xLength, yLength, zLength;

    public int blockXLength, blockYLength, blockZLength;
    public int maxBlockVolume;

    public Queue<Block> blockMeshQueue;

    public Block blockPrefab;

    public int blockMeshRate;

    public Dictionary<byte, Color> palette;

    private void Awake()
    {
        palette = BlockMesher.GetColorPalleteByte();

        while (blockXLength * blockYLength * blockZLength < maxBlockVolume
            && blockXLength * blockYLength * blockZLength < xLength * yLength * zLength)
        {
            blockXLength = System.Math.Min(xLength, blockXLength + 1);
            blockYLength = System.Math.Min(yLength, blockYLength + 1);
            blockZLength = System.Math.Min(zLength, blockZLength + 1);
        }

        blocks = new Block[
            Mathf.CeilToInt(xLength / (float)blockXLength),
            Mathf.CeilToInt(yLength / (float)blockYLength),
            Mathf.CeilToInt(zLength / (float)blockZLength)];

        blockMeshQueue = new Queue<Block>(blocks.Length + 1);

        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int z = 0; z < blocks.GetLength(2); z++)
                {
                    blockPrefab.SetResolution(blockXLength, blockYLength, blockZLength);
                    blockPrefab.transform.position = transform.position
                        - new Vector3(xLength * .5f, yLength * .5f, zLength * .5f)
                        + new Vector3(x * blockXLength, y * blockYLength, z * blockZLength);

                    Block block = Instantiate<Block>(blockPrefab, transform)
                        .FillRandom(x, y, z, xLength, yLength, zLength);

                    blocks[x, y, z] = block;

                    blockMeshQueue.Enqueue(block);
                }
            }
        }

        for (int x = 0; x < blocks.GetLength(0); x++)
        {
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int z = 0; z < blocks.GetLength(2); z++)
                {
                    Block block = blocks[x, y, z];

                    if (x > 0)
                    {
                        block.left = blocks[x - 1, y, z];
                    }
                    if (x < blocks.GetLength(0) - 1)
                    {
                        block.right = blocks[x + 1, y, z];
                    }
                    if (y > 0)
                    {
                        block.bottom = blocks[x, y - 1, z];
                    }
                    if (y < blocks.GetLength(1) - 1)
                    {
                        block.top = blocks[x, y + 1, z];
                    }
                    if (z > 0)
                    {
                        block.back = blocks[x, y, z - 1];
                    }
                    if (z < blocks.GetLength(2) - 1)
                    {
                        block.front = blocks[x, y, z + 1];
                    }
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
        int meshRate = blockMeshRate;
        while (blockMeshQueue.Count > 0 && meshRate > 0)
        {
            meshRate--;

            Block block = blockMeshQueue.Dequeue();
            BlockMesher.MeshCubeFaces(block, palette);
        }
    }
}
