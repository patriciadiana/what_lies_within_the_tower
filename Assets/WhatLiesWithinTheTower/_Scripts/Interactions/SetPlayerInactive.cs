using UnityEngine;

public class SetPlayerInactive : MonoBehaviour
{
    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Destroy(player);
        }
        else
        {
            Debug.LogWarning("No Player found in the scene.");
        }
    }
}
