using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    public bool isFinished = false;

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

    public void MarkObjectiveAsFinished()
    {
        isFinished = true;
    }

    public void ResetObjectiveFlag()
    {
        isFinished = false;
    }
}
