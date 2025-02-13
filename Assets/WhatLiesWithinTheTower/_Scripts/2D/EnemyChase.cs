using System.Collections;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float distanceOffset;

    private float distance;
    private Animator animator;
    private Vector2 lastPosition;
    private bool isPlayerDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;

        Health.OnPlayerDeath += OnPlayerDeath;
    }

    void OnDestroy()
    {
        Health.OnPlayerDeath -= OnPlayerDeath;
    }

    void Update()
    {
        if (isPlayerDead)
        {
            MoveAwayFromPlayer();
            return;
        }

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

    private void OnPlayerDeath()
    {
        isPlayerDead = true;
        Debug.Log("Player is dead womp womp");
    }

    private void MoveAwayFromPlayer()
    {
        Vector2 direction = transform.position - player.transform.position;
        direction.Normalize();

        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + direction, speed * Time.deltaTime);

        Vector2 currentPosition = transform.position;
        float velocity = ((currentPosition - lastPosition) / Time.deltaTime).magnitude;
        animator.SetFloat("slimeVelocity", velocity);

        lastPosition = currentPosition;
    }
}