using UnityEngine;

public class PlayerPositionLoader : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            Vector3 savedPosition = GameManager.Instance.GetPlayerPosition();

            if (savedPosition != Vector3.zero)
            {
                transform.position = savedPosition;
            }
        }
        else
        {
            Debug.LogWarning("GameManager instance is null!");
        }
    }
}
