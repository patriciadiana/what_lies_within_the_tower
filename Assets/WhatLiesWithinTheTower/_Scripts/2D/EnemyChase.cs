using System.Collections;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float distanceOffset;
    public float stepSoundInterval = 1.0f;

    private float distance;
    private Animator animator;
    private Vector2 lastPosition;
    private bool isPlayerDead = false;
    private bool isMoving = false;
    private Coroutine footstepCoroutine;

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
            MoveTowardsPlayer();
        }
        else
        {
            StopMovement();
        }
    }

    private void MoveTowardsPlayer()
    {
        transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
        HandleMovement();
    }

    private void MoveAwayFromPlayer()
    {
        Vector2 direction = transform.position - player.transform.position;
        direction.Normalize();
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + direction, speed * Time.deltaTime);
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 currentPosition = transform.position;
        float velocity = ((currentPosition - lastPosition) / Time.deltaTime).magnitude;

        animator.SetFloat("slimeVelocity", velocity);

        if (velocity > 0.1f && !isMoving)
        {
            isMoving = true;
            footstepCoroutine = StartCoroutine(PlayFootsteps());
        }
        else if (velocity <= 0.1f)
        {
            StopMovement();
        }

        lastPosition = currentPosition;
    }

    private void StopMovement()
    {
        animator.SetFloat("slimeVelocity", 0f);
        if (isMoving)
        {
            isMoving = false;
            if (footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
        }
    }

    private IEnumerator PlayFootsteps()
    {
        while (isMoving)
        {
            SoundManager.PlaySound(SoundType.SLIMEMOVE2D, 0.5f);
            yield return new WaitForSeconds(stepSoundInterval);
        }
    }

    private void OnPlayerDeath()
    {
        isPlayerDead = true;
        StopMovement();
        Debug.Log("Player is dead womp womp");
    }
}
