using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChunkGroup : MonoBehaviour
{
    public Chunk[,,] chunks;
    public int[] chunkSizeXYZ;

    public Queue<Chunk> allocationQueue;
    public Queue<Chunk> fillQueue;
    public Queue<Chunk> renderQueue;

    public int allocateRate;
    public int fillRate;
    public int renderRate;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int chunksLeftToAllocate = allocateRate;
        while (allocationQueue.Count > 0 && chunksLeftToAllocate > 0)
        {
            chunksLeftToAllocate--;
            Chunk chunk = allocationQueue.Dequeue();

            chunk.SetMaxResolution(chunkSizeXYZ[0], chunkSizeXYZ[1], chunkSizeXYZ[2]);
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

    protected abstract void Render(Chunk chunk);
}
