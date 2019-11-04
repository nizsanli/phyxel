using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phobject : MonoBehaviour
{
    public Chunk[,,] chunks;
    public Chunk chunkPrefab;

    private int xLength, ylength, zLength;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Phobject SetScale(Vector3 scale)
    {
        transform.localScale = scale;
        return this;
    }

    public Phobject SetDimensions(int xDim, int yDim, int zDim, int res)
    {
        xLength = xDim; ylength = yDim; zLength = zDim;

        float r = (float)res;
        chunks = new Chunk[
            Mathf.CeilToInt(xLength / r),
            Mathf.CeilToInt(ylength / r),
            Mathf.CeilToInt(zLength / r)];


        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int z = 0; z < chunks.GetLength(2); z++)
                {
                    chunks[x, y, z] = Instantiate<Chunk>(chunkPrefab, transform)
                        .PutAt(new Vector3(
                            -xLength * .5f + x * res, -ylength * .5f + y * res, -zLength * .5f + z * res))
                        .SetResolution(res)
                        .Fill(1)
                        .Mesh();
                }
            }
        }

        return this;
    }
}

