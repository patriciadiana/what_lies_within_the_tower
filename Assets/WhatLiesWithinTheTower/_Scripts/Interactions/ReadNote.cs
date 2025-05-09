using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadNote : MonoBehaviour
{
    public GameObject player;
    public GameObject note;

    public GameObject objectiveUI;
    private Animator objectiveAnimator;

    private bool padlockCodeNote;

    private bool isNoteActive = false;

    void Start()
    {
        note.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        objectiveUI = GameObject.FindGameObjectWithTag("ObjectiveUI");
        objectiveAnimator = objectiveUI.GetComponent<Animator>();
    }

    public void Interact()
    {
        note.SetActive(true);
        SoundManager.PlaySound(SoundType.PICKUPNOTE, 1f);
        player.GetComponent<PlayerMovement>().enabled = false;
        objectiveAnimator.Play("ObjectiveHide");
        isNoteActive = true;

        if (note.CompareTag("PadlockCode"))
            padlockCodeNote = true;
        else
            padlockCodeNote = false;
    }

    void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (objectiveUI == null)
        {
            objectiveUI = GameObject.FindGameObjectWithTag("ObjectiveUI");
            if (objectiveUI != null)
                objectiveAnimator = objectiveUI.GetComponent<Animator>();
        }

        if (isNoteActive && Input.GetKeyDown(KeyCode.Q))
        {
            CloseNote();
            if ((ObjectiveManager.Instance != null) && (padlockCodeNote == false))
                ObjectiveManager.Instance.MarkObjectiveAsFinished();
        }
    }


    private void CloseNote()
    {
        note.SetActive(false);
        player.GetComponent<PlayerMovement>().enabled = true;
        isNoteActive = false;
    }

    public bool IsNoteActive()
    {
        return isNoteActive;
    }
}
