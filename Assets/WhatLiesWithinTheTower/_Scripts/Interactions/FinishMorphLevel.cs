using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishMorphLevel : MonoBehaviour
{
    public GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.SetLevelComplete(3);
        SceneManager.LoadScene("MainScene");
    }
}
