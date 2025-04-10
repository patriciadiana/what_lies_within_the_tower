using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveUIPersist : MonoBehaviour
{
    private static ObjectiveUIPersist Instance;
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
