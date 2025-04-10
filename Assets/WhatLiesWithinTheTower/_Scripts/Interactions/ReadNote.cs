using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadNote : MonoBehaviour
{
    public GameObject player;
    public GameObject note;

    public GameObject objectiveUI;
    private Animator objectiveAnimator;

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
    }

    void Update()
    {
        if (isNoteActive && Input.GetKeyDown(KeyCode.Q))
        {
            CloseNote();
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
