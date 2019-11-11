using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMeshChain : MonoBehaviour
{
    public List<Block> blocks;

    public void AddBlock(Block block)
    {
        if (blocks == null)
            blocks = new List<Block>();

        blocks.Add(block);
    }

    public void MeshBlocks()
    {
        foreach (Block block in blocks)
        {
            BlockMesher.MeshCubeFaces(block);
        }

        Mesh mesh = new Mesh();
        BlockMesher.FillMesh(mesh);
        BlockMesher.Clear();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
