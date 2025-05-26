using UnityEngine;
using UnityEngine.SceneManagement;

public class Grabber : MonoBehaviour
{
    private GameObject selectedObject;
    private Vector3 offset;
    private bool isDragging = false;

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
            if (!isDragging)
            {
                RaycastHit hit = CastRay();
                if (hit.collider != null && hit.collider.CompareTag("drag"))
                {
                    SoundManager.PlaySound(SoundType.GRABPIECE, 0.9f);
                    selectedObject = hit.collider.gameObject;
                    isDragging = true;

                    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
                        new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                        Camera.main.WorldToScreenPoint(selectedObject.transform.position).z));
                    offset = selectedObject.transform.position - mouseWorldPos;

                    Cursor.lockState = CursorLockMode.Confined;
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
                ReleaseObject();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("MainScene");
        }

        if (isDragging && selectedObject != null)
        {
            Vector3 mouseScreenPos = new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            selectedObject.transform.position = mouseWorldPos + offset;

            if (Input.GetMouseButtonDown(1))
            {
                SoundManager.PlaySound(SoundType.GRABPIECE, 0.9f);
                selectedObject.transform.Rotate(0, 90, 0, Space.World);
            }
        }
    }

    private void ReleaseObject()
    {
        isDragging = false;
        selectedObject = null;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private RaycastHit CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
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

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && isDragging)
        {
            ReleaseObject();
        }
    }
}