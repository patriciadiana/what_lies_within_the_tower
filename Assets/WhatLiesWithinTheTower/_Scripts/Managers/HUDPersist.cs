using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPersist : MonoBehaviour
{
    private static HUDPersist Instance;

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
}
