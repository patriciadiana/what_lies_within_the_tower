using SunTemple;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public Animator doorAnimator;
    private void OnTriggerEnter(Collider other)
    {
        SoundManager.PlaySound(SoundType.CLOSEDOOR, 1f);
        doorAnimator.Play("TowerDoorClose");
    }
}
