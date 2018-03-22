using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : MonoBehaviour {

    public enum TriggerType { Player, Creature };
    public enum BehaviourTypes { None,OpenDoor, LowerLadder, ActivateObject, DeactivateObject,ActivatePad }

    #region Public Variables
    [Header ("Behaviour")]
    public BehaviourTypes behaviourType;
    public BehaviourTypes secondaryType;
    public TriggerType triggerType;

    public bool isActivated;

    public Interactable interactObject;
    public Interactable secondaryObject;
    public PuzzleButton twinButton;

    [Header ("Visuals")]
    public Sprite activatedSprite;
    public Sprite deactivatedSprite;
    public Vector2 childOffsetY = new Vector2 (0.12f, 0.3f);
    #endregion

    #region Private Variables
    //References
    private SpriteRenderer spriteRenderer;
    private Transform child;
    private AudioSource audioSource;
    private bool on = false;
    #endregion

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        audioSource = GetComponent<AudioSource> ();
        audioSource.panStereo = Mathf.Clamp (transform.position.x / 15f, -1f, 1f);
        if (transform.childCount > 0) {
            child = transform.GetChild (0);
        } else {
            Debug.LogError ("Error: Not child of button object for switch.");
            Debug.Break ();
        }
    }

    #region Messages
    private void OnTriggerStay2D(Collider2D other)
    {

        if(other.tag == "Player")
        {
            if (triggerType == TriggerType.Player)
            {
                if (twinButton != null)
                {
                    if (twinButton.isActivated)
                    {
                        EnactBehaviour(behaviourType);
                        isActivated = true;
                    }
                    else
                    {
                        isActivated = true;
                    }
                }
                else
                {
                    Debug.Log("Yup");
                    EnactBehaviour(behaviourType);
                    if(secondaryType != BehaviourTypes.None)
                    {
                        EnactSecondaryBehaviour(secondaryType);
                    }
                    isActivated = true;
                }
            }
            else
            {
                Debug.Log("WRONG!");
            }
        }
        if(other.tag == "Creature")
        {
            
            if (triggerType == TriggerType.Creature)
            {
                if (twinButton != null)  // If button has an active twin, the twin needs to be active
                {
                    if (twinButton.isActivated)
                    {
                        EnactBehaviour(behaviourType);
                        if (secondaryType != BehaviourTypes.None)
                        {
                            EnactSecondaryBehaviour(secondaryType);
                        }
                        isActivated = true;
                    }
                    else
                    {
                        isActivated = true;
                    }
                }
                else  // Otherwise, go ahead
                {
                    EnactBehaviour(behaviourType);
                    if (secondaryType != BehaviourTypes.None)
                    {
                        EnactSecondaryBehaviour(secondaryType);
                    }
                    isActivated = true;
                }
            }
           
        }
        if (!on)
            OnStep ();
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        on = false;
        OnRelease();
        isActivated = false;
    }
    #endregion

    private void EnactBehaviour(BehaviourTypes behaviour)
    {
        interactObject.ButtonInteract ();
        switch (behaviour)
        {
            case BehaviourTypes.ActivateObject:
                ActivateObject(interactObject);
                break;
            case BehaviourTypes.DeactivateObject:
                DeActivateObject(interactObject);
                break;
            case BehaviourTypes.OpenDoor:
                //OpenDoor(interactObject);
                break;
            case BehaviourTypes.LowerLadder:
                interactObject.gameObject.GetComponent<Ladder>().Activate();
                break;
            case BehaviourTypes.ActivatePad:
                interactObject.gameObject.GetComponent<PullPush>().Activate();
                break;
        }

    }

    private void EnactSecondaryBehaviour(BehaviourTypes behaviour)
    {
        Debug.Log("Starting secondary task");
        secondaryObject.ButtonInteract();
        switch (behaviour)
        {
            case BehaviourTypes.ActivateObject:
                ActivateObject(secondaryObject);
                break;
            case BehaviourTypes.DeactivateObject:
                DeActivateObject(secondaryObject);
                break;
            case BehaviourTypes.LowerLadder:
                secondaryObject.gameObject.GetComponent<Ladder>().Activate();
                break;
            case BehaviourTypes.ActivatePad:
                secondaryObject.gameObject.GetComponent<PullPush>().Activate();
                break;
        }

    }

    private void ActivateObject(Interactable objToActivate)
    {
        Debug.Log("Activate");
        objToActivate.gameObject.SetActive(true);
    }

    private void DeActivateObject(Interactable objToDeactivate)
    {
        objToDeactivate.gameObject.SetActive(false);
    }

    private void OpenDoor(Interactable doorToOpen)
    {
        doorToOpen.gameObject.SetActive(false);
    }

    private void OnStep()
    {
        on = true;
        spriteRenderer.sprite = activatedSprite;
        child.localPosition = new Vector3 (child.localPosition.x, childOffsetY.x, child.localPosition.z);
        audioSource.clip = AudioManager.main.buttonOnClip;
        audioSource.Play ();
    }

    private void OnRelease()
    {
        spriteRenderer.sprite = deactivatedSprite;
        child.localPosition = new Vector3 (child.localPosition.x, childOffsetY.y, child.localPosition.z);

        if(behaviourType == BehaviourTypes.LowerLadder)
        {
            interactObject.gameObject.GetComponent<Ladder>().Deactivate();
        }

        if (behaviourType == BehaviourTypes.ActivatePad)
        {
            interactObject.gameObject.GetComponent<PullPush>().Deactivate();
        }
        audioSource.clip = AudioManager.main.buttonOffClip;
        audioSource.Play ();
    }
}
