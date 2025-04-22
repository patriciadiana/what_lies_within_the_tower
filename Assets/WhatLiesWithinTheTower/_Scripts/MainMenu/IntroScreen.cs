using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroScreen : MonoBehaviour
{
    public Image introImage;
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
            StartCoroutine(PlayIntro());
        }
    }

    IEnumerator PlayIntro()
    {
        introImage.gameObject.SetActive(true);

        Color color = introImage.color;
        color.a = 0f;
        introImage.color = color;

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, timer / fadeInDuration);
            color.a = t;
            introImage.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(1f, 0f, timer / fadeOutDuration);
            color.a = t;
            introImage.color = color;
            yield return null;
        }

        introImage.gameObject.SetActive(false);
        hasPlayed = false;
    }
}
