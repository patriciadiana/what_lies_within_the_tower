using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapScript : MonoBehaviour
{
    public float snapOffset = 0.2f;

    private Vector3 RightPosition;

    // Start is called before the first frame update
    void Start()
    {
        RightPosition = transform.position;
        transform.position = new Vector3(Random.Range(-4f, 4f),0f, Random.Range(-0.5f, 0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, RightPosition) <= snapOffset)
        {
            transform.position = RightPosition;
        }
    }
}
