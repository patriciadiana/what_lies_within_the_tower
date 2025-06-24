using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void SaveCurrentLevelTime()
    {
        if (Timer.Instance == null)
            return;

        float timeInSeconds = Timer.Instance.currentTime;

        string folderPath = Path.Combine(Application.persistentDataPath, "CSV");
        string filePath = Path.Combine(folderPath, "TimerLog.csv");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        bool fileExists = File.Exists(filePath);

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            if (!fileExists)
            {
                writer.WriteLine("Level Name,Time (MM:SS),Time (Seconds)");
            }

            int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
            string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            string levelName = SceneManager.GetActiveScene().name;

            writer.WriteLine($"{levelName},{formattedTime},{timeInSeconds:F2}");
        }

        Debug.Log($"Timer saved to CSV at: {filePath}");
    }
}
