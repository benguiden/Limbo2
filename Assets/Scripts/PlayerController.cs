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
    [HideInInspector]
    public Vector2 velocity;
    #endregion

    #region Private Variables
    //Movement
    private float horizontalTime = 0f;

    //Gravity
    private float jumpVelocity;
    private float gravityUp, gravityDown;

    //Input
    private float horizontalInput = 0f;
    private float absHorizontalInput = 0f;

    //References
    private Rigidbody2D rb2d;
    #endregion

    #region Mono Methods
    private void Awake() {
        //References
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        //Get Input
        horizontalInput = Input.GetAxisRaw(horizontalInputString);
        absHorizontalInput = Mathf.Abs(horizontalInput);

        //Update Velocity based on the horizontalCurve
        UpdateVelocity();

        //Alter Rigidbody
        UpdateRigidbody();
    }
    #endregion

    #region Movement Methods
    private void UpdateVelocity() {

        //Change Time base on input
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

    private void CalculateJump() {
        jumpVelocity = 2f * jumpHeight / (riseTime + fallTime);
        gravityDown = -2f * jumpHeight / (fallTime * fallTime);
        gravityUp = -2f * jumpHeight / (riseTime * riseTime);
    }
    #endregion
    
}
