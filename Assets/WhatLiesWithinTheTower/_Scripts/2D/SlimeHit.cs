using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeHit : MonoBehaviour
{
    public int hitPoints = 3;
    private Animator animator;
    private Rigidbody2D rb;
    public float knockbackForce;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Hit(Vector2 attackDirection)
    {
        hitPoints--;

        animator.SetTrigger("Damage");

        rb.AddForce(attackDirection * knockbackForce, ForceMode2D.Impulse);

        if (hitPoints <= 0)
        {
            animator.SetTrigger("Death");
            rb.velocity = Vector2.zero;
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}