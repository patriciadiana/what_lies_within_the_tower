using UnityEngine;

public class SnapScript : MonoBehaviour
{
    public float snapOffset;
    private Vector3 RightPosition;
    private Quaternion initialRotation;

    void Start()
    {
        RightPosition = transform.position;
        initialRotation = transform.rotation;

        transform.position = new Vector3(Random.Range(-7f, 9f), 0f, Random.Range(2f, 9f));

        float randomYRotation = 90f * Random.Range(0, 4);
        transform.rotation = Quaternion.Euler(
            initialRotation.eulerAngles.x,
            randomYRotation,
            initialRotation.eulerAngles.z
        );
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, RightPosition) <= snapOffset &&
            Quaternion.Angle(transform.rotation, initialRotation) < 1f)
        {
            transform.position = RightPosition;
            transform.rotation = initialRotation;
        }
    }

    public bool IsPiecePlaced()
    {
        return Vector3.Distance(transform.position, RightPosition) <= snapOffset &&
               Quaternion.Angle(transform.rotation, initialRotation) < 1f;
    }
}
