using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : MonoBehaviour {

    public enum TriggerType { Player, Creature };
    public enum BehaviourTypes { OpenDoor, LowerLadder, ActivateObject, DeactivateObject }

    #region Public Variables
    [Header ("Behaviour")]
    public BehaviourTypes behaviourType; 
    public TriggerType triggerType;

    public bool isActivated;

    public Interactable interactObject;
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
    #endregion

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer> ();
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
                    isActivated = true;
                }
            }
           
        }
        OnStep();
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
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
        }

    }

    private void ActivateObject(Interactable objToActivate)
    {
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
        spriteRenderer.sprite = activatedSprite;
        child.localPosition = new Vector3 (child.localPosition.x, childOffsetY.x, child.localPosition.z);
    }

    private void OnRelease()
    {
        spriteRenderer.sprite = deactivatedSprite;
        child.localPosition = new Vector3 (child.localPosition.x, childOffsetY.y, child.localPosition.z);

        if(behaviourType == BehaviourTypes.LowerLadder)
        {
            interactObject.gameObject.GetComponent<Ladder>().Deactivate();
        }
    }
}
