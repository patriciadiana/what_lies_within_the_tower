using UnityEngine;

public class MorphObject : MonoBehaviour
{
    public GameObject player;
    public GameObject morphObject;

    public Camera playerCamera;
    public Camera morphCamera;

    private bool isMorphed = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        morphCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SwapForm();
        }
    }
    private void SwapForm()
    {
        if (!isMorphed)
        {
            morphObject.transform.position = player.transform.position;

            morphObject.SetActive(true);
            morphCamera.gameObject.SetActive(true);

            player.SetActive(false);
            playerCamera.gameObject.SetActive(false);

            isMorphed = true;
        }
        else
        {
            player.transform.position = morphObject.transform.position;

            player.SetActive(true);
            playerCamera.gameObject.SetActive(true);

            morphObject.SetActive(false);
            morphCamera.gameObject.SetActive(false);

            isMorphed = false;
        }
    }
}
