using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCreator : MonoBehaviour
{
    [SerializeField] int shipWidth, shipLength;
    [SerializeField] int roomWidthMin, roomLengthMin;
    [SerializeField] int corridorWidth;
    [SerializeField] int maxIterations;
    [SerializeField] Material material1;
    [SerializeField] Material material2;
    [SerializeField] Material material3;

    private Material material;

    void Start()
    {
        CreateShip();
    }

    private void CreateShip()
    {
        ShipGenerator generator = new ShipGenerator(shipWidth, shipLength);
        List<Node> listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin);
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            if ((i + 2) % 3 == 0)
            {
                material = material2;
            }
            else if ((i + 1) % 3 == 0)
            {
                material = material3;
            }
            else
            {
                material = material1;
            }
            CreateMesh(listOfRooms[i].BottomLeftCorner, listOfRooms[i].TopRightCorner);
        }
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftVertex = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightVertex = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftVertex = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightVertex = new Vector3(topRightCorner.x, 0, topRightCorner.y);
        Vector3[] vertices = new Vector3[]
        {
            topLeftVertex,
            topRightVertex,
            bottomLeftVertex,
            bottomRightVertex
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject floor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        floor.transform.position = Vector3.zero;
        floor.transform.localScale = Vector3.one;
        floor.GetComponent<MeshFilter>().mesh = mesh;
        floor.GetComponent<MeshRenderer>().material = material;
    }
}
