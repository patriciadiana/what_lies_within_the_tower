using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SolvePadlock : MonoBehaviour
{
    public GameObject player;

    public Animator door;
    private bool doorOpened = false;

    private void Start()
    { 
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void Interact()
    {
        SceneManager.LoadScene("PadLockScene");
    }
    private void Update()
    {
        if ((GameManager.Instance.GetLockOpened()) && (doorOpened == false))
        {
           door.Play("MorphDoorOpen");
           SoundManager.PlaySound(SoundType.OPENDOOR, 1f);
           doorOpened = true;
           Destroy(gameObject);
        }
    }
}
