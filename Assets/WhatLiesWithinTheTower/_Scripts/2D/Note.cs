using UnityEngine;

public class Note : MonoBehaviour
{
    private bool playerNearby = false;

    private Inventory playerInventory;

    private void Start()
    {
        playerInventory = Inventory.Instance;
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            CollectNote();
        }
    }

    private void CollectNote()
    {
        playerInventory.AddItem("2DNote");
        Debug.Log("Note collected!");
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player2D"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player2D"))
        {
            playerNearby = false;
        }
    }
}
