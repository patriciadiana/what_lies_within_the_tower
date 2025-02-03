using SunTemple;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public Animator doorAnimator;
    private bool hasPlayedAnimation = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayedAnimation) 
        {
            hasPlayedAnimation = true; 
            SoundManager.PlaySound(SoundType.CLOSEDOOR, 1f);
            doorAnimator.Play("TowerDoorClose");
        }
    }
}
