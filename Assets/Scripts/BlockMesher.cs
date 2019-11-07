using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMesher
{
    static public Block currentBlock;
    static public Dictionary<byte, Color> colorMap;

    public static Block MeshCubeFaces(Block block)
    {
        currentBlock = block;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();

        
        Dictionary<byte, Color> tempColorMap = new Dictionary<byte, Color>();
        /*
        for (int x = 0; x < block.lodVoxels.GetLength(0); x++)
        {
            for (int y = 0; y < block.lodVoxels.GetLength(1); y++)
            {
                for (int z = 0; z < block.lodVoxels.GetLength(2); z++)
                {
                    Color avgCol = Color.clear;
                    for (int lodX = 0; lodX < 1 + block.lodMesh; lodX++)
                    {
                        for (int lodY = 0; lodY < 1 + block.lodMesh; lodY++)
                        {
                            for (int lodZ = 0; lodZ < 1 + block.lodMesh; lodZ++)
                            {
                                byte val = block.voxels[x + lodX, y + lodY, z + lodZ];

                                if (val > 0)
                                {
                                    avgVal += val;
                                    avgCol += colorMap[val];
                                    tot++;
                                }
                            }
                        }
                    }
                    avgVal /= tot;
                    avgCol /= tot;
                    
                    tempColorMap[(byte)avgVal] = avgCol;
                    block.lodVoxels[x, y, z] = (byte)avgVal;
                }
            }
        }
        colorMap = tempColorMap;
        */
        Vector3 scl = new Vector3(
            block.voxels.GetLength(0) / (float)block.lodVoxels.GetLength(0),
            block.voxels.GetLength(1) / (float)block.lodVoxels.GetLength(1),
            block.voxels.GetLength(2) / (float)block.lodVoxels.GetLength(2));

        Vector3 scaledRight = Vector3.right * scl.x;
        Vector3 scaledUp = Vector3.up * scl.y;
        Vector3 scaledForward = Vector3.forward * scl.z;

        for (int x = 0; x < block.lodVoxels.GetLength(0); x++)
        {
            for (int y = 0; y < block.lodVoxels.GetLength(1); y++)
            {
                for (int z = 0; z < block.lodVoxels.GetLength(2); z++)
                {
                    byte val = block.lodVoxels[x, y, z];
                    Color color = colorMap[val];
                    
                    bool isMass = block.lodVoxels[x, y, z] > 0;
                    bool drawLeft =
                        (CoordinateInBounds(x - 1, y, z) && block.lodVoxels[x - 1, y, z] == 0)
                        || (!CoordinateInBounds(x - 1, y, z) && block.left != null && block.left.lodVoxels[block.left.lodVoxels.GetLength(0) - 1, y, z] == 0)
                        || (!CoordinateInBounds(x - 1, y, z) && block.left == null);
                    bool drawRight =
                        (CoordinateInBounds(x + 1, y, z) && block.lodVoxels[x + 1, y, z] == 0)
                        || (!CoordinateInBounds(x + 1, y, z) && block.right != null && block.right.lodVoxels[0, y, z] == 0)
                        || (!CoordinateInBounds(x + 1, y, z) && block.right == null);
                    bool drawBot =
                        (CoordinateInBounds(x, y - 1, z) && block.lodVoxels[x, y - 1, z] == 0)
                        || (!CoordinateInBounds(x, y - 1, z) && block.bottom != null && block.bottom.lodVoxels[x, block.bottom.lodVoxels.GetLength(1) - 1, z] == 0)
                        || (!CoordinateInBounds(x, y - 1, z) && block.bottom == null);
                    bool drawTop =
                        (CoordinateInBounds(x, y + 1, z) && block.lodVoxels[x, y + 1, z] == 0)
                        || (!CoordinateInBounds(x, y + 1, z) && block.top != null && block.top.lodVoxels[x, 0, z] == 0)
                        || (!CoordinateInBounds(x, y + 1, z) && block.top == null);
                    bool drawBack =
                        (CoordinateInBounds(x, y, z - 1) && block.lodVoxels[x, y, z - 1] == 0)
                        || (!CoordinateInBounds(x, y, z - 1) && block.back != null && block.back.lodVoxels[x, y, block.back.lodVoxels.GetLength(2) - 1] == 0)
                        || (!CoordinateInBounds(x, y, z - 1) && block.back == null);
                    bool drawFront =
                        (CoordinateInBounds(x, y, z + 1) && block.lodVoxels[x, y, z + 1] == 0)
                        || (!CoordinateInBounds(x, y, z + 1) && block.front != null && block.front.lodVoxels[x, y, 0] == 0)
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
            x >= 0 && x < currentBlock.lodVoxels.GetLength(0) &&
            y >= 0 && y < currentBlock.lodVoxels.GetLength(1) &&
            z >= 0 && z < currentBlock.lodVoxels.GetLength(2);
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
