using UnityEngine;

public class PacManMovement : MonoBehaviour
{
    [field: SerializeField]
    public float speed { get; set; } = 12.0f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        Vector2 move = new Vector2();

        if (Input.GetKey(KeyCode.W))
        {
            move.y += speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move.x -= speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move.x += speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move.y -= speed;
        }

        rb.MovePosition(rb.position + (move * Time.deltaTime));
    }
}
