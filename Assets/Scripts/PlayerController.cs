using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Should be executed last of frame
public class PlayerController : MonoBehaviour {

    #region Public Variables
    [Header("Movement")]
    public float accelerationTime = 0.5f;
    public float decelerationTime = 0.4f;
    public AnimationCurve horizontalCurve;

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

    //Input
    private float horizontalInput = 0f;

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

        //Update Velocity based on the horizontalCurve
        UpdateVelocity();

        //Alter Rigidbody

    }
    #endregion

    #region Movement Methods
    private void UpdateVelocity() {
        horizontalTime += Time.deltaTime * horizontalInput;
        horizontalTime = Mathf.Clamp(horizontalTime, -horizontalTime, horizontalTime);

        velocity.x = horizontalCurve.Evaluate(horizontalTime / accelerationTime);
    }

    private void UpdateRigidbody() {
        rb2d.velocity = velocity;
        velocity = Vector2.zero;
    }
    #endregion

}
