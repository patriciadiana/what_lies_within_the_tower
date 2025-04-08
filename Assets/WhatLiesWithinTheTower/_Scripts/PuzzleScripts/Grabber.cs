using UnityEngine;
using UnityEngine.SceneManagement;

public class Grabber : MonoBehaviour
{
    private GameObject selectedObject;
    private Vector3 offset;

    private static int placedPiecesCount = 0;
    private static int totalPieces = 10;

    private void Start()
    {
        placedPiecesCount = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                RaycastHit hit = CastRay();
                if (hit.collider != null && hit.collider.CompareTag("drag"))
                {
                    SoundManager.PlaySound(SoundType.GRABPIECE, 0.9f);
                    selectedObject = hit.collider.gameObject;

                    Vector3 objectScreenPos = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
                    offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(
                        new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectScreenPos.z));

                    Cursor.visible = false;
                }
            }
            else
            {
                if (selectedObject.GetComponent<SnapScript>().IsPiecePlaced())
                {
                    SoundManager.PlaySound(SoundType.SNAPPIECE, 1f);
                    placedPiecesCount++;
                    CheckPuzzleCompletion();
                }
                selectedObject = null;
                Cursor.visible = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("MainScene");
        }

        if (selectedObject != null)
        {
            Vector3 objectScreenPos = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectScreenPos.z));
            selectedObject.transform.position = mouseWorldPosition + offset;

            if (Input.GetMouseButtonDown(1))
            {
                SoundManager.PlaySound(SoundType.GRABPIECE, 0.9f);
                selectedObject.transform.rotation = Quaternion.Euler(new Vector3(
                    selectedObject.transform.rotation.eulerAngles.x,
                    selectedObject.transform.rotation.eulerAngles.y + 90,
                    selectedObject.transform.rotation.eulerAngles.z));
            }
        }
    }

    private RaycastHit CastRay()
    {
        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);

        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out RaycastHit hit);
        return hit;
    }

    private void CheckPuzzleCompletion()
    {
        if (placedPiecesCount == totalPieces)
        {
            GameManager.Instance.SetLevelComplete(1);
            GameManager.Instance.SetLevelComplete(2);
            SceneManager.LoadScene("MainScene");
            placedPiecesCount = 0;
        }
    }
}
