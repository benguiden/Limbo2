using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lure : MonoBehaviour {

    private Rigidbody2D rb2d;
    private AudioSource audioSource;

    private void Awake() {
        rb2d = GetComponent<Rigidbody2D> ();
        audioSource = GetComponent<AudioSource> ();
    }

    private void Start() {
        SceneManager.main.lures.Add (this);
        Invoke ("DisableRB", 3f);

        transform.localEulerAngles = new Vector3 (0f, 0f, Random.Range (0f, 360f));
    }

    private void Update() {
        if (rb2d != null) {
            if ((rb2d.velocity.magnitude > 0f) && (rb2d.bodyType != RigidbodyType2D.Kinematic)) {
                transform.Rotate (new Vector3 (0f, 0f, 40f * rb2d.velocity.magnitude * Time.deltaTime * -Mathf.Sign (rb2d.velocity.x)));
            }
        }
    }

    private void DisableRB() {
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (rb2d.velocity.magnitude > 0.1f) {
            audioSource.time = 0f;
            audioSource.pitch = 0.8f + (0.1f * rb2d.velocity.magnitude);
            audioSource.volume = 0.4f * rb2d.velocity.magnitude;
            audioSource.panStereo = Mathf.Clamp (transform.position.x / 15f, -1f, 1f);
            audioSource.Play ();
        }
    }

}
