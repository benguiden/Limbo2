using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{

    public AudioSource audioSource;
    public PlayerController player;
    public AudioClip feet, jump;

    public void Play() {
        audioSource.clip = feet;
        audioSource.pitch = 0.9f + (Mathf.Abs (player.velocity.x) * 0.1f);
        audioSource.volume = 0.5f + (Mathf.Abs (player.velocity.x) * 0.2f);
        audioSource.panStereo = Mathf.Clamp (transform.position.x / 15f, -1f, 1f);
        audioSource.time = 0f;
        audioSource.Play ();
    }

    public void PlayJump() {
        audioSource.clip = jump;
        audioSource.pitch = 1f;
        audioSource.volume = 0.75f;
        audioSource.panStereo = Mathf.Clamp (transform.position.x / 15f, -1f, 1f);
        audioSource.time = 0f;
        audioSource.Play ();
    }
}
