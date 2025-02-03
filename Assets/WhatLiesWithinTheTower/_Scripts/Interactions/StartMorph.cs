using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMorph : MonoBehaviour
{
    public GameObject player;
    public void Interact()
    {
        GameManager.Instance.SetPlayerPosition(player.transform.position);
        SceneManager.LoadScene("MorphPuzzle");
    }
}
