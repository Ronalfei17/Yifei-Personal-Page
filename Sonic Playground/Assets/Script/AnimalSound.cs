using System.Collections.Generic;
using SteamAudio;
using UnityEngine;

public class AnimalImpactAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private SteamAudioSource steamAudioSource;
    public AudioClip impactSound;

    public List<string> impactTags = new List<string> { "StoneBlock", "WoodBlock" };

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        steamAudioSource = GetComponent<SteamAudioSource>();

        audioSource.loop = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (impactTags.Contains(collision.gameObject.tag))
        {
            if (impactSound != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(impactSound);
            }
        }
    }
}