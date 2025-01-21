using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator door;
    private bool isClosed;

    public enum DoorType
    {
        House,
        Tower
    }

    public DoorType doorType;

    void Start()
    {
        isClosed = true;
    }

    public void Interact()
    {
        if(isClosed) {
            DoorOpens();
        }
        else {
            DoorCloses();
        }
    }

    private void DoorCloses()
    {
        isClosed = true;
        switch (doorType)
        {
            case DoorType.House:
                SoundManager.PlaySound(SoundType.CLOSEDOOR, 1f);
                door.Play("HomeDoorClose");
                break;
            case DoorType.Tower:
                SoundManager.PlaySound(SoundType.CLOSEDOOR, 1f);
                door.Play("TowerDoorClose");
                break;
            default:
                Debug.LogError("Unrecognized door type!");
                break;
        }
    }

    private void DoorOpens()
    {
        isClosed = false;
        switch (doorType)
        {
            case DoorType.House:
                SoundManager.PlaySound(SoundType.OPENDOOR, 1f);
                door.Play("HomeDoorOpen");
                break;
            case DoorType.Tower:
                SoundManager.PlaySound(SoundType.OPENDOOR, 1f);
                door.Play("TowerDoorOpen");
                break;
            default:
                Debug.LogError("Unrecognized door type!");
                break;
        }
    }
}
