using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeHit : MonoBehaviour
{
    public int hitPoints = 3;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Hit()
    {
        hitPoints = hitPoints - 1;
        animator.SetTrigger("Damage");

        if (hitPoints <= 0)
        {
            animator.SetTrigger("Death");
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
