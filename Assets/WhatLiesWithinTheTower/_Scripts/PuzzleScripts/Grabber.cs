using UnityEngine;
using UnityEngine.SceneManagement;

public class Grabber : MonoBehaviour
{
    private GameObject selectedObject;
    private Vector3 offset;

    private static int placedPiecesCount = 0;
    private static int totalPieces = 1;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        // Your existing code for puzzle mechanics...
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                RaycastHit hit = CastRay();
                if (hit.collider != null && hit.collider.CompareTag("drag"))
                {
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
                    placedPiecesCount++;
                    CheckPuzzleCompletion();
                }
                selectedObject = null;
                Cursor.visible = true;
            }
        }

        if (selectedObject != null)
        {
            Vector3 objectScreenPos = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectScreenPos.z));
            selectedObject.transform.position = mouseWorldPosition + offset;

            if (Input.GetMouseButtonDown(1))
            {
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
            SceneManager.LoadScene("MainScene");
        }
    }
}
