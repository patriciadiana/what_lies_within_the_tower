using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public static GameManager Instance;
    private string lastSceneName;
    private GameObject player;

    private Vector3 playerPosition;
    private Vector3 spawnPosition = new Vector3(-2.53999996f, 1.83899999f, 0.00999999978f);
    private bool hasSavedPosition = false;
    private bool isLockOpened = false;

    public GameObject hatchLevel1;
    public GameObject hatchLevel2;
    public GameObject hatchLevel3;
    public GameObject hatchLevel4;

    private bool activateLevel1 = false;
    private bool activateLevel2 = false;
    private bool activateLevel3 = false;
    private bool activateLevel4 = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (scene.name == "MorphPuzzle")
            {
                player.SetActive(true);
                rb.MovePosition(spawnPosition);
            }
            else if (scene.name == "MainScene" && hasSavedPosition)
            {
                ActivateUnlockedHatches();
                rb.MovePosition(playerPosition);
            }
            else if (scene.name == "PadLockScene")
            {
                player.SetActive(false);
            }
        }
    }

    public void SetPlayerPosition(Vector3 position)
    {
        playerPosition = position;
        hasSavedPosition = true;
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }

    public void SetSceneName(string sceneName)
    {
        lastSceneName = sceneName;
    }

    public string GetSceneName()
    {
        return lastSceneName; 
    }

    public void ActivateUnlockedHatches()
    {
        if (activateLevel1 && hatchLevel1 != null)
        {
            hatchLevel1.SetActive(true);
        }
        if (activateLevel2 && hatchLevel2 != null)
        {
            SoundManager.PlaySound(SoundType.UNLOCKHATCH, 0.3f);
            hatchLevel2.SetActive(false);
        }
        if (activateLevel3 && hatchLevel3 != null)
        {
            hatchLevel2.SetActive(true);
            SoundManager.PlaySound(SoundType.UNLOCKHATCH, 0.3f);
            hatchLevel3.SetActive(false);
        }
        if (activateLevel4 && hatchLevel4 != null)
        {
            hatchLevel3.SetActive(true);
            SoundManager.PlaySound(SoundType.UNLOCKHATCH, 0.3f);
            hatchLevel4.SetActive(false);
        }
    }

    public void SetLevelComplete(int level)
    {
        switch (level)
        {
            case 1: activateLevel1 = true; break;
            case 2: activateLevel2 = true; break;
            case 3: activateLevel3 = true; break;
            case 4: activateLevel4 = true; break;
        }
    }

    public void SetLockOpened(bool opened)
    {
        isLockOpened = opened;
    }

    public bool GetLockOpened()
    {
        return isLockOpened;
    }

    public void LoadSavedScene()
    {
        if (!string.IsNullOrEmpty(lastSceneName))
        {
            SceneManager.LoadScene(lastSceneName);
        }
        else
        {
            Debug.LogWarning("No scene name saved!");
        }
    }

}
