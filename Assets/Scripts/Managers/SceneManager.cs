using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

    public static SceneManager main;

    #region Public Variables
    [Header ("Rooms")]
    public int roomIndex = 0;
    public PuzzleRoom[] rooms;
    public Transform loadedRoomParent;
    [HideInInspector]
    public PuzzleRoom loadedRoomPuzzle;
    [HideInInspector]
    public bool canLoad = false;

    [Header ("Scene Objects")]
    public PlayerController player;
    public CreatureBehaviour creature;

    [Header ("Input")]
    public string restartInputString = "Restart";

    [Header ("Visuals")]
    public RoomTransition roomTransition;
    #endregion

    #region Hidden Variables
    //References
    [HideInInspector]
    public List<Lure> lures = new List<Lure> ();
    #endregion

    #region Private Variables
    private Coroutine loadRoomCo;

    //Input
    private bool restartInput = false;
    private bool restartInputLast = false;
    #endregion

    #region Mono Methods
    private void Awake() {
        main = this;
    }

    private void Start() {
        roomTransition.enabled = false;
        LoadRoom ();
        roomTransition.enabled = true;
    }

    private void Update() {
        //Restart Input
        UpdateRestartInput ();
        if (restartInput) {
            LoadRoom ();
        }
    }
    #endregion

    #region Set up Methods
    public void LoadRoom() {
        if (loadRoomCo != null) {
            Application.Quit();
        } else {
            loadRoomCo = StartCoroutine (ILoadRoom ());
            roomTransition.Transition ();
        }
    }

    public void LoadRoom(int newRoomIndex) {
        if (canLoad) {
            roomIndex = newRoomIndex;
            if (roomIndex >= rooms.Length) {
                Application.Quit();
            } else {
                LoadRoom ();
            }
        }
    }

    public void LoadNextRoom() {
        if (canLoad) {
            roomIndex++;
            if (roomIndex >= rooms.Length) {
                Application.Quit();
            } else {
                LoadRoom ();
            }
        }
    }

    private IEnumerator ILoadRoom() {
        canLoad = false;

        yield return null;

        //Unload current room
        if (loadedRoomParent.childCount > 0) {
            Destroy (loadedRoomParent.GetChild (0).gameObject);
        }

        loadedRoomPuzzle = Instantiate (rooms[roomIndex].gameObject, loadedRoomParent).GetComponent<PuzzleRoom> ();
        loadedRoomPuzzle.transform.localPosition = Vector3.zero;
        loadedRoomPuzzle.gameObject.SetActive (true);

        foreach(Lure lure in lures) {
            if (lure != null)
                Destroy (lure.gameObject);
        }
        lures = new List<Lure> ();

        yield return null;

        ResetObjects ();

        loadRoomCo = null;
        canLoad = true;
    }

    private void ResetObjects() {
        player.transform.position = loadedRoomPuzzle.playerSpawn.position;
        creature.transform.position = loadedRoomPuzzle.playerSpawn.position + new Vector3 (0f, 0.85f, 0f);
        creature.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
        creature.GetComponent<Rigidbody2D> ().isKinematic = true;
        player.velocity = Vector2.zero;
        player.throwingController.heldObjectType = ThrowController.HeldObjectType.Creature;
        creature.transform.parent = player.transform;
        creature.SetState (CreatureBehaviour.States.None);

        player.throwingController.heldObjectType = ThrowController.HeldObjectType.Creature;

        Camera.main.orthographicSize = loadedRoomPuzzle.size.y / 2f;
    }
    #endregion

    #region Input Methods
    private void UpdateRestartInput() {
        if (Input.GetAxisRaw (restartInputString) > 0f) {
            if (restartInputLast)
                restartInput = false;
            else
                restartInput = true;
            restartInputLast = true;
        } else {
            restartInput = false;
            restartInputLast = false;
        }
    }
    #endregion

}
