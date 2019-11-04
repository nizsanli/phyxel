using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public byte[,,] data;

    public Chunk left, top, right, down, front, back;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Chunk SetResolution(int size)
    {
        data = new byte[size, size, size];

        return this;
    }

    public Chunk PutAt(Vector3 loc)
    {
        transform.localPosition = loc;
        return this;
    }

    public Chunk Fill(byte val)
    {
        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int z = 0; z < data.GetLength(2); z++)
                {
                    data[x, y, z] = val;
                }
            }
        }

        return this;
    }

    public Chunk Mesh(Phobject phobject, int xLoc, int yLoc, int zLoc)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Color> colors = new List<Color>();

        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int z = 0; z < data.GetLength(2); z++)
                {
                    bool isMass = data[x, y, z] > 0;
                    bool drawLeft = (CoordInBounds(x - 1, y, z) && data[x - 1, y, z] == 0)
                        || (!CoordInBounds(x - 1, y, z) && left != null && left.data[left.data.GetLength(0) - 1, y, z] == 0)
                        || (!CoordInBounds(x - 1, y, z) && left == null);
                    bool drawRight = (CoordInBounds(x + 1, y, z) && data[x + 1, y, z] == 0)
                        || (!CoordInBounds(x + 1, y, z) && right != null && right.data[0, y, z] == 0)
                        || (!CoordInBounds(x + 1, y, z) && right == null);
                    bool drawBot = (CoordInBounds(x, y - 1, z) && data[x, y - 1, z] == 0)
                        || (!CoordInBounds(x, y - 1, z) && down != null && down.data[x, down.data.GetLength(1) - 1, z] == 0)
                        || (!CoordInBounds(x, y - 1, z) && down == null);
                    bool drawTop = (CoordInBounds(x, y + 1, z) && data[x, y + 1, z] == 0)
                        || (!CoordInBounds(x, y + 1, z) && top != null && top.data[x, 0, z] == 0)
                        || (!CoordInBounds(x, y + 1, z) && top == null);
                    bool drawBack = (CoordInBounds(x, y, z - 1) && data[x, y, z - 1] == 0)
                        || (!CoordInBounds(x, y, z - 1) && back != null && back.data[x, y, back.data.GetLength(2) - 1] == 0)
                        || (!CoordInBounds(x, y, z - 1) && back == null);
                    bool drawFront = (CoordInBounds(x, y, z + 1) && data[x, y, z + 1] == 0)
                        || (!CoordInBounds(x, y, z + 1) && front != null && front.data[x, y, 0] == 0)
                        || (!CoordInBounds(x, y, z + 1) && front == null);

                    Vector3 orig = new Vector3(x, y, z);

                    // left
                    if (isMass && drawLeft)
                    {
                        DrawFace(orig + Vector3.forward, -Vector3.forward, Vector3.up, -Vector3.right,
                            vertices, triangles, normals, colors);
                    }
                    // right
                    if (isMass && drawRight)
                    {
                        DrawFace(orig + Vector3.right, Vector3.forward, Vector3.up, Vector3.right, 
                            vertices, triangles, normals, colors);
                    }
                    // bot
                    if (isMass && drawBot)
                    {
                        DrawFace(orig + Vector3.forward, Vector3.right, -Vector3.forward, -Vector3.up, 
                            vertices, triangles, normals, colors);
                    }
                    // top
                    if (isMass && drawTop)
                    {
                        DrawFace(orig + Vector3.up, Vector3.right, Vector3.forward, Vector3.up, 
                            vertices, triangles, normals, colors);
                    }
                    // back
                    if (isMass && drawBack)
                    {
                        DrawFace(orig, Vector3.right, Vector3.up, -Vector3.forward, 
                            vertices, triangles, normals, colors);
                    }
                    // front
                    if (isMass && drawFront)
                    {
                        DrawFace(orig + Vector3.forward + Vector3.right, -Vector3.right, Vector3.up, Vector3.forward, 
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

        MeshRenderer rend = GetComponent<MeshRenderer>();
        MeshFilter filt = GetComponent<MeshFilter>();

        //mesh.Optimize();
        filt.sharedMesh = mesh;

        MeshCollider col = gameObject.AddComponent<MeshCollider>();
        col.convex = true;

        return this;
    }

    private void DrawFace(Vector3 botLeft, Vector3 right, Vector3 up, Vector3 normal, 
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

        Color randomColor = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            1f);
        colors.Add(randomColor);
        colors.Add(randomColor);
        colors.Add(randomColor);
        colors.Add(randomColor);
    }

    public bool CoordInBounds(int x, int y, int z)
    {
        return
            x >= 0 && x < data.GetLength(0) &&
            y >= 0 && y < data.GetLength(1) &&
            z >= 0 && z < data.GetLength(2);
    }
}
