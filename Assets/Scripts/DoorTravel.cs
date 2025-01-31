using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTravel : MonoBehaviour
{
    private BoxCollider2D door;
    public Camera camera;
    public GameObject player;
    private GameObject doorObj;

    // Start is called before the first frame update
    void Start()
    {
        door = GetComponent<BoxCollider2D>();
        doorObj = this.gameObject;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        bool doorCheck = door.IsTouchingLayers(LayerMask.GetMask("Player"));

        if (doorCheck)
        {
            if (name == "LeftDoor")
            {
                doorObj.SetActive(false);
                Camera.main.transform.Translate(-23, 0, 0);
                player.transform.Translate(-4, 0, 0);
                doorObj.SetActive(true);
            }
            if (name == "RightDoor")
            {
                doorObj.SetActive(false);
                Camera.main.transform.Translate(23, 0, 0);
                player.transform.Translate(4, 0, 0);
                doorObj.SetActive(true);
            }
            if (name == "BottomDoor")
            {
                doorObj.SetActive(false);
                Camera.main.transform.Translate(0, -12, 0);
                player.transform.Translate(0, -5, 0);
                doorObj.SetActive(true);
            }
            if (name == "TopDoor")
            {
                doorObj.SetActive(false);
                Camera.main.transform.Translate(0, 12, 0);
                player.transform.Translate(0, 5, 0);
                doorObj.SetActive(true);
            }
        }
    }
}
