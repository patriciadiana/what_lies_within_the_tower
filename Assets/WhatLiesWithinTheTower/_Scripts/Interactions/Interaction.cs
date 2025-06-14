using UnityEngine;

public class Interaction : MonoBehaviour
{
    public GameObject player;
    public GameObject doorText;
    public GameObject noteText;
    public GameObject keyText;
    public GameObject potionText;
    public GameObject puzzleText;
    public GameObject morphText;
    public GameObject note;

    public LayerMask doorLayer;
    public LayerMask towerDoorLayer;
    public LayerMask cabinDoorLayer;
    public LayerMask noteLayer;
    public LayerMask towerKeyLayer;
    public LayerMask puzzleLayer;
    public LayerMask potionLayer;
    public LayerMask morphLayer;
    public LayerMask bookLayer;
    public LayerMask slimeLayer;
    public LayerMask padlockLayer;

    public Transform orientation;

    private Camera mainCamera;
    private Vector3 rayOrigin;
    private Vector3 rayDirection;

    public Animator doorAnimator;

    private int rayLength = 3;

    private Inventory playerInventory;

    private void Start()
    {
        mainCamera = Camera.main;
        playerInventory = Inventory.Instance;
    }

    private void setTextActive()
    {
        RaycastHit info;
        LayerMask combinedMask = doorLayer | towerDoorLayer | cabinDoorLayer | noteLayer
            | towerKeyLayer | potionLayer | puzzleLayer | morphLayer | bookLayer | slimeLayer | padlockLayer;

        doorText.SetActive(false);
        noteText.SetActive(false);
        keyText.SetActive(false);
        puzzleText.SetActive(false);
        potionText.SetActive(false);
        morphText.SetActive(false);

        if (Physics.Raycast(rayOrigin, rayDirection, out info, rayLength, combinedMask))
        {
            if (((1 << info.collider.gameObject.layer) & doorLayer) != 0
                || ((1 << info.collider.gameObject.layer) & towerDoorLayer) != 0
                || ((1 << info.collider.gameObject.layer) & cabinDoorLayer) != 0)
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
            else if (((1 << info.collider.gameObject.layer) & potionLayer) != 0)
            {
                potionText.SetActive(true);
            }
            else if (((1 << info.collider.gameObject.layer) & puzzleLayer) != 0)
            {
                puzzleText.SetActive(true);
            }
            else if (((1 << info.collider.gameObject.layer) & morphLayer) != 0)
            {
                puzzleText.SetActive(true);
            }
            else if (((1 << info.collider.gameObject.layer) & bookLayer) != 0)
            {
                morphText.SetActive(true);
            }
            else if (((1 << info.collider.gameObject.layer) & slimeLayer) != 0)
            {
                puzzleText.SetActive(true);
            }
            else if (((1 << info.collider.gameObject.layer) & padlockLayer) != 0)
            {
                if (playerInventory.HasItem("Potion"))
                {
                    puzzleText.SetActive(false);
                }
                else
                {
                    puzzleText.SetActive(true);
                }
            }
        }
    }

    private void InteractionWithObject()
    {
        RaycastHit info;
        LayerMask combinedMask = doorLayer | towerDoorLayer | cabinDoorLayer | noteLayer | towerKeyLayer 
            | potionLayer | puzzleLayer | morphLayer | bookLayer | slimeLayer | padlockLayer;

        if (Physics.Raycast(rayOrigin, rayDirection, out info, rayLength, combinedMask))
        {
            if (((1 << info.collider.gameObject.layer) & towerKeyLayer) != 0)
            {
                playerInventory.AddItem("TowerKey");
                SoundManager.PlaySound(SoundType.PICKUPKEY, 0.9f);
                Destroy(info.collider.gameObject);
            }
            else if (((1 << info.collider.gameObject.layer) & potionLayer) != 0)
            {
                playerInventory.AddItem("Potion");
                SoundManager.PlaySound(SoundType.PICKUPPOTION, 0.2f);
                Destroy(info.collider.gameObject);
            }
            else if (((1 << info.collider.gameObject.layer) & doorLayer) != 0
                || ((1 << info.collider.gameObject.layer) & towerDoorLayer) != 0
                || ((1 << info.collider.gameObject.layer) & cabinDoorLayer) != 0)
            {
                DoorController door = info.collider.GetComponent<DoorController>();
                if (door != null)
                {
                    if (door.doorType == DoorController.DoorType.Tower && !playerInventory.HasItem("TowerKey"))
                    {
                        SoundManager.PlaySound(SoundType.LOCKEDDOOR, 0.3f);
                        doorAnimator.Play("LockedDoor", 0, 0f);
                    }
                    else if (door.doorType == DoorController.DoorType.Tower && playerInventory.HasItem("TowerKey"))
                    {
                        playerInventory.RemoveItem("TowerKey");
                        door.Interact();
                    }
                    else
                    {
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
            else if (((1 << info.collider.gameObject.layer) & morphLayer) != 0)
            {
                StartMorph morphPuzzle = info.collider.GetComponent<StartMorph>();
                if (morphPuzzle != null)
                {
                    morphPuzzle.Interact();
                }
            }
            else if (((1 << info.collider.gameObject.layer) & bookLayer) != 0)
            {
                if (Input.GetKeyDown(KeyCode.M))
                {
                    if (playerInventory.HasItem("Potion"))
                    {
                        playerInventory.RemoveItem("Potion");
                    }
                }
            }
            else if (((1 << info.collider.gameObject.layer) & slimeLayer) != 0)
            {
                Start2DPuzzle slime = info.collider.GetComponent<Start2DPuzzle>();
                if (slime != null)
                {
                    slime.Interact();
                }
            }
            else if (((1 << info.collider.gameObject.layer) & padlockLayer) != 0)
            {
                SolvePadlock padlock = info.collider.GetComponent<SolvePadlock>();
                if (padlock != null)
                {
                    padlock.Interact();
                }
            }
        }
    }

    private void Update()
    {
        rayOrigin = mainCamera.transform.position;
        rayDirection = mainCamera.transform.forward;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.M))
        {
            InteractionWithObject();
        }
        setTextActive();
    }
}
