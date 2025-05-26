using UnityEngine;
using UnityEngine.SceneManagement;

public class Note : MonoBehaviour
{
    private bool playerNearby = false;

    private Inventory playerInventory;
    private int collectedNotes = 0;

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
        SoundManager.PlaySound(SoundType.PICKUP2D, 0.6f);
        collectedNotes++;
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
