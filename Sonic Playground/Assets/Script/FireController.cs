using UnityEngine;

public class FireController : MonoBehaviour
{
    [SerializeField] private AudioSource fireAudioSource;

    [SerializeField] private AudioClip lowFireClip;
    [SerializeField] private AudioClip highFireClip;

    public string woodBlockTag = "WoodBlock";

    public float highFireDuration = 10.0f;

    private float timer;
    private bool isHighFire = false;
    void Start()
    {
        if (fireAudioSource != null && lowFireClip != null)
        {
            fireAudioSource.clip = lowFireClip;
            fireAudioSource.loop = true; 
            fireAudioSource.Play();
        }
    }

    void Update()
    {
        // If the current state is high fire, start the timer.
        if (isHighFire)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                // End, switch to low fire
                SwitchToLowFire();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is a block of wood
        if (other.CompareTag(woodBlockTag))
        {
            SwitchToHighFire();
        }
    }

    private void SwitchToHighFire()
    {
        if (fireAudioSource.clip != highFireClip)
        {
            fireAudioSource.clip = highFireClip;
            fireAudioSource.Play(); 
        }

        // Reset the timer and enter the high fire state.
        isHighFire = true;
        timer = highFireDuration;
    }

    private void SwitchToLowFire()
    {
        if (fireAudioSource.clip != lowFireClip)
        {
            fireAudioSource.clip = lowFireClip;
            fireAudioSource.Play();
        }
        isHighFire = false;
    }
}
