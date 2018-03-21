using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public enum InteractableType {Door, Spikes, Ladder}

    #region Private Variables
    private InteractableType interactableType;
    private Door door;
    private Spikes spikes;
    private Ladder ladder;
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

        if (door != null)
            interactableType = InteractableType.Door;
        else if (spikes != null)
            interactableType = InteractableType.Spikes;
        else if (ladder != null)
            interactableType = InteractableType.Ladder;
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
                    Debug.Log("Lower Ladder");
                    ladder.Activate ();
                    break;
            }
        }
    }
    #endregion

}
