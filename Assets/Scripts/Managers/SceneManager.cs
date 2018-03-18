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
    #endregion

    #region Private Variables
    private Coroutine loadRoomCo;
    #endregion

    #region Mono Methods
    private void Awake() {
        main = this;
    }

    private void Start() {
        LoadRoom ();
    }
    #endregion

    #region Set up Methods
    public void LoadRoom() {
        if (loadRoomCo != null) {
            Debug.LogError ("Error: 2 load room coroutines activated.");
            Debug.Break ();
        } else {
            loadRoomCo = StartCoroutine (ILoadRoom ());
        }
    }

    private IEnumerator ILoadRoom() {
        canLoad = false;

        //Unload current room
        if (loadedRoomParent.childCount > 0) {
            Destroy (loadedRoomParent.GetChild (0).gameObject);
        }

        loadedRoomPuzzle = Instantiate (rooms[roomIndex].gameObject, loadedRoomParent).GetComponent<PuzzleRoom> ();
        loadedRoomPuzzle.transform.localPosition = Vector3.zero;
        loadedRoomPuzzle.gameObject.SetActive (true);

        player.transform.position = loadedRoomPuzzle.playerSpawn.position;
        creature.transform.position = loadedRoomPuzzle.creatureSpawn.position;

        yield return null;

        loadRoomCo = null;
        canLoad = true;
    }
    #endregion

}
