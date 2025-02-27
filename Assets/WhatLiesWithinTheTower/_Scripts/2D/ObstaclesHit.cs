using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesHit : MonoBehaviour
{
    public int hitPoints = 10;
    private SpriteRenderer spriteRenderer;

    public Sprite brokenSprite;
    public Sprite moreBrokenSprite;
    public GameObject note;

    private bool noteSpawned = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (note != null)
        {
            note.SetActive(false);
        }
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
        if (note != null)
        {
            note.SetActive(true);
            note.transform.position = transform.position + new Vector3(0, 1, 0);
        }
        else
        {
            Debug.LogWarning("Note object is not assigned!");
        }
    }
}
