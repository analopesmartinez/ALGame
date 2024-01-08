using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor.Overlays;
using UnityEngine;
using Unity.AI.Navigation;

public class ShipCreator : MonoBehaviour
{
    [SerializeField] int shipWidth, shipLength, shipHeight;
    [SerializeField] int roomWidthMin, roomLengthMin;
    [SerializeField] int corridorWidth;
    [SerializeField] int maxIterations;
    [SerializeField][Range(0.0f, 0.3f)] float roomBottomCornerModifier;
    [SerializeField][Range(0.7f, 1.0f)] float roomTopCornerModifier;
    [SerializeField][Range(0, 3)] int roomOffset;
    [SerializeField] Material floorMat;
    [SerializeField] Material ceilingMat;
    // [SerializeField] Material material3;
    [SerializeField] GameObject wallHorizontal, wallVertical, doorPrefab, lightPrefab;
    [SerializeField] NavMeshSurface _navMeshSurface;
    [SerializeField] GameObject playerPrefab, enemyPrefab;
    [SerializeField] int enemyCount;
    [SerializeField] GameObject[] premadeRooms;
    [SerializeField] int premadeRoomOffset;

    private List<Vector3> possibleDoorVerticalPositions;
    private List<Vector3> possibleDoorHorizontalPositions;
    private List<Vector4> wallVerticalPositions;
    private List<Vector4> wallHorizontalPositions;

    private List<RoomNode> premadeRoomNodes;

    private Material material;
    private GameObject player;

    void Start()
    {
        //CreateShip();
    }

