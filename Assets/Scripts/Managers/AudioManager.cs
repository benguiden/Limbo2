using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

    public static AudioManager main;

    #region Private Variables
    private AudioSource audioSource;
    #endregion

    #region Mono Methods
    private void Awake() {
        main = this;

        audioSource = GetComponent<AudioSource> ();
    }
    #endregion

    public void Play() {
        audioSource.Play ();
    }

    public void Play(float time) {
        audioSource.time = time;
        Play ();
    }

    public void Play(AudioClip audioClip) {
        audioSource.clip = audioClip;
        Play (0f);
    }

    public void Play(AudioClip audioClip, float volume) {
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        Play (0f);
    }

    public void Stop() {
        audioSource.Stop ();
    }

}
