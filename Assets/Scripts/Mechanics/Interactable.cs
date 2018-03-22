using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public enum InteractableType {Door, Spikes, Ladder, PullPush}

    #region Private Variables
    private InteractableType interactableType;
    private Door door;
    private Spikes spikes;
    private Ladder ladder;
    private PullPush pullPush;
    #endregion

    #region Mono Methods
    private void Awake() {
        RefreshReferences ();
    }
    #endregion

    #region Public Methods
    public void RefreshReferences() {
        door = GetComponent<Door> ();
        spikes = GetComponent<Spikes> ();
        ladder = GetComponent<Ladder> ();
        pullPush = GetComponent<PullPush> ();

        if (door != null)
            interactableType = InteractableType.Door;
        else if (spikes != null)
            interactableType = InteractableType.Spikes;
        else if (ladder != null)
            interactableType = InteractableType.Ladder;
        else if (pullPush != null)
            interactableType = InteractableType.PullPush;
        else {
            Debug.LogError ("Error: Interactable component with no compatible component included.");
            Debug.Break ();
        }
    }

    public void ButtonInteract() {
        if (enabled) {
            switch (interactableType) {
                case InteractableType.Door:
                    door.Unlock ();
                    break;
                case InteractableType.Spikes:
                    spikes.Deactivate ();
                    break;
                case InteractableType.Ladder:
                    ladder.Activate ();
                    break;
                case InteractableType.PullPush:
                    pullPush.enabled = true;
                    break;
            }
        }
    }
    #endregion

}
