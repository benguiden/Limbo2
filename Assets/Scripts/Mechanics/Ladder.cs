using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Ladder : MonoBehaviour {

    #region Public Variables
    #endregion

    #region Private Variables
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Mono Methods
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    private void Start() {
        
    }
    #endregion

    #region Mono Messages
    private void OnTriggerEnter2D(Collider2D collision) {
        if (enabled) {
            if (collision.gameObject.tag == "Player") {
                SceneManager.main.player.rb2d.velocity = new Vector2 (SceneManager.main.player.rb2d.velocity.x, 0f);
                SceneManager.main.player.isClimbing = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (enabled) {
            if (collision.gameObject.tag == "Player") {
                SceneManager.main.player.isClimbing = false;
            }
        }
    }
    #endregion

    #region Public Methods
    public void Activate() {
        enabled = true;
        spriteRenderer.enabled = true;
    }
    #endregion

}
