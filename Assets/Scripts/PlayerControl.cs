using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class PlayerControl : MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    private Vector2 movement;

    public float runSpeed = 7.5f;

    private Animator animator;
    private const string horiAnim = "Horizontal";
    private const string vertAnim = "Vertical";
    private const string lasthoriAnim = "LastHorizontal";
    private const string lastvertAnim = "LastVertical";

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down

    }

    void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }
        movement = new Vector2(horizontal * runSpeed, vertical * runSpeed);
        body.velocity = movement;

        animator.SetFloat(horiAnim, movement.x / runSpeed);
        animator.SetFloat(vertAnim, movement.y / runSpeed);

        if (movement != Vector2.zero)
        {
            animator.SetFloat(lasthoriAnim, movement.x);
            animator.SetFloat(lastvertAnim, movement.y);
        }
    }
}