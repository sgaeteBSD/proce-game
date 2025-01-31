using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{

	float speed = 10f;
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start()
	{
		rb2d = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
	{

		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		rb2d.velocity = new Vector2(moveHorizontal * speed, moveVertical * speed);
	}
}