using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public GameObject player;
    public float speed;

    private float distance;
    public float distanceOffset;

    private Animator animator;
    private Vector2 lastPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    
    void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();

        if (distance < distanceOffset)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            Vector2 currentPosition = transform.position;
            float velocity = ((currentPosition - lastPosition) / Time.deltaTime).magnitude;

            animator.SetFloat("slimeVelocity", velocity);

            lastPosition = currentPosition;
        }
        else
        {
            animator.SetFloat("slimeVelocity", 0f);
        }
    }
}
