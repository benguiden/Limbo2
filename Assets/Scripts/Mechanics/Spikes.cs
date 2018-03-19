using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {

    public void Deactivate() {
        gameObject.SetActive (false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if ((enabled) && (SceneManager.main.canLoad)) {
            if (collision.gameObject.tag == "Player") {
                SceneManager.main.LoadRoom ();
            } else if (collision.gameObject.tag == "Creature") {
                SceneManager.main.LoadRoom ();
            }
        }
    }

}
