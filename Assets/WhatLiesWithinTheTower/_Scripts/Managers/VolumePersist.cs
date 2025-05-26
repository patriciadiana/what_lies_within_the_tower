using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumePersist : MonoBehaviour
{
    private static VolumePersist Instance;

    void Awake()
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
