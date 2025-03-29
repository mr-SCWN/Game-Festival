using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; 
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private float lastMoveX = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true; 
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movement = new Vector2(moveX, moveY).normalized;

        if (moveX != 0)
        {
            lastMoveX = moveX;
        }

        float animationX = (moveX == 0 && moveY != 0) ? lastMoveX : moveX;

        animator.SetFloat("MoveX", animationX);
        animator.SetFloat("MoveY", moveY);
        animator.SetBool("IsMoving", movement.magnitude > 0.01f);

    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
