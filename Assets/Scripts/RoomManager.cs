using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // Serialized fields allow these to be set in the Unity Inspector
    [SerializeField] GameObject roomPrefab; // Prefab for the room objects
    [SerializeField] private int maxRooms = 15; // Maximum number of rooms to generate
    [SerializeField] private int minRooms = 10; // Minimum number of rooms needed before stopping

    // Room size (width and height) in Unity units
    int roomWidth = 60;
    int roomHeight = 36;

    // Grid dimensions, defines the grid size for room placement
    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;

    [SerializeField] public float minRoomScale = 1.0f; // Minimum room scale factor
    [SerializeField] public float maxRoomScale = 3.0f; // Maximum room scale factor

    [SerializeField] private GameObject[] randomElementPrefabs; // Array to hold the random element prefabs (e.g., enemies, items, etc.)
    [SerializeField] private int maxRandomElements = 3; // Max number of random elements to spawn in each room

    // List to store room GameObjects and queue to manage room generation
    private List<GameObject> roomObjects = new List<GameObject>();
    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    // The grid to track which spots are occupied by rooms
    private int[,] roomGrid;

    // Counter to track the number of rooms generated
    private int roomCount;

    // Flag to check whether the room generation is complete
    private bool generationComplete = false;

    // Start is called when the script is first executed
    private void Start()
    {
        // Initialize the grid with empty values (no rooms yet)
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        // Start room generation from the center of the grid
        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    // Update is called once per frame, checks whether room generation is complete
    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            // Get the next room to generate from the queue
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            // Try generating rooms in the four cardinal directions (left, right, up, down)
            TryGenerateRoom(new Vector2Int(gridX - 1, gridY)); // Left
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY)); // Right
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1)); // Up
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1)); // Down
        }
        else if (roomCount < minRooms)
        {
            // If we have less than the minimum required rooms, regenerate
            Debug.Log("RoomCount was less than the minimum amount of rooms. Trying again");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            ApplyRandomScaleToRooms();
            // If we reached the desired number of rooms, mark generation as complete
            Debug.Log($"Generation complete. {roomCount} rooms created.");
            generationComplete = true;
        }
    }

    // Initializes the generation process from a specific room index
    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex); // Add the initial room to the queue
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1; // Mark the grid spot as occupied
        roomCount++; // Increment the room counter

        // Instantiate the room at the calculated position
        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex; // Set room index in the Room component
        roomObjects.Add(initialRoom); // Add to the list of room objects
    }

    // Tries to generate a room at a specific grid index
    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        // If we've reached the max number of rooms, don't generate more
        if (roomCount >= maxRooms)
        {
            return false;
        }

        // Randomly decide whether to generate a room (50% chance)
        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
        {
            return false;
        }

        // Ensure that there are no more than one adjacent room
        if (CountAdjacentRooms(roomIndex) > 1)
        {
            return false;
        }

        // Add the room to the queue for future generation
        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1; // Mark the grid as occupied
        roomCount++; // Increment the room counter

        // Instantiate the new room prefab at the grid position
        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom); // Add to the room objects list

        //Gen random elements
        GenerateRandomElements(newRoom);

        // Open doors between neighboring rooms
        OpenDoors(newRoom, x, y);

        return true;
    }

    // Clears the current rooms and restarts room generation
    private void RegenerateRooms()
    {
        // Destroy all room objects and clear the list
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();

        // Reset the grid and counters
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        // Restart room generation from the center
        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    // Opens doors between neighboring rooms based on adjacency
    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        // Get the neighboring rooms (left, right, top, bottom)
        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        // Open doors between this room and its neighbors if they exist
        if (x > 0 && roomGrid[x - 1, y] != 0) // Left
        {
            newRoomScript.OpenDoor(Vector2Int.left);
            leftRoomScript.OpenDoor(Vector2Int.right);
        }
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) // Right
        {
            newRoomScript.OpenDoor(Vector2Int.right);
            rightRoomScript.OpenDoor(Vector2Int.left);
        }
        if (y > 0 && roomGrid[x, y - 1] != 0) // Below
        {
            newRoomScript.OpenDoor(Vector2Int.down);
            bottomRoomScript.OpenDoor(Vector2Int.up);
        }
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) // Above
        {
            newRoomScript.OpenDoor(Vector2Int.up);
            topRoomScript.OpenDoor(Vector2Int.down);
        }
    }

    // Finds and returns the Room script attached to a room at the given grid index
    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
        {
            return roomObject.GetComponent<Room>();
        }
        return null;
    }

    // Counts how many neighboring rooms are adjacent to the given room index
    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        // Check for neighbors in all four directions
        if (x > 0 && roomGrid[x - 1, y] != 0) count++; // Left
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++; // Right
        if (y > 0 && roomGrid[x, y - 1] != 0) count++; // Below
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++; // Above

        return count;
    }

    // Calculates the position of a room based on its grid index
    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        // Convert grid index to world position, centered around the middle of the grid
        return new Vector3(roomWidth * (gridX - gridSizeX / 2), roomHeight * (gridY - gridSizeY / 2));
    }

    // Draws the grid in the editor for debugging purposes
    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        // Draw a wireframe cube at each grid position
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }

    private void GenerateRandomElements(GameObject room)
    {
        // Get the room's position and calculate bounds for where to place the elements
        Vector3 roomPosition = room.transform.position;

        // Determine the number of random elements to spawn based on a random value
        int elementsToSpawn = Random.Range(1, maxRandomElements + 1); // Random number between 1 and maxRandomElements

        for (int i = 0; i < elementsToSpawn; i++)
        {
            // Randomly choose one element prefab from the array
            GameObject randomElement = randomElementPrefabs[Random.Range(0, randomElementPrefabs.Length)];

            // Random position inside the room, assuming the room size is `roomWidth` and `roomHeight`
            float randomX = Random.Range(roomPosition.x - roomWidth / 2, roomPosition.x + roomWidth / 2);
            float randomY = Random.Range(roomPosition.y - roomHeight / 2, roomPosition.y + roomHeight / 2);
            Vector3 randomPosition = new Vector3(randomX, randomY, roomPosition.z);

            // Instantiate the random element at the calculated position
            GameObject elementInstance = Instantiate(randomElement, randomPosition, Quaternion.identity);

            elementInstance.transform.SetParent(room.transform);
        }
    }
    private void ApplyRandomScaleToRooms()
    {
        foreach (var room in roomObjects)
        {
            // Get a random scale factor within the specified range
            float randomScale = Random.Range(minRoomScale, maxRoomScale);

            // Apply the random scale to the room's transform
            room.transform.localScale = new Vector3(randomScale, randomScale, 1);
        }
    }


}
