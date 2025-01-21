using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesHit : MonoBehaviour
{
    public int hitPoints = 10;
   
    private SpriteRenderer spriteRenderer;
    public Sprite brokenSprite;
    public Sprite moreBrokenSprite;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Hit()
    {
        hitPoints = hitPoints - 1;

        if (hitPoints == 5)
        {
            spriteRenderer.sprite = brokenSprite;
        }

        if (hitPoints <= 0)
        {
            spriteRenderer.sprite = moreBrokenSprite;
        }
    }
}
