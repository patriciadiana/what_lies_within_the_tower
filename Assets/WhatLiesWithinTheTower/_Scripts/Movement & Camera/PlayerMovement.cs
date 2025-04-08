using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float sprintSpeed;

    public float groundDrag;
    public float stairDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    public float playerHeight;
    public LayerMask whatIsGround;

    bool grounded;
    bool onStairs;
    bool isSprinting;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private float footstepCooldown = 0.5f;
    private float footstepTimer;
    private bool isMoving;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.4f, whatIsGround);

        MyInput();
        SpeedControl();

        if (onStairs)
        {
            rb.drag = stairDrag;
        }
        else if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        isSprinting = Input.GetKey(sprintKey) && (verticalInput > 0) && !onStairs;

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        isMoving = (horizontalInput != 0 || verticalInput != 0) && (grounded || onStairs);

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        if (onStairs)
        {
            Vector3 stairVelocity = moveDirection.normalized * currentSpeed * 0.8f;
            rb.velocity = new Vector3(stairVelocity.x, rb.velocity.y, stairVelocity.z);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float maxSpeed = isSprinting ? sprintSpeed : moveSpeed;

        if (onStairs) maxSpeed *= 0.7f;

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void HandleFootsteps()
    {
        footstepTimer -= Time.deltaTime;
        if (isMoving && footstepTimer <= 0f)
        {
            SoundManager.PlaySound(SoundType.FOOTSTEPS, 0.1f);
            footstepTimer = footstepCooldown;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("stairs"))
        {
            float dot = Vector3.Dot(moveDirection.normalized, collision.transform.forward);
            onStairs = Mathf.Abs(dot) > 0.3f && (horizontalInput != 0 || verticalInput != 0);
        }
        else
        {
            onStairs = false;
        }
    }
}