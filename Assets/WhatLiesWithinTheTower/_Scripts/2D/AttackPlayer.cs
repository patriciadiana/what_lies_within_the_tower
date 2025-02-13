using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    private float attackRange = 0.3f;
    public float attackDamage = 1f;
    public float attackCooldown = 1.5f;

    private Health playerHealth;
    private bool isPlayerInRange = false;
    private float lastAttackTime;

    void Start()
    {
        CircleCollider2D attackCollider = GetComponent<CircleCollider2D>();
        if (attackCollider != null)
        {
            attackCollider.radius = attackRange;
        }
    }

    void Update()
    {
        if (isPlayerInRange && Time.time > lastAttackTime + attackCooldown)
        {
            AttackPlayerFunction();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player2D"))
        {
            playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                isPlayerInRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player2D"))
        {
            isPlayerInRange = false;
            playerHealth = null;
        }
    }

    private void AttackPlayerFunction()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamange(attackDamage);
            lastAttackTime = Time.time;
        }
    }
}