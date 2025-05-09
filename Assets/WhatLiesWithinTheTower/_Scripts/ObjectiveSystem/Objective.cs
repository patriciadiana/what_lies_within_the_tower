using System.Collections;
using UnityEngine;
using TMPro;

public class Objective : MonoBehaviour
{
    public GameObject trigger;
    public GameObject objectiveUI;
    public string message = "Default objective message";
    public float displayDuration = 3f;

    private Animator objectiveAnimator;
    private TextMeshProUGUI objectiveText;
    private void Awake()
    {
        if (objectiveUI != null)
        {
            DontDestroyOnLoad(objectiveUI);
        }
    }

    private void Start()
    {
        objectiveUI = GameObject.FindGameObjectWithTag("ObjectiveUI");

        if (objectiveUI != null)
        {
            objectiveAnimator = objectiveUI.GetComponent<Animator>();
            objectiveText = objectiveUI.GetComponent<TextMeshProUGUI>();

            if (objectiveText != null)
                objectiveText.text = "";
        }

        if (trigger != null)
            trigger.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ObjectiveManager.Instance.isFinished)
        {
            ShowObjective();
        }
    }

    private void ShowObjective()
    {
        if (objectiveText != null)
        {
            objectiveText.text = message;
        }

        if (objectiveAnimator != null)
        {
            objectiveAnimator.Play("ObjectiveDisplay");
        }

        ObjectiveManager.Instance.ResetObjectiveFlag();

        if (trigger != null)
            trigger.SetActive(false);
    }
}
