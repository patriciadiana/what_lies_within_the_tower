using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatchPersist : MonoBehaviour
{
    private static HatchPersist Instance;

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
