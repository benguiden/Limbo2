using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour {

    #region Public Variables
    [Header ("Physics")]
    public float throwSpeed = 1f;
    public Vector2 horizontalSpeedRange = new Vector2 (2f, 10f);
    public Vector2 verticalSpeedRange = new Vector2 (2f, 10f);
    public float gravity = -20f;
    public Transform releasePosition;

    [Header ("Visuals")]
    public LineRenderer lineRenderer;
    public float lineLength = 5f;
    public uint lineResolution = 32;

    [Header ("Input")]
    public string throwInputString = "Fire1";
    public string aimHorStringInput = "AimHorizontal";
    public string aimVerStringInput = "AimVertical";

    [Header("Lure")]
    public Object lurePrefab;
    #endregion

    #region Private Variables
    //References
    private Rigidbody2D rb2d;

    //Input
    private Vector2 aimInput = new Vector2 ();
    #endregion

    #region Mono Methods
    private void Awake() {
        //References
        if (lineRenderer == null) {
            Debug.LogError ("Error: Line renderer not declared in throw controller.");
            Debug.Break ();
        }
        rb2d = GetComponent<Rigidbody2D> ();
    }

    private void Update() {
        GetAimInput ();

        if ((aimInput.x != 0f) || (aimInput.y != 0f)) {
            lineRenderer.enabled = true;
            SetLinePoints ();
            if (Input.GetKeyDown (KeyCode.LeftShift)) {
                Transform lureTrans = ((GameObject)Instantiate (lurePrefab)).transform;
                if (releasePosition == null)
                    lureTrans.position = transform.position;
                else
                    lureTrans.position = releasePosition.position;

                Rigidbody2D lureBody = lureTrans.GetComponent<Rigidbody2D> ();
                lureBody.velocity = (GetThrowVelocity () * throwSpeed) + rb2d.velocity;
                lureBody.gravityScale = Mathf.Abs ((gravity * throwSpeed * throwSpeed) / Physics.gravity.y);
            }
        } else {
            lineRenderer.enabled = false;
        }
    }
    #endregion

    #region Calculation Methods
    private void SetLinePoints() {
        Vector2 initalVelocity = GetThrowVelocity ();
        lineRenderer.positionCount = (int)lineResolution;
        lineRenderer.SetPositions (ProjectionPositions (lineResolution, initalVelocity));
    }

    private Vector2 GetThrowVelocity() {
        Vector2 throwVelocity = Vector2.zero;
        throwVelocity.x = horizontalSpeedRange.x + ((horizontalSpeedRange.y - horizontalSpeedRange.x) * aimInput.x);
        throwVelocity.y = verticalSpeedRange.x + ((verticalSpeedRange.y - verticalSpeedRange.x) * aimInput.y);
        return throwVelocity;
    }

    private Vector3[] ProjectionPositions(uint resolution, Vector2 initalVelocity) {
        Vector3[] points = new Vector3[(int)resolution];
        float side = Mathf.Sign (initalVelocity.x);
        initalVelocity.x = Mathf.Abs (initalVelocity.x);

        for (int i=0; i<points.Length; i++) {
            points[i].x = (lineLength * Mathf.Abs (aimInput.x + (0.001f * resolution)) * (float)i) / ((float)resolution - 1f);
            float timeAtX = 0f;
            if (initalVelocity.x == 0f)
                timeAtX = 0.01f;
            else
                timeAtX = points[i].x / (initalVelocity.x * throwSpeed);
            points[i].y = (timeAtX * initalVelocity.y * throwSpeed) + ((gravity * timeAtX * timeAtX * throwSpeed * throwSpeed) / 2f);
            points[i].x *= side;
        }

        return points;
    }
    #endregion

    #region Input Methods
    private void GetAimInput() {
        aimInput = new Vector2 (Input.GetAxisRaw (aimHorStringInput), Input.GetAxisRaw (aimVerStringInput));
    }
    #endregion

}
