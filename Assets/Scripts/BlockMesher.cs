﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMesher
{
    static public Block currentBlock;
    static public Vector3 scl;
    static public Dictionary<byte, Color> colorMap;

    public static Block MeshCubeFaces(Block block)
    {
        currentBlock = block;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();

        Dictionary<byte, Color> tempColorMap = new Dictionary<byte, Color>();
        
        byte[,,] data = block.voxels;
        byte[,,] tempData;
        for (int i = 0; i < block.lodMesh; i++)
        {
            tempData = new byte[data.GetLength(0) - 1, data.GetLength(1) - 1, data.GetLength(2) - 1];
            for (int x = 0; x < tempData.GetLength(0); x++)
            {
                for (int y = 0; y < tempData.GetLength(1); y++)
                {
                    for (int z = 0; z < tempData.GetLength(2); z++)
                    {
                        byte[] vals = { data[x, y, z], data[x + 1, y, z], data[x, y + 1, z], data[x + 1, y + 1, z] };
                        
                        float avgVal = ((float)vals[0] + vals[1] + vals[2] + vals[3]) / 4f;
                        Color avgCol = (colorMap[vals[0]] + colorMap[vals[1]] + colorMap[vals[2]] + colorMap[vals[3]]) / 4f;
                        tempColorMap[(byte)avgVal] = avgCol;

                        tempData[x, y, z] = (byte)avgVal;
                    }
                }
            }
            data = tempData;
        }

        scl = new Vector3(block.voxels.GetLength(0) / (float)data.GetLength(0), block.voxels.GetLength(1) / (float)data.GetLength(1), block.voxels.GetLength(2) / (float)data.GetLength(2));

        Vector3 scaledRight = Vector3.right * scl.x;
        Vector3 scaledUp = Vector3.up * scl.y;
        Vector3 scaledForward = Vector3.forward * scl.z;

        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int z = 0; z < data.GetLength(2); z++)
                {
                    byte val = data[x, y, z];
                    Color color = tempColorMap[val];
                    
                    bool isMass = block.voxels[x, y, z] > 0;
                    bool drawLeft =
                        (CoordinateInBounds(x - 1, y, z) && block.voxels[x - 1, y, z] == 0)
                        || (!CoordinateInBounds(x - 1, y, z) && block.left != null && block.left.voxels[block.left.voxels.GetLength(0) - 1, y, z] == 0)
                        || (!CoordinateInBounds(x - 1, y, z) && block.left == null);
                    bool drawRight =
                        (CoordinateInBounds(x + 1, y, z) && block.voxels[x + 1, y, z] == 0)
                        || (!CoordinateInBounds(x + 1, y, z) && block.right != null && block.right.voxels[0, y, z] == 0)
                        || (!CoordinateInBounds(x + 1, y, z) && block.right == null);
                    bool drawBot =
                        (CoordinateInBounds(x, y - 1, z) && block.voxels[x, y - 1, z] == 0)
                        || (!CoordinateInBounds(x, y - 1, z) && block.bottom != null && block.bottom.voxels[x, block.bottom.voxels.GetLength(1) - 1, z] == 0)
                        || (!CoordinateInBounds(x, y - 1, z) && block.bottom == null);
                    bool drawTop =
                        (CoordinateInBounds(x, y + 1, z) && block.voxels[x, y + 1, z] == 0)
                        || (!CoordinateInBounds(x, y + 1, z) && block.top != null && block.top.voxels[x, 0, z] == 0)
                        || (!CoordinateInBounds(x, y + 1, z) && block.top == null);
                    bool drawBack =
                        (CoordinateInBounds(x, y, z - 1) && block.voxels[x, y, z - 1] == 0)
                        || (!CoordinateInBounds(x, y, z - 1) && block.back != null && block.back.voxels[x, y, block.back.voxels.GetLength(2) - 1] == 0)
                        || (!CoordinateInBounds(x, y, z - 1) && block.back == null);
                    bool drawFront =
                        (CoordinateInBounds(x, y, z + 1) && block.voxels[x, y, z + 1] == 0)
                        || (!CoordinateInBounds(x, y, z + 1) && block.front != null && block.front.voxels[x, y, 0] == 0)
                        || (!CoordinateInBounds(x, y, z + 1) && block.front == null);

                    Vector3 orig = new Vector3(x * scl.x, y * scl.y, z * scl.z);

                    // left
                    if (isMass && drawLeft)
                    {
                        DrawFace(orig + scaledForward, -scaledForward, scaledUp, -scaledRight, color,  
                            vertices, triangles, normals, colors);
                    }
                    // right
                    if (isMass && drawRight)
                    {
                        DrawFace(orig + scaledRight, scaledForward, scaledUp, scaledRight, color,
                            vertices, triangles, normals, colors);
                    }
                    // bot
                    if (isMass && drawBot)
                    {
                        DrawFace(orig + scaledForward, scaledRight, -scaledForward, -scaledUp, color,
                            vertices, triangles, normals, colors);
                    }
                    // top
                    if (isMass && drawTop)
                    {
                        DrawFace(orig + scaledUp, scaledRight, scaledForward, scaledUp, color,
                            vertices, triangles, normals, colors);
                    }
                    // back
                    if (isMass && drawBack)
                    {
                        DrawFace(orig, scaledRight, scaledUp, -scaledForward, color,
                            vertices, triangles, normals, colors);
                    }
                    // front
                    if (isMass && drawFront)
                    {
                        DrawFace(orig + scaledForward + scaledRight, -scaledRight, scaledUp, scaledForward, color,
                            vertices, triangles, normals, colors);
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
        mesh.Optimize();

        block.GetComponent<MeshFilter>().sharedMesh = mesh;

        return block;
    }

    public static bool CoordinateInBounds(int x, int y, int z)
    {
        return
            x >= 0 && x < currentBlock.voxels.GetLength(0) &&
            y >= 0 && y < currentBlock.voxels.GetLength(1) &&
            z >= 0 && z < currentBlock.voxels.GetLength(2);
    }

    static public void DrawFace(Vector3 botLeft, Vector3 right, Vector3 up, Vector3 normal, Color color, 
        List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Color> colors)
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

    public static Dictionary<byte, Color> GetColorPalleteByte()
    {
        Dictionary<byte, Color> colorMap = new Dictionary<byte, Color>();
        for (int v = 0; v < byte.MaxValue + 1; v++)
        {
            colorMap.Add((byte)v, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }

        return colorMap;
    }
}
