using UnityEngine;

public class DeletePlayerInScene : MonoBehaviour
{
    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Destroy(player);
            Debug.Log("Player deleted from scene.");
        }
        else
        {
            Debug.LogWarning("No Player found in the scene.");
        }
    }
}
