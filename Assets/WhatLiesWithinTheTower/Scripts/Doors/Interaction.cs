using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public GameObject player;
    public GameObject doorText;

    public LayerMask doorLayer;
    public LayerMask towerDoorLayer;

    public Transform orientation;

    private void setTextActive()
    {
        RaycastHit info;
        LayerMask doorsMask = doorLayer | towerDoorLayer;

        doorText.SetActive(false);

        if(Physics.Raycast(transform.position, orientation.forward, out info, 3, doorsMask))
        {
            doorText.SetActive(true);
        }
    }

    private void InteractionWithObject()
    {
        RaycastHit info;
        LayerMask doorsMask = doorLayer | towerDoorLayer;

        if (Physics.Raycast(transform.position, orientation.forward, out info, 3, doorsMask))
        {
            DoorController door = info.collider.GetComponent<DoorController>();
            if (door != null)
            {
                door.Interact();
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            InteractionWithObject();
        }

        setTextActive();
    }
}
