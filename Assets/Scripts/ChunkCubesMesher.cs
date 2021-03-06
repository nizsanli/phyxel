﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkCubesMesher
{
    static readonly List<Vector3> vertices = new List<Vector3>();
    static List<Vector3> normals = new List<Vector3>();
    static List<Color> colors = new List<Color>();
    static List<int> triangles = new List<int>();

    public static Chunk currentChunk;

    public static Mesh Mesh(Chunk chunk, ChunkGroup chunkGroup)
    {
        currentChunk = chunk;

        Clear();

        /*
        Vector3 scl = new Vector3(
            chunk.ResolutionX / chunk.LengthX,
            chunk.ResolutionY / chunk.LengthY,
            chunk.ResolutionZ / chunk.LengthZ);
        */

        for (int x = 0; x < chunk.typeGrid.GetLength(0); x++)
        {
            for (int y = 0; y < chunk.typeGrid.GetLength(1); y++)
            {
                for (int z = 0; z < chunk.typeGrid.GetLength(2); z++)
                {
                    int voxelValue = chunk.typeGrid[x, y, z];
                    bool voxelIsMass = voxelValue > 0;

                    //Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                    ushort colorVal = chunk.colorGrid[x, y, z];
                    ushort mask = 0xf;
                    int r, g, b, a;
                    r = (colorVal >> 12) & mask;
                    g = (colorVal >> 8) & mask;
                    b = (colorVal >> 4) & mask;
                    a = (colorVal) & mask;
                    Color color = new Color(r / 15f, g / 15f, b / 15f, a / 15f);

                    Vector3 off = new Vector3(x, y, z);

                    Vector3 orig = new Vector3(chunk.indexX * chunk.SizeX, chunk.indexY * chunk.SizeY, chunk.indexZ * chunk.SizeZ) + off;
                    Vector3 forward = Vector3.forward;
                    Vector3 right = Vector3.right;
                    Vector3 up = Vector3.up;
                    
                    bool drawLeft =
                        (CoordinateInBounds(x - 1, y, z) && chunk.typeGrid[x - 1, y, z] == 0) ||
                        (!CoordinateInBounds(x - 1, y, z));

                    bool drawRight =
                        (CoordinateInBounds(x + 1, y, z) && chunk.typeGrid[x + 1, y, z] == 0) ||
                        (!CoordinateInBounds(x + 1, y, z));

                    bool drawBottom =
                        (CoordinateInBounds(x, y - 1, z) && chunk.typeGrid[x, y - 1, z] == 0) ||
                        (!CoordinateInBounds(x, y - 1, z));

                    bool drawTop =
                        (CoordinateInBounds(x, y + 1, z) && chunk.typeGrid[x, y + 1, z] == 0) ||
                        (!CoordinateInBounds(x, y + 1, z));

                    bool drawBack =
                        (CoordinateInBounds(x, y, z - 1) && chunk.typeGrid[x, y, z - 1] == 0) ||
                        (!CoordinateInBounds(x, y, z - 1));

                    bool drawFront =
                        (CoordinateInBounds(x, y, z + 1) && chunk.typeGrid[x, y, z + 1] == 0) ||
                        (!CoordinateInBounds(x, y, z + 1));
                    

                    // left
                    if (voxelIsMass && drawLeft)
                    {
                        DrawFace(orig + forward, -forward, up, -right, color);
                    }
                    // right
                    if (voxelIsMass && drawRight)
                    {
                        DrawFace(orig + right, forward, up, right, color);
                    }
                    // bottom
                    if (voxelIsMass && drawBottom)
                    {
                        DrawFace(orig + right, -right, forward, -up, color);
                    }
                    // top
                    if (voxelIsMass && drawTop)
                    {
                        DrawFace(orig + up, right, forward, up, color);
                    }
                    // back
                    if (voxelIsMass && drawBack)
                    {
                        DrawFace(orig, right, up, -forward, color);
                    }
                    // front
                    if (voxelIsMass && drawFront)
                    {
                        DrawFace(orig + forward + right, -right, up, -forward, color);
                    }
                }
            }
        }

        Mesh mesh = new Mesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateBounds();
        //mesh.Optimize();

        return mesh;
    }

    public static bool CoordinateInBounds(int x, int y, int z)
    {
        return
            x >= 0 && x < currentChunk.typeGrid.GetLength(0) &&
            y >= 0 && y < currentChunk.typeGrid.GetLength(1) &&
            z >= 0 && z < currentChunk.typeGrid.GetLength(2);
    }

    static public void DrawFace(Vector3 botLeft, Vector3 right, Vector3 up, Vector3 normal, Color color)
    {
        int count = vertices.Count;

        vertices.Add(botLeft);
        vertices.Add(botLeft + up);
        vertices.Add(botLeft + up + right);
        vertices.Add(botLeft + right);

        triangles.Add(count);
        triangles.Add(count + 1);
        triangles.Add(count + 2);
        triangles.Add(count);
        triangles.Add(count + 2);
        triangles.Add(count + 3);

        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    public static void Clear()
    {
        vertices.Clear();
        normals.Clear();
        colors.Clear();
        triangles.Clear();
    }
}
