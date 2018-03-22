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
    private bool throwInput = false;
    private bool throwInputLast = false;
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
        rb2d = GetComponent<Rigidbody2D>();
        creature = GameObject.FindGameObjectWithTag("Creature").transform;
    }

    private void Update() {
        UpdateThrowInput ();
        GetAimInput ();
        if ((aimInput.x != 0f) || (aimInput.y != 0f)) {
            lineRenderer.enabled = true;
            SetLinePoints();
            
            if (throwInput) {
                Throw();
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

    private Vector3[] ProjectionPositions(uint resolution, Vector2 velocity) {
        Vector3[] points = new Vector3[(int)resolution];

        float timeDelta = lineTime / resolution;

        for (int i=1; i<points.Length; i++) {
            Vector2 lastPoint = points[i - 1];
            Vector2 displacement = Vector2.zero;

            displacement.x = velocity.x * timeDelta;
            points[i].x = lastPoint.x + displacement.x;

            displacement.y = velocity.y * timeDelta;
            points[i].y = lastPoint.y + displacement.y;
            velocity.y += (gravity / 1.95f) * timeDelta;
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
            lureBody.velocity = (GetThrowVelocity() * throwSpeed) + new Vector2(rb2d.velocity.x, 0f);
            lureBody.gravityScale = Mathf.Abs ((gravity * throwSpeed * throwSpeed) / Physics.gravity.y);
        } else {
            heldObjectType = HeldObjectType.Lure;
            creature.parent = null;
            Rigidbody2D creatureRB = creature.GetComponent<Rigidbody2D> ();
            creatureRB.isKinematic = false;
            creatureRB.velocity = (GetThrowVelocity () * throwSpeed) + new Vector2 (rb2d.velocity.x, 0f);
            creatureRB.gravityScale = Mathf.Abs ((gravity * throwSpeed * throwSpeed) / Physics.gravity.y);
        }
    }

    private void UpdateThrowInput() {
        if (Input.GetAxisRaw (throwInputString) > 0f) {
            if(throwInputLast)
                throwInput = false;
            else
                throwInput = true;
            throwInputLast = true;
        } else {
            throwInput = false;
            throwInputLast = false;
        }
    }
    #endregion

}
