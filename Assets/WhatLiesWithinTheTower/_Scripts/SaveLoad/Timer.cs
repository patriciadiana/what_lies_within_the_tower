using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance;

    [Header("Component")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    public float currentTime;

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

    private void Update()
    {
        currentTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime % 60F);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
