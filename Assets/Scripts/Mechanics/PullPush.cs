using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullPush : MonoBehaviour {

    [Header("Force")]
    public float force;
    public AnimationCurve forceOverDistance;

    private BoxCollider2D boxCollider;

    public bool isActive;

    private void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player" && isActive) {
            float distance = Vector2.Distance(collision.transform.position, transform.position);
            float currentForce = forceOverDistance.Evaluate(1f - (distance / (boxCollider.size.x / 2f)));

            PlayerController player = SceneManager.main.player;
            currentForce *= force;

            if (player.velocity.y <= 0f) {
                currentForce *= player.riseTime / player.fallTime;
            }

            SceneManager.main.player.velocity += (Vector2)transform.right * currentForce;
        }
    }

    public void Activate()
    {
        isActive = true;
        //Activate particle effects
    }

    public void Deactivate()
    {
        isActive = false;
        //Deactivate particle effect
    }

}
