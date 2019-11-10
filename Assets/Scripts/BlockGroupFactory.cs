﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGroupFactory : MonoBehaviour
{
    public Block blockPrefab;
    public BlockGroup blockGroupPrefab;

    public int maxVolume;

    public Queue<Block> blockMeshQueue;
    public int blockMeshRate;

    public Dictionary<int, List<Block>> meshChainMap;
    public Queue<Transform> blockMeshChains;

    public Transform blockMeshChainPrefab;

    private void Awake()
    {
        blockMeshQueue = new Queue<Block>();
    }

    private void Update()
    {
        int blocksLeftToMesh = blockMeshRate;
        while (blockMeshQueue.Count > 0 && blocksLeftToMesh > 0)
        {
            blocksLeftToMesh--;

            Block block = blockMeshQueue.Dequeue();

            BlockMesher.MeshCubeFaces(block);
        }
    }

    public BlockGroup CreateRectangularPrism(int xLength, int yLength, int zLength, Vector3 focusCenter, int voxelType = 1)
    {
        int blockSizeX, blockSizeY, blockSizeZ;
        blockSizeX = blockSizeY = blockSizeZ = 1;

        while (Volume(blockSizeX, blockSizeY, blockSizeZ) < maxVolume)
        {
            blockSizeX = System.Math.Min(xLength, blockSizeX * 2);
            blockSizeY = System.Math.Min(yLength, blockSizeY * 2);
            blockSizeZ = System.Math.Min(zLength, blockSizeZ * 2);
        }

        //Debug.Log(string.Join(" ", blockSizeX, blockSizeY, blockSizeZ));

        int numBlocksX, numBlocksY, numBlocksZ;
        numBlocksX = Mathf.CeilToInt(xLength / blockSizeX);
        numBlocksY = Mathf.CeilToInt(yLength / blockSizeY);
        numBlocksZ = Mathf.CeilToInt(zLength / blockSizeZ);

        blockGroupPrefab.transform.position = Vector3.zero;
        BlockGroup blockGroup = Instantiate(blockGroupPrefab, transform);
        blockGroup.AllocateBlocks(numBlocksX, numBlocksY, numBlocksZ);

        for (int xBlock = 0; xBlock < numBlocksX; xBlock++)
        {
            for (int yBlock = 0; yBlock < numBlocksY; yBlock++)
            {
                for (int zBlock = 0; zBlock < numBlocksZ; zBlock++)
                {
                    Block block = blockGroup.blocks[xBlock, yBlock, zBlock];
                    block.SetResolution(blockSizeX, blockSizeY, blockSizeZ);

                    int xCut = (xBlock + 1) * blockSizeX <= xLength ? blockSizeX : xLength - (xBlock * blockSizeX);
                    int yCut = (yBlock + 1) * blockSizeY <= yLength ? blockSizeY : yLength - (yBlock * blockSizeY);
                    int zCut = (zBlock + 1) * blockSizeZ <= zLength ? blockSizeZ : zLength - (zBlock * blockSizeZ);

                    byte[,,] voxels = block.dataLevels[0];
                    for (int x = 0; x < xCut; x++)
                    {
                        for (int y = 0; y < yCut; y++)
                        {
                            for (int z = 0; z < zCut; z++)
                            {
                                if (Random.Range(0, 10) < 5)
                                    voxels[x, y, z] = (byte)voxelType;
                            }
                        }
                    }

                    block.transform.position = Vector3.Scale(new Vector3(xBlock, yBlock, zBlock), new Vector3(blockSizeX, blockSizeY, blockSizeZ));

                    int lod = (int)Mathf.Min(
                        Vector3.Distance(block.transform.position, focusCenter) / (block.ResolutionY * 4),
                        Mathf.Log(Mathf.Max(blockSizeX, blockSizeY, blockSizeZ), 2));

                    if (lod > 0)
                    {
                        block.AddDetailLevel(lod);
                        block.levelOfDetail = (byte)lod;
                    }

                    if (!meshChainMap.ContainsKey(lod))
                    {
                        meshChainMap[lod] = new List<Block>();
                    }
                    meshChainMap[lod].Add(block);

                    // add to mesh queue
                    // blockMeshQueue.Enqueue(block);
                }
            }
        }

        foreach (var chain in meshChainMap)
        {
            int lod = chain.Key;
            List<Block> blocks = meshChainMap[lod];

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<int> triangles = new List<int>();

            int numChains = (int)(blocks.Count / Mathf.Pow(Mathf.Pow(2, lod), 3));
            
        }

        return blockGroup;
    }

    public int Volume(int xLen, int yLen, int zLen)
    {
        return xLen * yLen * zLen;
    }
}
