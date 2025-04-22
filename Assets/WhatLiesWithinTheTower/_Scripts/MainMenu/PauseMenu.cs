using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    public GameObject player;

    public static bool isPaused = false;
    public GameObject pauseMenu;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        isPaused = false;
        GameManager.Instance.SetSceneName(SceneManager.GetActiveScene().name);
        GameManager.Instance.SetPlayerPosition(player.transform.position);
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveGame()
    {
        SaveSystem.SavePlayer(player);
        Debug.Log("Game Saved");
        Resume(); 
    }

    public void LoadGame()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data != null)
        {
            player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            Inventory.Instance.LoadInventory(data.inventoryData);
            Debug.Log("Game Loaded");
            Resume();
        }
        else
        {
            Debug.LogError("No saved game found.");
        }
    }
}
