using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 8.0f;
    public float speedmultiplier = 1.0f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;

    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 startingPosition { get; private set; }

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>(); //Når spillet starter, finder den Rigidbody2D-komponenten.
        this.startingPosition = this.transform.position; //Gemmer startpositionen.
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState() //Resetter alt set start position
    {
        this.speedmultiplier = 1.0f;
        this.direction = this.initialDirection;
        this.nextDirection = Vector2.zero;
        this.transform.position = this.startingPosition;
        this.rigidbody.isKinematic = false;
        this.enabled = true;
    }

    private void Update() //hvis next direction ikke er null, skal den skifte til næste direction når mugligt
    {
        if (this.nextDirection != Vector2.zero)
        {
            SetDirection(this.nextDirection);
        }
    }

    private void FixedUpdate()
    {
        Vector2 position = this.rigidbody.position; //sætter rigedbody position til variable position
        Vector2 translation = this.direction * this.speed * this.speedmultiplier * Time.fixedDeltaTime; //udregner den nye position basseret på speed og tid og retning
        this.rigidbody.MovePosition(position + translation); //flytter objektet/pacman fremad i den direction 
    }

    public void SetDirection(Vector2 direction, bool forced = false) 
    {
        if(forced || !Occupied(direction)) //Hvis du kan skifte med det samme (ingen væg i vejen) ? skift nu.
        {
            this.direction = direction;
            this.nextDirection = Vector2.zero;
        }
        else //Hvis ikke, så gem den ønskede retning til senere.
        {
            this.nextDirection = direction;
        }
    }

    public bool Occupied(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.5f, this.obstacleLayer); //Skyder en usynlig box lidt foran i den ønskede retning.
        return hit.collider != null; //hvis den rammer noget så er der Occupied
    }
}

