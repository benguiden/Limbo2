using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Should be executed last of frame
public class PlayerController : MonoBehaviour {

    #region Public Variables
    [Header("Movement")]
    public float horizontalSpeed = 4f;
    public float accelerationTime = 0.5f;
    public float decelerationTime = 0.4f;
    public AnimationCurve horizontalCurve;

    [Header("Jumping")]
    public float jumpHeight = 4f;
    public float riseTime = 0.75f; 
    public float fallTime = 0.35f;

    [Header("Input")]
    public string horizontalInputString = "Horizontal"; //Left Joystick
    public string jumpInputString = "Jump"; //A on Xbox controller
    #endregion

    #region Hidden Variables
    //Movement
    [HideInInspector]
    public Vector2 velocity;

    //Jumping
    [HideInInspector]
    public bool isGrounded = false;
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
    private Rigidbody2D rb2d;
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

        //Calculations
        CalculateJump ();
    }

    private void Update() {
        //Get Input
        horizontalInput = Input.GetAxisRaw(horizontalInputString);
        UpdateJumpInput ();

        //Update Velocity based on the horizontalCurve
        UpdateVelocity();

        //Update Jump
        velocity.y = rb2d.velocity.y;
        UpdateJumpMovement ();
        UpdateGravity ();

        //Alter Rigidbody
        UpdateRigidbody();
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
            velocity.x = horizontalSpeed * horizontalCurve.Evaluate(horizontalTime);
        else
            velocity.x = horizontalSpeed * -horizontalCurve.Evaluate(-horizontalTime);
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
        if ((jumpInput) && (isGrounded)) {
            velocity.y = jumpVelocity;
        }
    }

    private void UpdateGravity() {
        if (velocity.y <= 0f)
            velocity.y += gravityDown * Time.deltaTime;
        else
            velocity.y += gravityUp * Time.deltaTime;
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

        RaycastHit2D[] boxHits = Physics2D.BoxCastAll (boxOrigin, new Vector2 (boxCollider.size.x, boxHeight), 0f, Vector2.down, boxHeight + (boxCollider.size.y / 2f));

        foreach (RaycastHit2D boxHit in boxHits) {
            if (boxHit) {
                debugGroundedPoint.Add (boxHit.point);
                if (boxHit.collider.gameObject.isStatic) {
                    isGrounded = true;
                }
            }
        }
    }
    #endregion

}
