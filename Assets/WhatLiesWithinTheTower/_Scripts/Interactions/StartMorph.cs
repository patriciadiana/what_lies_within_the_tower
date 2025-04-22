using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMorph : MonoBehaviour
{
    public GameObject player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void Interact()
    {
        GameManager.Instance.SetSceneName(SceneManager.GetActiveScene().name);
        GameManager.Instance.SetPlayerPosition(player.transform.position);
        SceneManager.LoadScene("MorphPuzzle");
    }
}
