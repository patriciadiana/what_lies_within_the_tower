using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    FOOTSTEPS,
    LADDER,
    OPENDOOR,
    CLOSEDOOR,
    LOCKEDDOOR,
    PICKUPKEY,
    PICKUPPOTION,
    PICKUPNOTE,
    GRABPIECE,
    SNAPPIECE,
    ROTATEPADLOCK,
    UNLOCKPADLOCK,
    FOOTSTEPS2D,
    ATTACK2D,
    SLIMEMOVE2D,
    TAKEDAMAGE2D,
    PICKUP2D,
    NOTEAPPEAR2D,
    SLIMEDEATH2D,
    PLAYERDEATH2D
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance;

    [Header("---------- Audio Source ----------")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip[] soundList;
    [SerializeField] private AudioClip[] musicList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public static void PlaySound(SoundType sound, float volume = 1f)
    {
        if (Instance.soundList.Length > (int)sound)
        {
            Instance.sfxSource.PlayOneShot(Instance.soundList[(int)sound], volume);
        }
        else
        {
            Debug.LogWarning("Sound clip not found for: " + sound);
        }
    }

    public static void PlayMusic(int musicIndex, float volume = 1)
    {
        if (Instance.musicList.Length > musicIndex)
        {
            Instance.musicSource.clip = Instance.musicList[musicIndex];
            Instance.musicSource.volume = volume;
            Instance.musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music clip not found for index: " + musicIndex);
        }
    }
}