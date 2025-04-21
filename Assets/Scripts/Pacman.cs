using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    public Movement movement { get; private set; }

    public void Awake() //ved start henter den Movement objektet og gemmer den i movement variablen.
    {
        this.movement = GetComponent<Movement>();
    }

    private void Update() // håndtere bevægelse ved brug af WASD eller pile
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
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward); //håndtere rotationen af pacmman når han vender sig om 
        MoveSnake(); //håndterer bevægelse og hale-opførsel
    }

    public void ResetState() //er en metode vi skal have så vi kan kalde den og resette statsne
    {
        this.gameObject.SetActive(true);
        this.movement.ResetState();
        ClearTail(); // Clear the tail when resetting        
        InitializeTail(); // Reinitialize the tail        
    }

    //opset af nye variabler
    public GameObject bodySegmentPrefab;
    public int initialTailLength = 0;
    private List<Transform> tailSegments = new List<Transform>(); // List to store body segments        
    private float moveTimer = 0f;
    public float moveInterval = 0.1f; // Time interval between moves        

    void Start() //opretter den initiale hale
    {
        InitializeTail();
    }

    void InitializeTail() //Tilføjer et antal hale-segmenter
    {
        for (int i = 0; i < initialTailLength; i++)
        {
            AddTailSegment();
        }
    }

    void AddTailSegment() // Laver et nyt hale-segment og føjer det til listen.
    {
        GameObject segment = Instantiate(bodySegmentPrefab, transform.position, Quaternion.identity);
        tailSegments.Add(segment.transform);
    }

    void ClearTail() //Fjerner alle eksisterende hale-segmenter (bruges ved reset).
    {
        foreach (Transform segment in tailSegments)
        {
            Destroy(segment.gameObject);
        }
        tailSegments.Clear();
    }

    public void GrowSnake() //Når slangen skal blive længere, kaldes denne funktion
    {
        AddTailSegment();
    }

    void MoveSnake()
    {
        moveTimer += Time.deltaTime; //Først venter den på, at moveTimer når moveInterval.
        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;

            Vector2 previousPosition = transform.position; //Gemmer Pacmans nuværende position som previousPosition.

            transform.position = new Vector3( //Flytter Pacman i den nuværende retning.
                transform.position.x + movement.direction.x * movement.speed * Time.deltaTime,
                transform.position.y + movement.direction.y * movement.speed * Time.deltaTime,
                transform.position.z
            );

            for (int i = 0; i < tailSegments.Count; i++) //Går igennem alle hale-segmenterne:
            {
                Vector2 tempPosition = tailSegments[i].position; //Flytter hvert segment til det forrige segments position.
                tailSegments[i].position = previousPosition;
                previousPosition = tempPosition; //Derved følger halen efter hovedet.
            }

            CheckCollisionWithTail(); //Tjekker om Pacman rammer sin egen hale med CheckCollisionWithTail().
        }
    }

    void CheckCollisionWithTail()
    {
        for (int i = 5; i < tailSegments.Count; i++) // Starter med at ignorere de første 5 segmenter (for at undgå at dø med det samme, hvis halen er meget kort).
        {
            if (Vector2.Distance(transform.position, tailSegments[i].position) < 0.5f) // Adjusted collision threshold  
            {
                Die();
                break;
            }
        }//Hvis Pacmans hoved kommer tættere end 0.5 enheder på et segment ? Kalder Die().
    }

    void Die()
    {
        this.gameObject.SetActive(false); //hvis Die() bliver kald set pacman til false
        FindObjectOfType<GameManager>().PacmanEaten(); //kald pacmanEaten() for at genstarte spillet
    }
}