    public void CreateShip()
    {
        DestroyShip();
        premadeRoomNodes = new List<RoomNode>();
        ShipGenerator generator = new ShipGenerator(shipWidth, shipLength);
        List<Node> listOfRooms = generator.CalculateShip(maxIterations, roomWidthMin, roomLengthMin, roomBottomCornerModifier, roomTopCornerModifier, roomOffset, corridorWidth, premadeRooms, premadeRoomOffset, out premadeRoomNodes);

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorVerticalPositions = new List<Vector3>();
        possibleDoorHorizontalPositions = new List<Vector3>();
        wallVerticalPositions = new List<Vector4>();
        wallHorizontalPositions = new List<Vector4>();
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            /*if ((i + 2) % 3 == 0)
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
            }*/
            material = floorMat;
            CreateMesh(listOfRooms[i].BottomLeftCorner, listOfRooms[i].TopRightCorner);
            AddLight(listOfRooms[i].BottomLeftCorner, listOfRooms[i].TopRightCorner);
        }
        CreateWalls(wallParent);
        AddDoors(wallParent);
        CreatePremadeRooms(wallParent);
        _navMeshSurface.BuildNavMesh();
        CreatePlayer(listOfRooms);
        CreateEnemy(listOfRooms, enemyCount, wallParent);
    }

    private void CreatePremadeRooms(GameObject parent)
    {
        int i = 0;
        foreach (RoomNode room in premadeRoomNodes)
        {
            Vector3 location = new Vector3((room.BottomLeftCorner.x + room.TopRightCorner.x) / 2.0f, 0, (room.BottomLeftCorner.y + room.TopRightCorner.y) / 2.0f);
            Instantiate(premadeRooms[i], location, Quaternion.identity, parent.transform);
            i++;
        }
    }

    private void CreateEnemy(List<Node> listOfRooms, int count, GameObject parent)
    {
        Transform[] patrolPoints = new Transform[listOfRooms.Count];
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            GameObject patrolPoint = new GameObject("Patrol Point");
            patrolPoint.transform.position = new Vector3((listOfRooms[i].BottomLeftCorner.x + listOfRooms[i].TopRightCorner.x) / 2.0f, 0f, (listOfRooms[i].BottomLeftCorner.y + listOfRooms[i].TopRightCorner.y) / 2.0f);
            patrolPoint.transform.SetParent(parent.transform);
            patrolPoints[i] = patrolPoint.transform;
        }

        for (int i = 0; i < count; i++)
        {
            Node room = listOfRooms[Random.Range(0, listOfRooms.Count)];
            Vector2 spawnLocation = (Vector2)(room.BottomLeftCorner + room.TopRightCorner) / 2.0f;
            GameObject enemy = Instantiate(enemyPrefab, new Vector3(spawnLocation.x, 0.5f, spawnLocation.y), Quaternion.identity, parent.transform);
            enemy.GetComponent<EnemyAI>().SetPatrolPoints(patrolPoints);
        }
    }

    private void CreatePlayer(List<Node> listOfRooms)
    {
        Node room = listOfRooms[Random.Range(0, listOfRooms.Count)];
        Vector2 spawnLocation = (Vector2)(room.BottomLeftCorner + room.TopRightCorner) / 2.0f;
        player = Instantiate(playerPrefab, new Vector3(spawnLocation.x, 1.0f, spawnLocation.y), Quaternion.identity);
    }

    private void AddLight(Vector2 point1, Vector2 point2)
    {
        Vector2 spawnLocation = (point1 + point2) / 2.0f;
        Instantiate(lightPrefab, new Vector3(spawnLocation.x, shipHeight, spawnLocation.y), Quaternion.Euler(90f, 0f, 0f), transform);
    }

    private void AddDoors(GameObject wallParent)
    {
        List<Vector3> horizontalPositions = RemoveSimilarPositions(possibleDoorHorizontalPositions);
        List<Vector3> verticalPositions = RemoveSimilarPositions(possibleDoorVerticalPositions);
        foreach (var doorPosition in verticalPositions)
        {
            Instantiate(doorPrefab, doorPosition, Quaternion.Euler(0, 90, 0), wallParent.transform);
        }
        foreach (var doorPosition in horizontalPositions)
        {
            Instantiate(doorPrefab, doorPosition, Quaternion.identity, wallParent.transform);
        }
    }

    private List<Vector3> RemoveSimilarPositions(List<Vector3> possitions)
    {
        List<Vector3> filteredList = new List<Vector3>();
        int greaterCount = 0;
        int lesserCount = 0;

        for (int i = 0; i < possitions.Count - 1; i++)
        {
            Vector3 currentPosition = possitions[i];
            foreach (Vector3 pos in possitions)
            {
                if ((currentPosition.x > pos.x && currentPosition.x <= pos.x + 2 && currentPosition.y == pos.y && currentPosition.z == pos.z) || (currentPosition.x == pos.x && currentPosition.y == pos.y && currentPosition.z > pos.z && currentPosition.z <= pos.z + 2))
                {
                    greaterCount++;
                }
                else if ((currentPosition.x < pos.x && currentPosition.x >= pos.x - 2 && currentPosition.y == pos.y && currentPosition.z == pos.z) || (currentPosition.x == pos.x && currentPosition.y == pos.y && currentPosition.z < pos.z && currentPosition.z >= pos.z - 2))
                {
                    lesserCount++;
                }
            }
            if ((greaterCount == 1 && lesserCount == 1) || (greaterCount == 0 && lesserCount == 0))
            {
                filteredList.Add(currentPosition);
            }
            greaterCount = 0;
            lesserCount = 0;
        }

        return filteredList;
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wall in wallHorizontalPositions)
        {
            Vector3 wallPosition = new Vector3(wall.x, wall.y, wall.z);
            CreateWall(wallParent, wallPosition, wallHorizontal, new Vector3(wall.w, 1f, 1f));
        }
        foreach (var wall in wallVerticalPositions)
        {
            Vector3 wallPosition = new Vector3(wall.x, wall.y, wall.z);
            CreateWall(wallParent, wallPosition, wallVertical, new Vector3(1f, 1f, wall.w));
        }
    }

    private void CreateWall(GameObject wallParent, Vector3 wallPosition, GameObject wallPrefab, Vector3 scale) //add wall size here as a vector 3
    {
        GameObject wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
        wall.transform.localScale = Vector3.Scale(wall.transform.localScale, scale);
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
        mesh.RecalculateNormals();

        GameObject floor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        floor.transform.position = Vector3.zero;
        floor.transform.localScale = Vector3.one;
        floor.GetComponent<MeshFilter>().mesh = mesh;
        floor.GetComponent<MeshRenderer>().material = floorMat;
        floor.transform.parent = transform;
        MeshCollider floorCollider = floor.AddComponent<MeshCollider>();
        floorCollider.sharedMesh = mesh;

        GameObject ceiling = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        Mesh ceilingMesh = FlipMesh(mesh);
        ceiling.transform.position = new Vector3(0, shipHeight, 0);
        ceiling.transform.localScale = Vector3.one;
        ceiling.GetComponent<MeshFilter>().mesh = ceilingMesh;
        ceiling.GetComponent<MeshRenderer>().material = ceilingMat;
        ceiling.transform.parent = transform;
        MeshCollider ceilingCollider = ceiling.AddComponent<MeshCollider>();
        ceilingCollider.sharedMesh = ceilingMesh;

        //Horizontal walls
        Vector3 wallPosition = new Vector3((bottomLeftVertex.x + bottomRightVertex.x) / 2.0f, 0f, bottomLeftVertex.z);
        float scale = bottomRightVertex.x - bottomLeftVertex.x;
        AddWallPositionToList(wallPosition, wallHorizontalPositions, possibleDoorHorizontalPositions, scale, false);
        wallPosition = new Vector3((topLeftVertex.x + topRightVertex.x) / 2.0f, 0f, topLeftVertex.z);
        scale = topRightVertex.x - topLeftVertex.x;
        AddWallPositionToList(wallPosition, wallHorizontalPositions, possibleDoorHorizontalPositions, scale, false);
        //Vectical Walls
        wallPosition = new Vector3(bottomLeftVertex.x, 0f, (bottomLeftVertex.z + topLeftVertex.z) / 2.0f);
        scale = topLeftVertex.z - bottomLeftVertex.z;
        AddWallPositionToList(wallPosition, wallVerticalPositions, possibleDoorVerticalPositions, scale, true);
        wallPosition = new Vector3(bottomRightVertex.x, 0f, (bottomRightVertex.z + topRightVertex.z) / 2.0f);
        scale = topRightVertex.z - bottomRightVertex.z;
        AddWallPositionToList(wallPosition, wallVerticalPositions, possibleDoorVerticalPositions, scale, true);


        /*
        for (int row = (int)bottomLeftVertex.x; row < (int)bottomRightVertex.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftVertex.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPositions, possibleDoorHorizontalPositions); //addd individual wall possitions
        }
        for (int row = (int)topLeftVertex.x; row < (int)topRightVertex.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightVertex.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPositions, possibleDoorHorizontalPositions);
        }
        for (int col = (int)bottomLeftVertex.z; col < (int)topLeftVertex.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftVertex.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPositions, possibleDoorVerticalPositions);
        }
        for (int col = (int)bottomRightVertex.z; col < (int)topRightVertex.z; col++)
        {
            var wallPosition = new Vector3(bottomRightVertex.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPositions, possibleDoorVerticalPositions);
        }*/
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector4> wallList, List<Vector3> doorList, float scale, bool isVertical)
    {
        Vector4 wallToAdd = new Vector4(wallPosition.x, wallPosition.y, wallPosition.z, scale);

        bool duplicateFound = false;
        Vector4 wallToSplit = new Vector4();
        Vector4 splitLocation = new Vector4();
        Vector4 wallToRemove = new Vector4();
        foreach (var wall in wallList)
        {
            if (wall.w == 3.0f || wallToAdd.w == 3.0f)
            {
                if (wall.x == wallToAdd.x &&
                    ((wallToAdd.z < wall.z + (wall.w / 2.0f) && (wallToAdd.z > wall.z - (wall.w / 2.0f))) ||
                    (wall.z < wallToAdd.z + (wallToAdd.w / 2.0f) && wall.z > wallToAdd.z - (wallToAdd.w / 2.0f))))
                {
                    if (wall.w > wallToAdd.w)
                    {
                        wallToSplit = wall;
                        splitLocation = wallToAdd;
                    }
                    else
                    {
                        wallToSplit = wallToAdd;
                        splitLocation = wall;
                    }
                    duplicateFound = true;
                    wallToRemove = wall;
                    break;
                }
                else if (wall.z == wallToAdd.z &&
                    ((wallToAdd.x < wall.x + (wall.w / 2.0f) && (wallToAdd.x > wall.x - (wall.w / 2.0f))) ||
                    (wall.x < wallToAdd.x + (wallToAdd.w / 2.0f) && wall.x > wallToAdd.x - (wallToAdd.w / 2.0f))))
                {
                    if (wallToAdd.w == 3.0f && wallToAdd.w <= wall.w)
                    {
                        wallToSplit = wall;
                        splitLocation = wallToAdd;
                    }
                    else
                    {
                        wallToSplit = wallToAdd;
                        splitLocation = wall;
                    }
                    duplicateFound = true;
                    wallToRemove = wall;
                    break;
                }
            }
        }
        if (duplicateFound)
        {
            wallList.Remove(wallToRemove);
            doorList.Add(splitLocation);
            if (isVertical)
            {
                float wallScale1 = wallToSplit.z + (wallToSplit.w / 2.0f) - (splitLocation.z + (corridorWidth / 2.0f)) + 0.1f;
                float wallScale2 = splitLocation.z - (corridorWidth / 2.0f) - (wallToSplit.z - (wallToSplit.w / 2.0f)) + 0.1f;
                float wallZPosition1 = wallToSplit.z + (wallToSplit.w / 2.0f) - (wallScale1 / 2.0f);
                float wallZPosition2 = wallToSplit.z - (wallToSplit.w / 2.0f) + (wallScale2 / 2.0f);
                wallList.Add(new Vector4(wallToSplit.x, wallToSplit.y, wallZPosition1, wallScale1));
                wallList.Add(new Vector4(wallToSplit.x, wallToSplit.y, wallZPosition2, wallScale2));
            }
            else
            {
                float wallScale1 = wallToSplit.x + (wallToSplit.w / 2.0f) - (splitLocation.x + (corridorWidth / 2.0f)) + 0.1f;
                float wallScale2 = splitLocation.x - (corridorWidth / 2.0f) - (wallToSplit.x - (wallToSplit.w / 2.0f)) + 0.1f;
                float wallXPosition1 = wallToSplit.x + (wallToSplit.w / 2.0f) - (wallScale1 / 2.0f);
                float wallXPosition2 = wallToSplit.x - (wallToSplit.w / 2.0f) + (wallScale2 / 2.0f);
                wallList.Add(new Vector4(wallXPosition1, wallToSplit.y, wallToSplit.z, wallScale1));
                wallList.Add(new Vector4(wallXPosition2, wallToSplit.y, wallToSplit.z, wallScale2));
            }
        }
        else
        {
            wallList.Add(wallToAdd);
        }
        /*if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }*/
    }

    private void DestroyShip()
    {
        DestroyImmediate(player);
        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }

    private Mesh FlipMesh(Mesh originalMesh)
    {
        Mesh flippedMesh = new Mesh();
        flippedMesh.vertices = originalMesh.vertices;
        flippedMesh.triangles = new int[]
        {
            2,
            1,
            0,
            3,
            1,
            2
        };//Array.ConvertAll(originalMesh.triangles, i => originalMesh.triangles[originalMesh.triangles.Length - i - 1]);
        flippedMesh.uv = originalMesh.uv;

        return flippedMesh;
    }
}
