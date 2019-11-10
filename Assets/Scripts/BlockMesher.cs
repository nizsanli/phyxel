using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMesher
{
    static public Block currentBlock;
    static public Dictionary<byte, Color> colorMap;

    static List<Vector3> vertices = new List<Vector3>();
    static List<Vector3> normals = new List<Vector3>();
    static List<Color> colors = new List<Color>();
    static List<int> triangles = new List<int>();

    public static Block MeshCubeFaces(Block block)
    {
        currentBlock = block;

        vertices.Clear();
        normals.Clear();
        colors.Clear();
        triangles.Clear();

        Vector3 scl = new Vector3(
            block.ResolutionX / block.LengthX,
            block.ResolutionY / block.LengthY,
            block.ResolutionZ / block.LengthZ);

        for (int x = 0; x < block.Data.GetLength(0); x++)
        {
            for (int y = 0; y < block.Data.GetLength(1); y++)
            {
                for (int z = 0; z < block.Data.GetLength(2); z++)
                {
                    int voxelValue = block.Data[x, y, z];
                    bool voxelIsMass = voxelValue > 0;

                    Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                    Vector3 orig = Vector3.Scale(new Vector3(x, y, z), scl);
                    Vector3 forward = Vector3.Scale(Vector3.forward, scl);
                    Vector3 right = Vector3.Scale(Vector3.right, scl);
                    Vector3 up = Vector3.Scale(Vector3.up, scl);

                    bool drawLeft =
                        (CoordinateInBounds(x - 1, y, z) && block.Data[x - 1, y, z] == 0) ||
                        (!CoordinateInBounds(x - 1, y, z) && block.xMinusBlock != null && block.xMinusBlock.levelOfDetail == block.levelOfDetail && block.xMinusBlock.Data[(int)block.xMinusBlock.LengthX - 1, y, z] == 0) ||
                        (!CoordinateInBounds(x - 1, y, z) && block.xMinusBlock == null) ||
                        (!CoordinateInBounds(x - 1, y, z) && block.xMinusBlock != null && block.xMinusBlock.levelOfDetail != block.levelOfDetail);

                    bool drawRight =
                        (CoordinateInBounds(x + 1, y, z) && block.Data[x + 1, y, z] == 0) ||
                        (!CoordinateInBounds(x + 1, y, z) && block.xPlusBlock != null && block.xPlusBlock.levelOfDetail == block.levelOfDetail && block.xPlusBlock.Data[0, y, z] == 0) ||
                        (!CoordinateInBounds(x + 1, y, z) && block.xPlusBlock == null) ||
                        (!CoordinateInBounds(x + 1, y, z) && block.xPlusBlock != null && block.xPlusBlock.levelOfDetail != block.levelOfDetail);

                    bool drawBottom =
                        (CoordinateInBounds(x, y - 1, z) && block.Data[x, y - 1, z] == 0) ||
                        (!CoordinateInBounds(x, y - 1, z) && block.yMinusBlock != null && block.yMinusBlock.levelOfDetail == block.levelOfDetail && block.yMinusBlock.Data[x, (int)block.yMinusBlock.LengthY - 1, z] == 0) ||
                        (!CoordinateInBounds(x, y - 1, z) && block.yMinusBlock == null) ||
                        (!CoordinateInBounds(x, y - 1, z) && block.yMinusBlock != null && block.yMinusBlock.levelOfDetail != block.levelOfDetail);

                    bool drawTop =
                        (CoordinateInBounds(x, y + 1, z) && block.Data[x, y + 1, z] == 0) ||
                        (!CoordinateInBounds(x, y + 1, z) && block.yPlusBlock != null && block.yPlusBlock.levelOfDetail == block.levelOfDetail && block.yPlusBlock.Data[x, 0, z] == 0) ||
                        (!CoordinateInBounds(x, y + 1, z) && block.yPlusBlock == null) ||
                        (!CoordinateInBounds(x, y + 1, z) && block.yPlusBlock != null && block.yPlusBlock.levelOfDetail != block.levelOfDetail);

                    bool drawBack =
                        (CoordinateInBounds(x, y, z - 1) && block.Data[x, y, z - 1] == 0) ||
                        (!CoordinateInBounds(x, y, z - 1) && block.zMinusBlock != null && block.zMinusBlock.levelOfDetail == block.levelOfDetail && block.zMinusBlock.Data[x, y, (int)block.zMinusBlock.LengthZ - 1] == 0) ||
                        (!CoordinateInBounds(x, y, z - 1) && block.zMinusBlock == null) ||
                        (!CoordinateInBounds(x, y, z - 1) && block.zMinusBlock != null && block.zMinusBlock.levelOfDetail != block.levelOfDetail);

                    bool drawFront =
                        (CoordinateInBounds(x, y, z + 1) && block.Data[x, y, z + 1] == 0) ||
                        (!CoordinateInBounds(x, y, z + 1) && block.zPlusBlock != null && block.zPlusBlock.levelOfDetail == block.levelOfDetail && block.zPlusBlock.Data[x, y, 0] == 0) ||
                        (!CoordinateInBounds(x, y, z + 1) && block.zPlusBlock == null) ||
                        (!CoordinateInBounds(x, y, z + 1) && block.zPlusBlock != null && block.zPlusBlock.levelOfDetail != block.levelOfDetail);


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
        mesh.Optimize();

        block.GetComponent<MeshFilter>().sharedMesh = mesh;

        return block;
    }

    public static byte LastSlice(Block block, Vector3 mask, Vector3 index, int dimension, int toggle)
    {
        int dimMax = block.Data.GetLength(dimension) - 1;
        index = dimMax * mask + index;

        return block.Data[(int)index.x, (int)index.y, (int)index.z];
    }

    public static bool CoordinateInBounds(int x, int y, int z)
    {
        return
            x >= 0 && x < currentBlock.Data.GetLength(0) &&
            y >= 0 && y < currentBlock.Data.GetLength(1) &&
            z >= 0 && z < currentBlock.Data.GetLength(2);
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

    /*
    public static Dictionary<byte, Color> GetColorPalleteByte()
    {
        Dictionary<byte, Color> colorMap = new Dictionary<byte, Color>();
        for (int v = 0; v < byte.MaxValue + 1; v++)
        {
            colorMap.Add((byte)v, new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }

        return colorMap;
    }
    */
}
