using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    public Movement movement { get; private set; }

    public void Awake()
    {
        this.movement = GetComponent<Movement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.movement.SetDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.movement.SetDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.movement.SetDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.movement.SetDirection(Vector2.right);
        }

        float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x);
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        MoveSnake();
    }

    public void ResetState()
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();
        ClearTail(); // Clear the tail when resetting        
        InitializeTail(); // Reinitialize the tail        
    }

    public GameObject bodySegmentPrefab;
    public int initialTailLength = 0;
    private List<Transform> tailSegments = new List<Transform>(); // List to store body segments        
    private float moveTimer = 0f;
    public float moveInterval = 0.1f; // Time interval between moves        

    void Start()
    {
        InitializeTail();
    }

    void InitializeTail()
    {
        for (int i = 0; i < initialTailLength; i++)
        {
            AddTailSegment();
        }
    }

    void AddTailSegment()
    {
        GameObject segment = Instantiate(bodySegmentPrefab, transform.position, Quaternion.identity);
        tailSegments.Add(segment.transform);
    }

    void ClearTail()
    {
        foreach (Transform segment in tailSegments)
        {
            Destroy(segment.gameObject);
        }
        tailSegments.Clear();
    }

    public void GrowSnake()
    {
        AddTailSegment();
    }

    void MoveSnake()
    {
        moveTimer += Time.deltaTime;
        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;

            Vector2 previousPosition = transform.position;

            transform.position = new Vector3(
                transform.position.x + movement.direction.x * movement.speed * Time.deltaTime,
                transform.position.y + movement.direction.y * movement.speed * Time.deltaTime,
                transform.position.z
            );

            for (int i = 0; i < tailSegments.Count; i++)
            {
                Vector2 tempPosition = tailSegments[i].position;
                tailSegments[i].position = previousPosition;
                previousPosition = tempPosition;
            }

            CheckCollisionWithTail();
        }
    }

    void CheckCollisionWithTail()
    {
        for (int i = 1; i < tailSegments.Count; i++) // Start from index 1 to skip the first segment  
        {
            if (Vector2.Distance(transform.position, tailSegments[i].position) < 0.5f) // Adjusted collision threshold  
            {
                // Ensure Pacman doesn't collide with the tail segment immediately after turning  
                if (i == 1 && Vector2.Distance(transform.position, tailSegments[i].position) < 0.1f)
                {
                    continue;
                }

                Die();
                break;
            }
        }
    }

    void Die()
    {
        this.gameObject.SetActive(false);
        FindObjectOfType<GameManager>().PacmanEaten();
    }
}
