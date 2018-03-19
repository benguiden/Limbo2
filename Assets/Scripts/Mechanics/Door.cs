using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    #region Public Variables
    [Header ("Functionality")]
    public bool locked = true;

    [Header("Visuals")]
    public Sprite lockedSprite;
    public Sprite unlockedSprite;
    #endregion

    #region Private Variables
    //References
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Mono Methods
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    private void Start() {
        if (locked)
            spriteRenderer.sprite = lockedSprite;
        else
            spriteRenderer.sprite = unlockedSprite;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (!locked) {
            if (collision.gameObject.tag == "Player") {
                if (SceneManager.main.player.throwingController.heldObjectType == ThrowController.HeldObjectType.Creature) {
                    //Load To Next Room
                    SceneManager.main.LoadNextRoom ();
                }
            }
        }
    }
    #endregion

    #region Public Methods
    public void Unlock() {
        locked = false;
        spriteRenderer.sprite = unlockedSprite;
    }
    #endregion

}
