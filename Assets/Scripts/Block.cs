using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // links to adjacent blocks
    public Block xMinusBlock, xPlusBlock, yMinusBlock, yPlusBlock, zMinusBlock, zPlusBlock;

    public byte levelOfDetail;
    public byte[][,,] voxelLevels;

    public byte[,,] Voxels
    {
        get { return voxelLevels[levelOfDetail]; }
    }

    public Vector3 Dimensions
    {
        get { return new Vector3(Voxels.GetLength(0), Voxels.GetLength(1), Voxels.GetLength(2)); }
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

    public void AllocateFirstLevel(int blockSizeX, int blockSizeY, int blockSizeZ)
    {
        int totalLevels = (int)Mathf.Log(Mathf.Min(blockSizeX, blockSizeY, blockSizeZ), 2);
        voxelLevels = new byte[totalLevels][,,];

        voxelLevels[0] = new byte[blockSizeX, blockSizeY, blockSizeZ];
    }
}
