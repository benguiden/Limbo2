using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullPush : MonoBehaviour {

    [Header("Force")]
    public float force;
    public AnimationCurve forceOverDistance;

    [Header ("Visuals")]
    public Sprite enabledSprite;
    public Sprite disabledSprite;

    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    public bool canDeactivate;

    private void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer> ();

        if (enabled) {
            spriteRenderer.sprite = enabledSprite;
        } else {
            spriteRenderer.sprite = disabledSprite;
        }
    }

    private void OnEnable() {
        spriteRenderer.sprite = enabledSprite;
    }

    private void OnDisable() {
        spriteRenderer.sprite = disabledSprite;
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (enabled) {
            if (collision.gameObject.tag == "Player") {
                float distance = Vector2.Distance (collision.transform.position, transform.position);
                float currentForce = forceOverDistance.Evaluate (1f - (distance / (boxCollider.size.x / 2f)));

                PlayerController player = SceneManager.main.player;
                currentForce *= force;

                if (player.velocity.y <= 0f) {
                    currentForce *= player.riseTime / player.fallTime;
                }

                SceneManager.main.player.velocity += (Vector2)transform.right * currentForce;
            }
        }
    }

    public void Activate()
    {
        enabled = true;
        //Activate particle effects
    }

    public void Deactivate()
    {
        if(canDeactivate)
        {
            enabled = false;
        }
       
        //Deactivate particle effect
    }

}
