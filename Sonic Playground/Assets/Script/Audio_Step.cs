using UnityEngine;


public class Audio_Step : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip audio_step;
    private Vector3 lastPosition;
    public float moveThreshold = 0.05f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Set audio clip loop for stepping sound
        if (audio_step != null)
        {
            audioSource.clip = audio_step;
            audioSource.loop = true;
        }
        //save last position
        lastPosition = transform.position;


    }

    void Update()
    {
        // Calculate movement speed
        Vector3 delta = transform.position - lastPosition;
        float speed = delta.magnitude / Time.deltaTime;

        // Play or stop audio based on movement speed

        //you can choose different threshold to play different sound
        if (speed >= moveThreshold)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        //save last position
        lastPosition = transform.position;
    }
}
