using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public float climbSpeed = 5f;
    public KeyCode climbKey = KeyCode.W;
    public KeyCode descendKey = KeyCode.S;
    public float soundCooldown = 0.2f; // Time to wait before playing the sound again

    private Rigidbody rb;
    private bool isClimbing = false;
    private Collider ladderCollider;
    private float lastSoundTime = 0f; // Keeps track of the last time the sound played

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
            rb.useGravity = false; // Disable gravity while climbing
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
            shouldPlaySound = true; // Climbing up
        }
        else if (Input.GetKey(descendKey))
        {
            verticalInput = -1f;
            shouldPlaySound = true; // Climbing down
        }

        // Play sound if the cooldown has passed and there's a valid direction
        if (shouldPlaySound && Time.time - lastSoundTime >= soundCooldown)
        {
            SoundManager.PlaySound(SoundType.LADDER, 1f);
            lastSoundTime = Time.time; // Update the last sound play time
        }

        // Update climb velocity
        Vector3 climbVelocity = new Vector3(0, verticalInput * climbSpeed, 0);
        rb.velocity = climbVelocity;

        if (verticalInput == 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }
}
