using System.Collections.Generic;
using UnityEngine;

//NEEDS MASSIVE REFACTORING

public class ShipManager : MonoBehaviour
{

    [SerializeField] GameObject pirateObj;
    [SerializeField] GameObject pirateLockPos;
    [SerializeField] GameObject pirateCamera;
    [SerializeField] GameObject shipObj;
    [SerializeField] GameObject shipCamera;

    Rigidbody shipRB; 
    Rigidbody pirateRB; 

    ShipMovementContinous shipMovement;
    PirateMovement pirateMovement;

    List<BoxCollider> colliderList = new List<BoxCollider>();


    private void Start()
    {
        shipCamera.SetActive(false);
        pirateCamera.SetActive(true);
        PlayerController.instance.PirateControllerEvent += HandlePirateEvent;
        PlayerController.instance.ShipControllerEvent += HandleShipEvent;

        BoxCollider[] boxCol;
        boxCol = shipObj.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider col in boxCol)
        {
            colliderList.Add(col);
        }
        shipMovement = shipObj.GetComponent<ShipMovementContinous>();
        pirateMovement = pirateObj.GetComponent<PirateMovement>();
        shipRB = shipObj.GetComponent<Rigidbody>();
        pirateRB = pirateObj.GetComponent<Rigidbody>();
        shipRB.isKinematic = true;
        shipMovement.enabled = false;

    }

    private void OnDisable()
    {
        PlayerController.instance.PirateControllerEvent -= HandlePirateEvent;
        PlayerController.instance.ShipControllerEvent -= HandleShipEvent;
    }
    void HandleShipEvent()
    {
        pirateCamera.SetActive(false);
        shipCamera.SetActive(true);

        foreach (BoxCollider col in colliderList)
        {
            col.enabled = false;
        }

        pirateMovement.enabled = false;
        pirateObj.transform.SetParent(pirateLockPos.transform);
        shipMovement.enabled = true;

        pirateRB.isKinematic = true;
        pirateRB.detectCollisions = false;
        shipRB.isKinematic = false;

        pirateMovement.enabled = false;
    }

    void HandlePirateEvent()
    {
        shipCamera.SetActive(false);
        pirateCamera.SetActive(true);

        foreach (BoxCollider col in colliderList)
        {
            col.enabled = true;
        }

        pirateRB.isKinematic = false;
        pirateRB.detectCollisions = true;
        shipRB.isKinematic = true;

        shipMovement.enabled = false;
        pirateObj.transform.SetParent(null);
        pirateMovement.enabled = true;

    }
}
