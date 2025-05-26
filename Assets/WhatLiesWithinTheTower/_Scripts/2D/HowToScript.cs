using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToScript : MonoBehaviour
{
    public GameObject howToPanel;
    public float closeTime = 15f;

    private void Start()
    {
        ShowPanel();
    }

    public void ShowPanel()
    {
        howToPanel.SetActive(true);
        Time.timeScale = 0f;
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(closeTime);
        HidePanel();
    }

    public void HidePanel()
    {
        howToPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}