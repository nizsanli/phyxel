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

    public void RegisterHit(RaycastHit hitInfo, Ray shootRay)
    {
        Vector3 dir = transform.InverseTransformDirection(shootRay.direction);
        Vector3 loc = transform.InverseTransformPoint(hitInfo.point + shootRay.direction * .0001f);

        Chunk impactedChunk;
        MeshFilter cMesh;

        bool foundHit = false;

        do
        {
            // chunk index
            int chunkX, chunkY, chunkZ;
            chunkX = (int)(loc.x / chunkSizeXYZ[0]);
            chunkY = (int)(loc.y / chunkSizeXYZ[1]);
            chunkZ = (int)(loc.z / chunkSizeXYZ[2]);

            impactedChunk = chunks[chunkX, chunkY, chunkZ];
            cMesh = chunkMeshes[chunkX, chunkY, chunkZ];

            // index within chunk
            int x, y, z;
            x = (int)(loc.x - (chunkX * chunkSizeXYZ[0]));
            y = (int)(loc.y - (chunkY * chunkSizeXYZ[1]));
            z = (int)(loc.z - (chunkZ * chunkSizeXYZ[2]));

            if (impactedChunk.typeGrid[x, y, z] > 0)
            {
                impactedChunk.typeGrid[x, y, z] = 0;

                foundHit = true;
            }

            loc += dir * .1f;
        }
        while (!foundHit && ContainsPoint(loc));

        Mesh mesh = ChunkCubesMesher.Mesh(impactedChunk, this);
        cMesh.mesh = mesh;
    }

    public bool ContainsPoint(Vector3 pos)
    {
        return
            pos.x >= 0 && pos.x < chunks.GetLength(0) * chunkSizeXYZ[0] &&
            pos.y >= 0 && pos.y < chunks.GetLength(1) * chunkSizeXYZ[1] &&
            pos.z >= 0 && pos.z < chunks.GetLength(2) * chunkSizeXYZ[2];
    }
}
