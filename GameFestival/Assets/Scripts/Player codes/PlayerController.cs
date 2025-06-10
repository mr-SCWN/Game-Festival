using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    [HideInInspector] public Vector2 movement;
    Rigidbody2D rb;
    float lastMoveX = 1f;

    void Awake() {
      rb = GetComponent<Rigidbody2D>();
      rb.freezeRotation = true;
    }

    void Update() {
      float mx = Input.GetAxisRaw("Horizontal");
      float my = Input.GetAxisRaw("Vertical");
      movement = new Vector2(mx, my).normalized;
      if (mx != 0) lastMoveX = mx;
    }

    void FixedUpdate() {
      rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
