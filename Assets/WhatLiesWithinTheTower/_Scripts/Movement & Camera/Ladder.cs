using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public float climbSpeed = 5f;
    public KeyCode climbKey = KeyCode.W;
    public KeyCode descendKey = KeyCode.S;
    public float soundCooldown = 0.2f;

    private Rigidbody rb;
    private bool isClimbing = false;
    private Collider ladderCollider;
    private float lastSoundTime = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ladder"))
        {
            isClimbing = true;
            ladderCollider = other;
            rb.useGravity = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ladder") && other == ladderCollider)
        {
            isClimbing = false;
            ladderCollider = null;
            rb.useGravity = true;
        }
    }

    private void Update()
    {
        if (isClimbing)
        {
            ClimbLadder();
        }
    }

    private void ClimbLadder()
    {
        float verticalInput = 0f;
        bool shouldPlaySound = false;

        if (Input.GetKey(climbKey))
        {
            verticalInput = 1f;
            shouldPlaySound = true;
        }
        else if (Input.GetKey(descendKey))
        {
            verticalInput = -1f;
            shouldPlaySound = true;
        }

        if (shouldPlaySound && Time.time - lastSoundTime >= soundCooldown)
        {
            SoundManager.PlaySound(SoundType.LADDER, 1f);
            lastSoundTime = Time.time; 
        }

        Vector3 climbVelocity = new Vector3(0, verticalInput * climbSpeed, 0);
        rb.velocity = climbVelocity;

        if (verticalInput == 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }
}
