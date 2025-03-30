//using UnityEngine;

//public class DialougeTrigger : MonoBehaviour
//{
//    //Temp Code, will move.

//    [SerializeField]
//    GameObject popup;

//    [SerializeField]
//    TextAsset inkAsset;

//    bool bplayerInRange = false;

//    [SerializeField]
//    InputReader input;

//    void Start()
//    {
//        popup.SetActive(false);

//        input.PirateInteractEvent += HandleInteract;
//    }

//    void OnDestroy()
//    {
//        input.PirateInteractEvent -= HandleInteract;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if(other.gameObject.tag != "Player")
//        {
//            return;
//        }

//        popup.SetActive(true);
//        bplayerInRange = true;

//    }

//    private void OnTriggerExit(Collider other)
//    {
//        if (other.gameObject.tag != "Player")
//        {
//            return;
//        }

//        popup.SetActive(false);
//        bplayerInRange = false;
//    }

//    void HandleInteract(float val)
//    {
//        if(bplayerInRange && !DialougeManager.instance.bDialoguePlaying)
//        {
//            DialougeManager.instance.TriggerDialogue(inkAsset);
//        }
//    }
//}
