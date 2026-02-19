using Oculus.Interaction;
using UnityEngine;

public class Events_Grab : MonoBehaviour
{
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip releaseSound;

    private AudioSource audioSource;
    private Grabbable grabbable;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        grabbable = GetComponent<Grabbable>();

        
        if (grabbable != null)
        {
            // Subscribe to grab and release events, you can add more events as needed
            grabbable.WhenPointerEventRaised += GrabEventsAudio;
        }
    }

    private void GrabEventsAudio(PointerEvent evt)
    {
        if (audioSource == null) return;

        switch (evt.Type)
        {
            case PointerEventType.Select:
                // Play sound when grabbed
                if (grabSound != null)
                {
                    audioSource.PlayOneShot(grabSound);
                }
                else if (audioSource.clip != null)
                {
                    audioSource.Play();
                }
                break;

            case PointerEventType.Unselect:
                // Play sound when released
                if (releaseSound != null)
                {
                    audioSource.PlayOneShot(releaseSound);
                }
                break;
        }
    }
}
