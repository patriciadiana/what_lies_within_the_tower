using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class OutroScreen : MonoBehaviour
{
    public Image outroImage1;
    public Image outroImage2;

    public float fadeInDuration = 4f;
    public float displayTime = 3f;
    public float fadeOutDuration = 4f;

    private bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed)
        {
            hasPlayed = true;
            GetComponent<Collider>().enabled = false;
            StartCoroutine(PlayOutro());
        }
    }

    IEnumerator PlayOutro()
    {
        yield return StartCoroutine(FadeImageSequence(outroImage1));

        yield return StartCoroutine(FadeImageSequence(outroImage2));

        yield return new WaitForSeconds(2f);

        if (Timer.Instance != null)
        {
            SaveTimerToCSV(Timer.Instance.currentTime);
        }

        hasPlayed = false;
    }

    IEnumerator FadeImageSequence(Image image)
    {
        image.gameObject.SetActive(true);

        Color color = image.color;
        color.a = 0f;
        image.color = color;

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, timer / fadeInDuration);
            color.a = t;
            image.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(1f, 0f, timer / fadeOutDuration);
            color.a = t;
            image.color = color;
            yield return null;
        }

        image.gameObject.SetActive(false);
    }

    void SaveTimerToCSV(float timeInSeconds)
    {
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
                writer.WriteLine("Time (MM:SS),Time (Seconds)");
            }

            int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60F);
            string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            writer.WriteLine($"{formattedTime},{timeInSeconds:F2}");
        }

        Debug.Log($"Timer saved to CSV at: {filePath}");
    }

}
