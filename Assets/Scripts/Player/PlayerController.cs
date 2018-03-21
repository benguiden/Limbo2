using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Should be executed last of frame
public class PlayerController : MonoBehaviour {

    #region Public Variables
    [Header ("Movement")]
    public float horizontalSpeed = 4f;
    public float accelerationTime = 0.5f;
    public float decelerationTime = 0.4f;
    public AnimationCurve horizontalCurve;

    [Header ("Jumping")]
    public float jumpHeight = 4f;
    public float riseTime = 0.75f;
    public float fallTime = 0.35f;

    [Header ("Climbing")]
    public float climbingSpeed = 4f;

    [Header ("Input")]
    public string horizontalInputString = "Horizontal"; //Left Joystick
    public string jumpInputString = "Jump"; //A on Xbox controller

    [Header ("Collisions")]
    public LayerMask groundedLayer = 8;

    [Header ("Visuals")]
    public SpriteRenderer playerSpriteRenderer;
    #endregion

    #region Hidden Variables
    //Movement
    [HideInInspector]
    public Vector2 velocity;

    //Jumping
    [HideInInspector]
    public bool isGrounded = false;
    [HideInInspector]
    public bool isClimbing = false;

    //References
    [HideInInspector]
    public ThrowController throwingController;
    [HideInInspector]
    public Pathfollower movingPlatform;
    [HideInInspector]
    public Rigidbody2D rb2d;
    #endregion

    #region Private Variables
    //Movement
    private float horizontalTime = 0f;

    //Gravity
    private float jumpVelocity;
    private float gravityUp, gravityDown;
    private const float groundDetectionHeight = 0.15f;

    //Input
    private float horizontalInput = 0f;
    private bool jumpInput = false;
    private bool jumpInputLast = false;

    //References
    private BoxCollider2D boxCollider;

    //Debugging
    private float debugGroundBox = 0f;
    private List<Vector2> debugGroundedPoint = new List<Vector2> ();
    #endregion

    #region Mono Methods
    private void OnValidate() {
        CalculateJump ();
    }

    private void OnDrawGizmosSelected() {
        if (Application.isPlaying) {
            Vector2 boxPosition = (Vector2)transform.position;
            boxPosition.y -= debugGroundBox / 2f;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube ((Vector3)boxPosition, new Vector3 (1f, debugGroundBox, 1f));
            Gizmos.color = Color.red;
            foreach (Vector2 point in debugGroundedPoint) {
                Gizmos.DrawWireSphere ((Vector3)point, 0.05f);
            }
        }
    }

    private void Awake() {
        //References
        rb2d = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D> ();
        throwingController = GetComponent<ThrowController> ();

        //Calculations
        CalculateJump ();
    }

    private void Update() {
        //Get Input
        horizontalInput = Input.GetAxisRaw (horizontalInputString);
        UpdateJumpInput ();

        //Update Velocity based on the horizontalCurve
        UpdateVelocity ();

        //Update Jump
        if (!isClimbing)
            velocity.y += rb2d.velocity.y;
        UpdateJumpMovement ();
        UpdateGravity ();

        //Update Visuals
        UpdateSpriteFlip ();
    }

    private void LateUpdate() {
        //Alter Rigidbody
        UpdateRigidbody ();
    }

    private void FixedUpdate() {
        debugGroundedPoint = new List<Vector2> ();
        isGrounded = false;
        if (rb2d.velocity.y <= 0f)
            CheckGrounded ();
    }
    #endregion

    #region Movement Methods
    private void UpdateVelocity() {
 
        //Change Time base on input
        if (rb2d.velocity.x == 0f)
            horizontalTime = 0f;

        if ((horizontalInput == 0f) && (Mathf.Abs(horizontalInput - horizontalTime) <= 0.05f)) {
            horizontalTime = 0f;
        } else if (horizontalInput > horizontalTime) {
            if (horizontalTime >= 0f)
                horizontalTime += Time.deltaTime * 1f / accelerationTime;
            else
                horizontalTime += Time.deltaTime * 1f / decelerationTime;
        } else if (horizontalInput < horizontalTime) {
            if (horizontalTime <= 0f)
                horizontalTime -= Time.deltaTime * 1f / accelerationTime;
            else
                horizontalTime -= Time.deltaTime * 1f / decelerationTime;
        }

        //Clamp Time
        horizontalTime = Mathf.Clamp(horizontalTime, -1f, 1f);

        //Evaluate from curve
        if (horizontalTime >= 0f)
            velocity.x += horizontalSpeed * horizontalCurve.Evaluate(horizontalTime);
        else
            velocity.x += horizontalSpeed * -horizontalCurve.Evaluate(-horizontalTime);
    }

    private void UpdateRigidbody() {
        rb2d.velocity = velocity;
        velocity = Vector2.zero;
    }

    private void UpdateJumpInput() {
        if (Input.GetAxisRaw(jumpInputString) > 0f) {
            if (jumpInputLast)
                jumpInput = false;
            else
                jumpInput = true;
            jumpInputLast = true;
        } else {
            jumpInput = false;
            jumpInputLast = false;
        }
    }

    private void UpdateJumpMovement() {
        if ((jumpInput) && (isGrounded) && (!isClimbing)) {
            velocity.y = jumpVelocity;
        } else if ((Input.GetAxisRaw(jumpInputString) > 0.5f) && (isClimbing)) {
            velocity.y = climbingSpeed;
        }
    }

    private void UpdateGravity() {
        if (!isClimbing) {
            if (velocity.y <= 0f)
                velocity.y += gravityDown * Time.deltaTime;
            else
                velocity.y += gravityUp * Time.deltaTime;
        }
    }

    private void CalculateJump() {
        gravityDown = -2f * jumpHeight / (fallTime * fallTime);
        gravityUp = -2f * jumpHeight / (riseTime * riseTime);
        jumpVelocity = -gravityUp * riseTime;
    }
    #endregion

    #region Fixed Physics Methods
    private void CheckGrounded() {
        float boxHeight = Mathf.Clamp (groundDetectionHeight - (rb2d.velocity.y * Time.fixedDeltaTime), groundDetectionHeight, 5f);
        debugGroundBox = boxHeight;

        Vector2 boxOrigin = (Vector2)transform.position + boxCollider.offset;
        boxOrigin.y += groundDetectionHeight + (rb2d.velocity.y * Time.fixedDeltaTime);

        RaycastHit2D[] boxHits = Physics2D.BoxCastAll (boxOrigin, new Vector2 (boxCollider.size.x, boxHeight), 0f, Vector2.down, boxHeight + (boxCollider.size.y / 2f), groundedLayer);

        movingPlatform = null;
        foreach (RaycastHit2D boxHit in boxHits) {
            if (boxHit) {
                debugGroundedPoint.Add (boxHit.point);
                isGrounded = true;
                if (boxHit.collider.gameObject.tag == "Moving") {
                    movingPlatform = boxHit.collider.transform.parent.GetComponent<Pathfollower>();
                }
            }
        }
    }
    #endregion

    #region Visual Methods
    private void UpdateSpriteFlip() {
        if (velocity.x < 0f)
            playerSpriteRenderer.flipX = false;
        else if (velocity.x > 0f)
            playerSpriteRenderer.flipX = true;
    }
    #endregion

}
