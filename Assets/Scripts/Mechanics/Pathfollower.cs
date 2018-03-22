using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfollower : MonoBehaviour {

    #region Public Variables
    [Header("Path")]
    public int pathPointIndex;
    [Range(0f, 1f)]
    public float pathPointProgress;
    public Vector2[] pathPoints;

    [Header("Movement")]
    public float moveSpeed = 1f;
    public AnimationCurve moveCurve;

    [Header("Interaction")]
    public bool movesAbove = false;
    #endregion

    #region Private Variables
    //References
    private Transform platform;

    //Movement
    private Vector2 lastPosition = Vector2.zero;
    private Vector2 displacement = Vector2.zero;
    #endregion

    #region Mono Methods
    private void OnDrawGizmos() {
        if (pathPoints != null) {
            Gizmos.color = Color.red;
            for (int i = 0; i < pathPoints.Length; i++) {
                Gizmos.DrawWireCube(pathPoints[i] + (Vector2)transform.position, new Vector3(1f, 1f, 1f));
                if (i == 0) {
                    Gizmos.DrawLine(pathPoints[i] + (Vector2)transform.position, pathPoints[pathPoints.Length - 1] + (Vector2)transform.position);
                } else {
                    Gizmos.DrawLine(pathPoints[i] + (Vector2)transform.position, pathPoints[i - 1] + (Vector2)transform.position);
                }
            }
        }
    }

    private void Awake() {
        if (transform.childCount == 0) {
            Debug.LogError("Error: Pathfollow component with no child transform.");
            Debug.Break();
        } else {
            platform = transform.GetChild(0);
        }
    }

    private void Start() {
        UpdatePosition();
    }

    private void Update() {
        UpdatePointTime();
        UpdatePosition();
        if (movesAbove)
            UpdateAllAbove();
    }
    #endregion

    #region Movement Methods
    private void UpdatePointTime() {
        float pointDistance;
        if (pathPointIndex == pathPoints.Length - 1) {
            pointDistance = Vector2.Distance(pathPoints[pathPointIndex], pathPoints[0]);
        } else {
            pointDistance = Vector2.Distance(pathPoints[pathPointIndex], pathPoints[pathPointIndex + 1]);
        }

        
        float deltaTime = moveSpeed/ pointDistance;
        pathPointProgress += deltaTime * Time.deltaTime;

        if (pathPointProgress >= 1f) {
            pathPointProgress = pathPointProgress % 1f;
            pathPointIndex = (pathPointIndex + 1) % pathPoints.Length;
        }
    }

    private void UpdatePosition() {
        Vector2 nextPoint;
        if (pathPointIndex == pathPoints.Length - 1) {
            nextPoint = pathPoints[0];
        } else {
            nextPoint = pathPoints[pathPointIndex + 1];
        }

        Vector2 newPosition = Vector2.Lerp(pathPoints[pathPointIndex], nextPoint, moveCurve.Evaluate(pathPointProgress)) + (Vector2)transform.position;

        displacement = newPosition - lastPosition;
        lastPosition = newPosition;

        platform.position = new Vector3(newPosition.x, newPosition.y, platform.position.z);
    }

    private void UpdateAllAbove() {
        PlayerController player = SceneManager.main.player;
        if (player.isGrounded) {
            if (player.movingPlatform == this) {
                player.transform.position += (Vector3)displacement;
                //Debug.Log(player.velocity / Time.deltaTime);
            }
        }
    }
    #endregion

}
