using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start2DPuzzle : MonoBehaviour
{
    public GameObject player;

    public void Interact()
    {
        GameManager.Instance.SetPlayerPosition(player.transform.position);
        SceneManager.LoadScene("2DPuzzle");
    }
}
