using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureBehaviour : MonoBehaviour
{
    public enum States { None, Idle, Wander, Flee, Cling, Death, Jump, Seek };

    [SerializeField]
    States currentState = States.None;

    new Rigidbody2D rigidbody;

    public float moveSpeed;
    public float maxSpeed;

    public Vector3 velocity;
    Vector3 force;

    public GameObject fleeTarget;
    public GameObject lurePrefab; //Temporary variable, delete later
    public Transform lureSpawn; //Temporary variable, delete later
    public GameObject lure;


    [Header ("Picking Up")]
    public string pickUpString = "PickUp";
    public float pickUpDistance = 1.5f;
    GameObject finalDistraction;

    bool isJumping;

    PlayerController player;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastPosition = new Vector2 ();

    // Use this for initialization
    void Awake()
    {
        rigidbody = GetComponentInChildren<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
        spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
    }

    // Update is called once per frame
    void Update()
    {
        ActState();
        StartCoroutine(Jump());
        StartCoroutine(CheckForDistracton());
        StartCoroutine(CheckForLure());

        //Pick up creature
        if ((player.throwingController.heldObjectType != ThrowController.HeldObjectType.Creature) && (Input.GetAxisRaw (pickUpString) >= 0.5f)) {
            if (Vector2.Distance((Vector2)transform.position, (Vector2)player.transform.position) <= pickUpDistance) {
                player.throwingController.heldObjectType = ThrowController.HeldObjectType.Creature;

                SetState(States.None);
                transform.parent = GameObject.FindGameObjectWithTag ("Player").transform;
                transform.localPosition = new Vector3 (0f, 2.3f, 0f);
                rigidbody.isKinematic = true;
            }
        }

        //Visuals
        Vector2 movement = ((Vector2)transform.position - lastPosition).normalized;
        if (((Vector2)transform.position - lastPosition).magnitude > 0.01f) {
            if (movement.x >= 0f) {
                float newAngle = Mathf.Atan2 (movement.y, movement.x) * Mathf.Rad2Deg;
                spriteRenderer.transform.localEulerAngles = new Vector3 (0f, 0f, newAngle);
                spriteRenderer.flipX = false;
            } else {
                float newAngle = (-Mathf.Atan2 (-movement.y, movement.x) * Mathf.Rad2Deg) + 180f;
                spriteRenderer.transform.localEulerAngles = new Vector3 (0f, 0f, newAngle);
                spriteRenderer.flipX = true;
            }
        }
        lastPosition = transform.position;

    }

    public void SetState(States newState)
    {
        if (newState != States.None)
        {
            if (newState == States.Wander)
            {
                if (currentState == States.Flee || currentState == States.Death)
                {
                    Debug.Log("Can't wander");
                }
            }
            currentState = newState;
        }
    }

    private void ActState()
    {
        if (currentState == States.Wander)
        {
            force = Flee(fleeTarget);
            velocity += force * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            transform.position += velocity * Time.deltaTime;
        }
        else if(currentState == States.Idle)
        {
            velocity = Vector3.zero;
        }
        else if(currentState == States.Cling)
        {
            velocity = Vector3.zero;
        }
        else if ((currentState == States.Seek) && (lure != null))
        {
            if(player.throwingController.heldObjectType != ThrowController.HeldObjectType.Creature)
            {
                force = Seek(lure);
                velocity += force * Time.deltaTime;
                velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
                transform.position += velocity * Time.deltaTime;
                if (Vector2.Distance(transform.position, lure.transform.position) < 0.5f)
                {
                    Destroy(lure);
                    SetState(States.Idle);
                }
            }
        }
        else if (currentState == States.Jump)
        {
            isJumping = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            SetState(States.Idle);
        }
        if (collision.gameObject.tag == "Rope")
        {
            transform.parent = collision.gameObject.transform;
            SetState(States.Cling);
        }
        if (collision.gameObject.tag == "Obstacle")
        {
            SetState(States.Jump);
        }
        if(collision.gameObject.tag == "Rope")
        {
            SetState(States.Cling);
            transform.parent = collision.gameObject.transform;
        }
    }

    private Vector3 Flee(GameObject fleeFrom)
    {
        Vector3 desired = fleeFrom.transform.position - transform.position;
        desired.Normalize();
        desired *= moveSpeed;
        return velocity - desired;
    }

    private Vector3 Seek(GameObject seekTarget)
    {
        Vector3 desired = seekTarget.transform.position - transform.position;
        desired.Normalize();
        desired *= moveSpeed;
        return desired - velocity;
    }

    public void SeekDistraction(GameObject distraction)
    {
        force = Seek(distraction);
        velocity += force * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        transform.position += velocity * Time.deltaTime;
    }

    private IEnumerator Jump()
    {
        while (isJumping)
        {
            rigidbody.AddForce(Vector2.up * 10);
            yield return new WaitForSeconds(0.1f);
            isJumping = false;
            break;
        }
    }

    private IEnumerator CheckForLure()
    {
        lure = GameObject.Find("Lure");
        if (lure != null && player.throwingController.heldObjectType != ThrowController.HeldObjectType.Creature)
        {
            SetState(States.Seek);
        }
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator CheckForDistracton()
    {
        GameObject[] distractions = GameObject.FindGameObjectsWithTag("distraction");
       

        foreach(GameObject dis in distractions)
        {
            float closestDistance = 20;
            float thisDistance = Vector3.Distance(dis.transform.position, transform.position);

            if (thisDistance < closestDistance)
            {
                closestDistance = thisDistance;
                finalDistraction = dis;
            }
        }

        if(finalDistraction != null && currentState == States.Idle)
        {
            SeekDistraction(finalDistraction);
        }

        yield return new WaitForEndOfFrame();
    }
}
