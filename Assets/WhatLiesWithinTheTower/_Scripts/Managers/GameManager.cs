using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Vector3 playerPosition = Vector3.zero;

    private void Awake()
    {
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

    public void SetPlayerPosition(Vector3 position)
    {
        playerPosition = position;
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPosition;
    }
}
