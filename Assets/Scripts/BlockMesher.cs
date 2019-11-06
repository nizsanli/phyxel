using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMesher
{
    static public Block currentBlock;

    public static Block MeshCubeFaces(Block block, Dictionary<byte, Color> colorMap)
    {
        currentBlock = block;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < block.voxels.GetLength(0); x++)
        {
            for (int y = 0; y < block.voxels.GetLength(1); y++)
            {
                for (int z = 0; z < block.voxels.GetLength(2); z++)
                {
                    byte val = block.voxels[x, y, z];
                    Color color = colorMap[val];

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

                    Vector3 orig = new Vector3(x, y, z);

                    // left
                    if (isMass && drawLeft)
                    {
                        DrawFace(orig + Vector3.forward, -Vector3.forward, Vector3.up, -Vector3.right, color, 
                            vertices, triangles, normals, colors);
                    }
                    // right
                    if (isMass && drawRight)
                    {
                        DrawFace(orig + Vector3.right, Vector3.forward, Vector3.up, Vector3.right, color,
                            vertices, triangles, normals, colors);
                    }
                    // bot
                    if (isMass && drawBot)
                    {
                        DrawFace(orig + Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.up, color,
                            vertices, triangles, normals, colors);
                    }
                    // top
                    if (isMass && drawTop)
                    {
                        DrawFace(orig + Vector3.up, Vector3.right, Vector3.forward, Vector3.up, color,
                            vertices, triangles, normals, colors);
                    }
                    // back
                    if (isMass && drawBack)
                    {
                        DrawFace(orig, Vector3.right, Vector3.up, -Vector3.forward, color,
                            vertices, triangles, normals, colors);
                    }
                    // front
                    if (isMass && drawFront)
                    {
                        DrawFace(orig + Vector3.forward + Vector3.right, -Vector3.right, Vector3.up, Vector3.forward, color,
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
