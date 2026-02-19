using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class StoneImpactEcho : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip drySound;
    public AudioClip caveEchoSound;

    public CaveReverbSwitcher reverbInfo;

    void Awake()
    {
        // ×Ô¶¯²¹Æë audioSource
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (reverbInfo == null)
            reverbInfo = Object.FindFirstObjectByType<CaveReverbSwitcher>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (reverbInfo != null && reverbInfo.playerIsInsideCave)
            PlayEcho();
        else
            PlayDry();

        Destroy(gameObject, 2f);
    }

    void PlayEcho()
    {
        audioSource.clip = caveEchoSound;
        audioSource.Play();
    }

    void PlayDry()
    {
        audioSource.clip = drySound;
        audioSource.Play();
    }
}

