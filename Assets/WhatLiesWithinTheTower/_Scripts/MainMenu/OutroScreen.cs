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
            Timer.Instance.SaveCurrentLevelTime();
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
}
