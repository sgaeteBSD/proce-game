using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject TopDoor;
    [SerializeField] GameObject BottomDoor;
    [SerializeField] GameObject LeftDoor;
    [SerializeField] GameObject RightDoor;

    public Vector2Int RoomIndex { get; set; }

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
        {
            TopDoor.SetActive(true);
        }

        if (direction == Vector2Int.down)
        {
            BottomDoor.SetActive(true);
        }

        if (direction == Vector2Int.left)
        {
            LeftDoor.SetActive(true);
        }

        if (direction == Vector2Int.right)
        {
            RightDoor.SetActive(true);
        }
    }

}
