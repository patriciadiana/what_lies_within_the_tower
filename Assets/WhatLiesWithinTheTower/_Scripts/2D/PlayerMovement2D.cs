using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public int speed = 10;
    private Rigidbody2D characterBody;
    private Vector2 velocity;
    private Vector2 inputMovement;

    public float sprintMultiplier = 1.5f;
    private bool isSprinting = false;

    private Animator animator;
    public LayerMask attackLayerMask;

    private bool isMoving;
    public float footstepsDelay = 0.5f;

    void Start()
    {
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        inputMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        animator.SetFloat("xVelocity", inputMovement.magnitude);

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (inputMovement.magnitude > 0 && !isMoving)
        {
            StartCoroutine(PlayFootsteps());
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
            PerformAttack();
        }
    }


    private IEnumerator PlayFootsteps()
    {
        isMoving = true;
        while (inputMovement.magnitude > 0) 
        {
            SoundManager.PlaySound(SoundType.FOOTSTEPS2D, 0.2f);
            yield return new WaitForSeconds(footstepsDelay);
        }
        isMoving = false;
    }

    private void PerformAttack()
    {
        SoundManager.PlaySound(SoundType.ATTACK2D, 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.5f, attackLayerMask);
        if (hit.collider != null)
        {
            int hitLayer = hit.collider.gameObject.layer;

            if (hitLayer == LayerMask.NameToLayer("coffin"))
            {
                ObstaclesHit obstacle = hit.collider.GetComponent<ObstaclesHit>();
                if (obstacle != null)
                {
                    obstacle.Hit();
                }
            }
            else if (hitLayer == LayerMask.NameToLayer("slime"))
            {
                SlimeHit slime = hit.collider.GetComponent<SlimeHit>();
                if (slime != null)
                {
                    Vector2 knockbackDirection = (hit.collider.transform.position - transform.position).normalized;

                    slime.Hit(knockbackDirection);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;
        Vector2 delta = inputMovement * currentSpeed * Time.deltaTime;
        Vector2 newPosition = characterBody.position + delta;
        characterBody.MovePosition(newPosition);
    }

}
