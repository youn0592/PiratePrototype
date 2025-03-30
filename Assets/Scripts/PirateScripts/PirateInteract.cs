using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PirateInteract : MonoBehaviour
{
    //TODO - Have POPUP rotate with the camera so its always facing, account for multiple NPC's entering the trigger


    [SerializeField] GameObject popUp;

    bool bIsNpcInteract = false;
    bool bIsObjectInteractable= false;

    GameObject interactingObject;   


    private void OnEnable()
    {
        popUp.SetActive(false);
        GameEventManager.instance.inputEvents.PirateInteractEvent += InteractPressed;
    }

    private void OnDisable()
    {
        GameEventManager.instance.inputEvents.PirateInteractEvent -= InteractPressed;
    }
    void InteractPressed(float val)
    {
        if(bIsObjectInteractable)
        {
            InteractWithWheel();
        }
        if(bIsNpcInteract)
        {
            InteractWithNPC();
        }
    }

    void InteractWithNPC()
    {
        if(interactingObject == null)
        {
            Debug.LogError("Interacting NPC was null");
            return;
        }
        QuestPoint quest = interactingObject.GetComponent<QuestPoint>();
        if(quest == null)
        {
            Debug.LogError("QuestPoint was null on NPC");
            return;
        }
        quest.InteractEngaged();
    }

    void InteractWithWheel()
    {
        if(interactingObject == null)
        {
            Debug.LogError("Interacting NPC was null");
            return;
        }

        PlayerController.instance.SetShipController();
    }

    private void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("NPC"))
        {
            popUp.SetActive(true);
            bIsNpcInteract = true;
            interactingObject = other.gameObject;
        }
       if(other.CompareTag("Wheel"))
        {
            popUp.SetActive(true);
            bIsObjectInteractable = true;
            interactingObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            popUp.SetActive(false);
            bIsNpcInteract = false;
            interactingObject = null;
        }
        if(other.CompareTag("Wheel"))
        {
            popUp.SetActive(false);
            bIsObjectInteractable = false;
            interactingObject = null;
        }
    }
}
