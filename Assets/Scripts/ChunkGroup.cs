using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChunkGroup : MonoBehaviour
{
    public Chunk[,,] chunks;
    public MeshFilter[,,] chunkMeshes;
    public int[] chunkSizeXYZ;

    public Queue<Chunk> allocationQueue;
    public Queue<Chunk> fillQueue;
    public Queue<Chunk> renderQueue;

    public int allocateRate;
    public int fillRate;
    public int renderRate;

    public MeshFilter chunkMeshPrefab;

    protected virtual void Awake()
    {
        allocationQueue = new Queue<Chunk>();
        fillQueue = new Queue<Chunk>();
        renderQueue = new Queue<Chunk>();

        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int z = 0; z < chunks.GetLength(2); z++)
                {
                    Chunk chunk = new Chunk(x, y, z);
                    chunks[x, y, z] = chunk;

                    allocationQueue.Enqueue(chunk);
                }
            }
        }

        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                for (int z = 0; z < chunks.GetLength(2); z++)
                {
                    Chunk chunk = chunks[x, y, z];

                    // left
                    if (x > 0)
                        chunk.xMinusChunk = chunks[x - 1, y, z];
                    // right
                    if (x < chunks.GetLength(0) - 1)
                        chunk.xPlusChunk = chunks[x + 1, y, z];
                    // bottom
                    if (y > 0)
                        chunk.yMinusChunk = chunks[x, y - 1, z];
                    // top
                    if (y < chunks.GetLength(1) - 1)
                        chunk.yPlusChunk = chunks[x, y + 1, z];
                    // back
                    if (z > 0)
                        chunk.zMinusChunk = chunks[x, y, z - 1];
                    // front
                    if (z < chunks.GetLength(2) - 1)
                        chunk.zPlusChunk = chunks[x, y, z + 1];
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        int chunksLeftToAllocate = allocateRate;
        while (allocationQueue.Count > 0 && chunksLeftToAllocate > 0)
        {
            chunksLeftToAllocate--;
            Chunk chunk = allocationQueue.Dequeue();

            chunk.SetSize(chunkSizeXYZ[0], chunkSizeXYZ[1], chunkSizeXYZ[2]);
            fillQueue.Enqueue(chunk);
        }

        int chunksLeftToFill = fillRate;
        while (fillQueue.Count > 0 && chunksLeftToFill > 0)
        {
            chunksLeftToFill--;
            Chunk chunk = fillQueue.Dequeue();

            Fill(chunk);
            renderQueue.Enqueue(chunk);
        }

        int chunksLeftToRender = renderRate;
        while (renderQueue.Count > 0 && chunksLeftToRender > 0)
        {
            chunksLeftToRender--;
            Chunk chunk = renderQueue.Dequeue();

            Render(chunk);
        }
    }

    protected abstract void Fill(Chunk chunk);

    protected void Render(Chunk chunk)
    {
        Mesh mesh = ChunkCubesMesher.Mesh(chunk, this);

        MeshFilter cMesh = Instantiate<MeshFilter>(chunkMeshPrefab, transform);
        cMesh.mesh = mesh;

        chunkMeshes[chunk.indexX, chunk.indexY, chunk.indexZ] = cMesh;
    }

    public int RegisterHit(RaycastHit hitInfo, Ray shootRay, int bulletSize)
    {
        // get direction of hit in local space
        Vector3 dir = transform.InverseTransformDirection(shootRay.direction);

        // get location of hit in local space
        // need to nudge it into the chunk to get a valid index
        Vector3 loc = transform.InverseTransformPoint(hitInfo.point) + dir * .001f;

        // debug shot
        //Debug.DrawRay(loc, dir * 100f, Color.red, 100f);

        int chunkX, chunkY, chunkZ;
        float step = .1f;
        int amtDestroyed = 0;
        do
        {
            // chunk index
            // floor to int required to prevent negative decimals from being casted up (e.g. -0.5 -> 0)
            chunkX = Mathf.FloorToInt(loc.x / chunkSizeXYZ[0]);
            chunkY = Mathf.FloorToInt(loc.y / chunkSizeXYZ[1]);
            chunkZ = Mathf.FloorToInt(loc.z / chunkSizeXYZ[2]);

            // index within chunk
            int x, y, z;
            x = (int)(loc.x - (chunkX * chunkSizeXYZ[0]));
            y = (int)(loc.y - (chunkY * chunkSizeXYZ[1]));
            z = (int)(loc.z - (chunkZ * chunkSizeXYZ[2]));

            if (ChunkIndexValid(chunkX, chunkY, chunkZ) && ChunkVoxelPresent(chunks[chunkX, chunkY, chunkZ], x, y, z))
            {
                amtDestroyed += ApplyRadialDamage(chunks[chunkX, chunkY, chunkZ], bulletSize, x, y, z);
            }

            loc += dir * step;
        }
        while (amtDestroyed == 0 && ChunkIndexValid(chunkX, chunkY, chunkZ));

        return amtDestroyed;
    }

    public bool ChunkIndexValid(int x, int y, int z)
    {
        return
            x >= 0 && x < chunks.GetLength(0) &&
            y >= 0 && y < chunks.GetLength(1) &&
            z >= 0 && z < chunks.GetLength(2);
    }

    public bool ChunkVoxelPresent(Chunk chunk, int x, int y, int z) 
    {
        return chunk.typeGrid[x, y, z] > 0;
    }

    public int ApplyRadialDamage(Chunk chunk, int radius, int voxX, int voxY, int voxZ)
    {
        // figure out bottom-left-back corner and top-right-forward corner indices
        // iterate triple-nested for-loop all chunks and destroy voxels within radius

        Vector3 center = new Vector3(voxX, voxY, voxZ);
        Vector3 radialBox = Vector3.one * radius;

        Vector3 botLeftBackCorner = center - radialBox;
        Vector3 topRightForwardCorner = center + radialBox;

        int xSize = chunkSizeXYZ[0];
        int ySize = chunkSizeXYZ[1];
        int zSize = chunkSizeXYZ[2];

        int chunksLeft = Mathf.FloorToInt(botLeftBackCorner.x / xSize);
        int chunksDown = Mathf.FloorToInt(botLeftBackCorner.y / ySize);
        int chunksBack = Mathf.FloorToInt(botLeftBackCorner.z / zSize);

        int botLeftBackCornerChunkX = Math.Min(chunk.indexX - chunksLeft, 0);
        int botLeftBackCornerChunkY = Math.Min(chunk.indexY - chunksDown, 0);
        int botLeftBackCornerChunkZ = Math.Min(chunk.indexZ - chunksBack, 0);

        int chunksRight = Mathf.FloorToInt(topRightForwardCorner.x / xSize);
        int chunksUp = Mathf.FloorToInt(topRightForwardCorner.y / ySize);
        int chunksForward = Mathf.FloorToInt(topRightForwardCorner.z / zSize);

        int topRightForwardCornerChunkX = Math.Min(chunk.indexX + chunksRight, chunks.GetLength(0) - 1);
        int topRightForwardCornerChunkY = Math.Min(chunk.indexY + chunksUp, chunks.GetLength(1) - 1);
        int topRightForwardCornerChunkZ = Math.Min(chunk.indexZ + chunksForward, chunks.GetLength(2) - 1);

        // debug box corner indices
        //Vector3 blb = new Vector3(botLeftBackCornerChunkX, botLeftBackCornerChunkY, botLeftBackCornerChunkZ);
        //Vector3 trf = new Vector3(topRightForwardCornerChunkX, topRightForwardCornerChunkY, topRightForwardCornerChunkZ);
        //Debug.Log(blb + "  " + trf);

        Vector3 centerVec = new Vector3(
            chunk.indexX * xSize + voxX,
            chunk.indexY * ySize + voxY,
            chunk.indexZ * zSize + voxZ);

        int amtDestroyed = 0;

        for (int x = botLeftBackCornerChunkX; x <= topRightForwardCornerChunkX; x++)
        {
            for (int y = botLeftBackCornerChunkY; y <= topRightForwardCornerChunkY; y++)
            {
                for (int z = botLeftBackCornerChunkZ; z <= topRightForwardCornerChunkZ; z++)
                {
                    Chunk currChunk = chunks[x, y, z];

                    for (int vx = 0; vx < xSize; vx++)
                    {
                        for (int vy = 0; vy < ySize; vy++)
                        {
                            for (int vz = 0; vz < zSize; vz++)
                            {
                                Vector3 voxVec = new Vector3(
                                    x * xSize + vx,
                                    y * ySize + vy,
                                    z * zSize + vz);

                                if (Vector3.Distance(centerVec, voxVec) <= radius && ChunkVoxelPresent(currChunk, vx, vy, vz))
                                {
                                    currChunk.typeGrid[vx, vy, vz] = 0;
                                    amtDestroyed++;
                                }
                            }
                        }
                    }

                    Mesh mesh = ChunkCubesMesher.Mesh(currChunk, this);
                    chunkMeshes[x, y, z].mesh = mesh;
                }
            }
        }

        return amtDestroyed;
    }
}
