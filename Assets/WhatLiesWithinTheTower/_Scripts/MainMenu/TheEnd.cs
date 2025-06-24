using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TheEnd : MonoBehaviour
{
    public Image theEndImage;
    public GameObject timer;
    public float delayBeforequit = 3f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;
        else
        {
            hasTriggered = true;
            timer.SetActive(false);
            theEndImage.gameObject.SetActive(true);

            Invoke("QuitGame", delayBeforequit);
        }
    }

    private void QuitGame()
    {
        Application.Quit(); 
    }
}
