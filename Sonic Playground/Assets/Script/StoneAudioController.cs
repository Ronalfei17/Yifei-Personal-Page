using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(AudioSource))]
public class StoneAudioController : MonoBehaviour
{
    [Header("Impact Audio")]
    [SerializeField] private AudioClip impactSound;

    [Header("Grab / Release Audio")]
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip releaseSound;

    private AudioSource audioSource;
    private Grabbable grabbable;

    private bool hasPlayedImpact = false; // Avoid repeating

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;           
        audioSource.playOnAwake = false;

        // Bind grab events
        grabbable = GetComponent<Grabbable>();
        if (grabbable != null)
        {
            grabbable.WhenPointerEventRaised += OnGrabEvent;
        }
    }

    private void OnGrabEvent(PointerEvent evt)
    {
        switch (evt.Type)
        {
            case PointerEventType.Select:        
                if (grabSound != null)
                    audioSource.PlayOneShot(grabSound);
                break;

            case PointerEventType.Unselect:      
                if (releaseSound != null)
                    audioSource.PlayOneShot(releaseSound);
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasPlayedImpact) return;  // Only play the 1st time
        hasPlayedImpact = true;

        if (impactSound != null)
            audioSource.PlayOneShot(impactSound);

        Destroy(gameObject, 3f); // Clean the stone after 3s
    }
}