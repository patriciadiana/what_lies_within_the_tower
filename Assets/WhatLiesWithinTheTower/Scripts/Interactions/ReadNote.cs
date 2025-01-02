using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadNote : MonoBehaviour
{
    public GameObject player;
    public GameObject note;

    private bool isNoteActive = false;

    void Start()
    {
        note.SetActive(false);
    }

    public void Interact()
    {
        note.SetActive(true);
        player.GetComponent<PlayerMovement>().enabled = false;
        isNoteActive = true;
    }

    void Update()
    {
        if (isNoteActive && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNote();
        }
    }

    private void CloseNote()
    {
        note.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;
        isNoteActive = false;
    }
}
