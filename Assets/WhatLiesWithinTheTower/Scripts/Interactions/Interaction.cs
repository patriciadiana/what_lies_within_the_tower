using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Interaction : MonoBehaviour
{
    public GameObject player;
    public GameObject doorText;
    public GameObject noteText;
    public GameObject note;

    public LayerMask doorLayer;
    public LayerMask towerDoorLayer;
    public LayerMask noteLayer;

    public Transform orientation;

    private Camera mainCamera;
    private Vector3 rayOrigin;
    private Vector3 rayDirection;

    private int rayLength = 3;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void setTextActive()
    {
        RaycastHit info;
        LayerMask combinedMask = doorLayer | towerDoorLayer | noteLayer;

        doorText.SetActive(false);
        noteText.SetActive(false);

        if (Physics.Raycast(rayOrigin, rayDirection, out info, rayLength, combinedMask))
        {
            if (((1 << info.collider.gameObject.layer) & doorLayer) != 0
                || ((1 << info.collider.gameObject.layer) & towerDoorLayer) != 0)
            {
                doorText.SetActive(true);
            }
            else if (((1 << info.collider.gameObject.layer) & noteLayer) != 0)
            {
                noteText.SetActive(true);
            }
        }
    }

    private void InteractionWithObject()
    {
        RaycastHit info;
        LayerMask combinedMask = doorLayer | towerDoorLayer | noteLayer;

        if (Physics.Raycast(rayOrigin, rayDirection, out info, rayLength, combinedMask))
        {
            if (((1 << info.collider.gameObject.layer) & doorLayer) != 0
                || ((1 << info.collider.gameObject.layer) & towerDoorLayer) != 0)
            {
                DoorController door = info.collider.GetComponent<DoorController>();
                if (door != null)
                {
                    door.Interact();
                }
            }
            else if (((1 << info.collider.gameObject.layer) & noteLayer) != 0)
            {
                ReadNote note = info.collider.GetComponent<ReadNote>();
                if (note != null)
                {
                    note.Interact();
                }
            }
        }
    }

    private void Update()
    {
        rayOrigin = mainCamera.transform.position;
        rayDirection = mainCamera.transform.forward;

        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractionWithObject();
        }

        setTextActive();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rayOrigin, rayDirection * rayLength);
    }
}
