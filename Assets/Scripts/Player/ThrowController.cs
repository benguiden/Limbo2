﻿using UnityEngine;

public class ThrowController : MonoBehaviour {

    #region Public Variables
    [Header ("Physics")]
    public float throwSpeed = 1f;
    public Vector2 horizontalSpeedRange = new Vector2 (2f, 10f);
    public Vector2 verticalSpeedRange = new Vector2 (2f, 10f);
    public float gravity = -20f;
    public float gravityK = -20f;
    public Transform releasePosition;

    [Header ("Visuals")]
    public LineRenderer lineRenderer;
    public float lineLength = 5f;
    public uint lineResolution = 32;
    public float lineTime = 1.5f;

    [Header ("Input")]
    public string throwInputString = "Fire1";
    public string aimHorStringInput = "AimHorizontal";
    public string aimVerStringInput = "AimVertical";

    [Header("Lure")]
    public Object lurePrefab;
    #endregion

    #region Hidden Variables
    //Throwing
    [HideInInspector]
    public HeldObjectType heldObjectType = HeldObjectType.Lure;
    #endregion

    #region Private Variables
    //References
    private Rigidbody2D rb2d;
    private Transform creature;

    //Input
    private Vector2 aimInput = new Vector2 ();
    #endregion

    #region Enums
    public enum HeldObjectType { Creature, Lure}
    #endregion

    #region Mono Methods
    private void Awake() {
        //References
        if (lineRenderer == null) {
            Debug.LogError ("Error: Line renderer not declared in throw controller.");
            Debug.Break ();
        }
        rb2d = GetComponent<Rigidbody2D> ();
        creature = GameObject.FindGameObjectWithTag ("Creature").transform;
    }

    private void Update() {
        GetAimInput ();
        aimInput.x = 1f;
        aimInput.y = 1f;
        if ((aimInput.x != 0f) || (aimInput.y != 0f)) {
            lineRenderer.enabled = true;
            SetLinePoints ();
            //if (Input.GetKeyDown (KeyCode.LeftShift)) {
                Throw ();
            //}
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

    private Vector3[] ProjectionPositions(uint resolution, Vector2 velocity) {
        Vector3[] points = new Vector3[(int)resolution];

        float timeDelta = (lineTime * throwSpeed) / resolution;

        velocity.y += -Mathf.Abs(gravityK * 1f) * timeDelta * throwSpeed;

        for (int i=1; i<points.Length; i++) {
            Vector2 lastPoint = points[i - 1];
            Vector2 displacement = Vector2.zero;

            displacement.x = velocity.x * timeDelta * throwSpeed;
            points[i].x = lastPoint.x + displacement.x;

            displacement.y = velocity.y * timeDelta * throwSpeed;
            points[i].y = lastPoint.y + displacement.y;
            velocity.y += -Mathf.Abs(gravityK * 0.65f) * timeDelta * throwSpeed;
        }

        return points;
    }
    #endregion

    #region Input Methods
    private void GetAimInput() {
        aimInput = new Vector2 (Input.GetAxisRaw (aimHorStringInput), Input.GetAxisRaw (aimVerStringInput));
    }

    private void Throw() {
        if (heldObjectType == HeldObjectType.Lure) {
            Transform lureTrans = ((GameObject)Instantiate (lurePrefab)).transform;
            lureTrans.name = "Lure";
            if (releasePosition == null)
                lureTrans.position = transform.position;
            else
                lureTrans.position = releasePosition.position;

            Rigidbody2D lureBody = lureTrans.GetComponent<Rigidbody2D> ();
            lureBody.velocity = (GetThrowVelocity () * throwSpeed) + rb2d.velocity;
            lureBody.gravityScale = Mathf.Abs ((gravity * throwSpeed * throwSpeed) / Physics.gravity.y);
        } else {
            heldObjectType = HeldObjectType.Lure;
            creature.parent = null;
            Rigidbody2D creatureRB = creature.GetComponent<Rigidbody2D> ();
            creatureRB.isKinematic = false;
            creatureRB.velocity = (GetThrowVelocity () * throwSpeed) + rb2d.velocity;
            creatureRB.gravityScale = Mathf.Abs ((gravity * throwSpeed * throwSpeed) / Physics.gravity.y);
        }
    }
    #endregion

}
