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

    private Animator animator;
    public LayerMask attackLayerMask;

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

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1f, attackLayerMask);
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
                SlimeHit obstacle = hit.collider.GetComponent<SlimeHit>();
                if (obstacle != null)
                {
                    obstacle.Hit();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        Vector2 delta = inputMovement * velocity * Time.deltaTime;
        Vector2 newPosition = characterBody.position + delta;
        characterBody.MovePosition(newPosition);
    }
}
