using UnityEngine;
using UnityEngine.SceneManagement;

public class BookInteraction : MonoBehaviour
{
    public void Interact()
    {
        GameManager.Instance.SetLevelComplete(3);
        SceneManager.LoadScene("MainScene");
    }
}
