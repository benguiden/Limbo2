using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRoom : MonoBehaviour {

    public Vector2 size;
    public Transform playerSpawn;
    public Transform creatureSpawn;

    #region Mono Methods
    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube (transform.position, new Vector3 (size.y * 1.7778f, size.y, 1f));
    }

    private void Start() {
        //Deactivate
        if (SceneManager.main.loadedRoomPuzzle != this)
            gameObject.SetActive (false);
    }
    #endregion

}
