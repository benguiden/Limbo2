using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : MonoBehaviour {

    public enum TriggerType { Player, Creature };
    public enum BehaviourTypes { OpenDoor, ActivateObject, DeactivateObject }
    public BehaviourTypes behaviourType; 
    public TriggerType triggerType;

    public bool isActivated;

    public GameObject interactObject;
    public PuzzleButton twinButton;
    
    private void OnTriggerEnter2D(Collider2D other)
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
                Debug.Log("Creature Entered");
            }
            else
            {
                Debug.Log("WRONG!");
            }
        }
        OnStep();
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnRelease();
        isActivated = false;
    }

    private void EnactBehaviour(BehaviourTypes behaviour)
    {
        switch (behaviour)
        {
            case BehaviourTypes.ActivateObject:
                ActivateObject(interactObject);
                break;
            case BehaviourTypes.DeactivateObject:
                DeActivateObject(interactObject);
                break;
            case BehaviourTypes.OpenDoor:
                OpenDoor(interactObject);
                break;
        }

    }

    private void ActivateObject(GameObject objToActivate)
    {
        objToActivate.SetActive(true);
    }

    private void DeActivateObject(GameObject objToDeactivate)
    {
        objToDeactivate.SetActive(false);
    }

    private void OpenDoor(GameObject doorToOpen)
    {
        doorToOpen.SetActive(false);
    }

    

    private void OnStep()
    {
        transform.localScale = new Vector3(1.7f, 0.1f);
    }

    private void OnRelease()
    {
        transform.localScale = new Vector3(1.7f, 0.3f);
    }
}
