using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float startingHealth;
    private Animator animator;
    private Rigidbody2D rb;
    public float currentHealth { get; private set; }

    public static event Action OnPlayerDeath;

    private PlayerMovement2D playerMovement;
    public GameOverScreen gameOverScreen;

    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement2D>();
    }
    private void Start()
    {
        ResetHealth();
    }

    public void TakeDamange(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
           
        }
        else
        {
            animator.SetTrigger("Death");
            rb.velocity = Vector2.zero;
            SoundManager.PlaySound(SoundType.PLAYERDEATH2D, 0.8f);
            playerMovement.enabled = false;

            OnPlayerDeath?.Invoke();
            gameOverScreen.Setup();
        }
    }

    public void ResetHealth()
    {
        currentHealth = startingHealth;
        playerMovement.enabled = true;
        animator.ResetTrigger("Death");
    }
}