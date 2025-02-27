using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    public GameObject optionsMenu;
    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        Debug.Log("Game is quitted");
        Application.Quit();
    }

    public void OnOptionsButtonClicked()
    {
        optionsMenu.SetActive(true);
        gameObject.SetActive(false);

        OptionsMenu.Instance.SetLastMenu("MainMenu");
    }
}
