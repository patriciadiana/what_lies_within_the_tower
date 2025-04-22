using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPuzzle : MonoBehaviour
{
    public GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Interact()
    {
        GameManager.Instance.SetPlayerPosition(player.transform.position);
        SceneManager.LoadScene("Puzzle");
    }
}
