using UnityEngine;

public class ObstaclesHit : MonoBehaviour
{
    public int hitPoints = 10;
    private SpriteRenderer spriteRenderer;

    public Sprite brokenSprite;
    public Sprite moreBrokenSprite;
    public GameObject notePrefab;

    private bool noteSpawned = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Hit()
    {
        hitPoints -= 1;

        if (hitPoints == 5)
        {
            spriteRenderer.sprite = brokenSprite;
        }

        if (hitPoints <= 0 && !noteSpawned)
        {
            spriteRenderer.sprite = moreBrokenSprite;
            ShowNote();
            SoundManager.PlaySound(SoundType.NOTEAPPEAR2D, 1f);
            noteSpawned = true;
        }
    }

    void ShowNote()
    {
        if (notePrefab != null)
        {
            GameObject spawnedNote = Instantiate(notePrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Note prefab is not assigned!");
        }
    }
}
