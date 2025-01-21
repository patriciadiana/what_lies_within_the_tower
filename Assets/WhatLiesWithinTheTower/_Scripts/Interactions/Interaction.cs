using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Interaction : MonoBehaviour
{
    public GameObject player;
    public GameObject doorText;
    public GameObject noteText;
    public GameObject keyText;
    public GameObject puzzleText;
    public GameObject note;

    public LayerMask doorLayer;
    public LayerMask towerDoorLayer;
    public LayerMask noteLayer;
    public LayerMask towerKeyLayer;
    public LayerMask puzzleLayer;

    public Transform orientation;

    private Camera mainCamera;
    private Vector3 rayOrigin;
    private Vector3 rayDirection;

    private int rayLength = 3;

    private Inventory playerInventory;

    private void Start()
    {
        mainCamera = Camera.main;
        playerInventory = player.GetComponent<Inventory>();
    }

    private void setTextActive()
    {
        RaycastHit info;
        LayerMask combinedMask = doorLayer | towerDoorLayer | noteLayer | towerKeyLayer | puzzleLayer;

        doorText.SetActive(false);
        noteText.SetActive(false);
        keyText.SetActive(false);
        puzzleText.SetActive(false);

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
            else if (((1 << info.collider.gameObject.layer) & towerKeyLayer) != 0)
            {
                keyText.SetActive(true);
            }
            else if (((1 << info.collider.gameObject.layer) & puzzleLayer) != 0)
            {
                puzzleText.SetActive(true);
            }
        }
    }

    private void InteractionWithObject()
    {
        RaycastHit info;
        LayerMask combinedMask = doorLayer | towerDoorLayer | noteLayer | towerKeyLayer | puzzleLayer;

        if (Physics.Raycast(rayOrigin, rayDirection, out info, rayLength, combinedMask))
        {
            if (((1 << info.collider.gameObject.layer) & towerKeyLayer) != 0)
            {
                playerInventory.AddItem("TowerKey");
                Destroy(info.collider.gameObject);
            }
            else if (((1 << info.collider.gameObject.layer) & doorLayer) != 0
                || ((1 << info.collider.gameObject.layer) & towerDoorLayer) != 0)
            {
                DoorController door = info.collider.GetComponent<DoorController>();
                if (door != null)
                {
                    if (door.doorType == DoorController.DoorType.Tower && !playerInventory.HasItem("TowerKey"))
                    {
                        Debug.Log("You need the tower key to open this door!");
                    }
                    else
                    {
                        playerInventory.RemoveItem("TowerKey");
                        door.Interact();
                    }
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
            else if (((1 << info.collider.gameObject.layer) & puzzleLayer) != 0)
            {
                StartPuzzle puzzle = info.collider.GetComponent<StartPuzzle>();
                if (puzzle != null)
                {
                    puzzle.Interact();
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
        Gizmos.color = Color.red; // Set the color of the gizmo ray.
        Gizmos.DrawRay(rayOrigin, rayDirection);
    }
}
