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

    [Header ("Scene Objects")]
    public PlayerController player;
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
        //Unload current room
        if (loadedRoomParent.childCount > 0) {
            Destroy (loadedRoomParent.GetChild (0).gameObject);
        }

        loadedRoomPuzzle = GameObject.Instantiate (rooms[roomIndex].gameObject, loadedRoomParent).GetComponent<PuzzleRoom> ();
        loadedRoomPuzzle.transform.localPosition = Vector3.zero;
        loadedRoomPuzzle.gameObject.SetActive (true);

        yield return null;
    }
    #endregion

}
