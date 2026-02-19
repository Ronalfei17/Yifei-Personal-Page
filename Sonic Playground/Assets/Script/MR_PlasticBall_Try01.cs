using UnityEngine;

public class MR_PlasticBall_Try01 : MonoBehaviour
{
    /*
<<<<<<< Updated upstream
    // Initializing the audio sources in the assets folder to use them in the object's components
    public AudioSource rollingSound;
    public AudioSource collisionSound;

    //Initializing the rigidbody
    Rigidbody rb;

    //Initializing values tfor speed and collision intensity to check if the ball reaches the level, trying to make the ball react accordingly
    float speedThreshold = 0.2f;
    float collisionthreshold = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb= GetComponent<Rigidbody>();
=======
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< Updated upstream
        float ballSpeed = rb.linearVelocity.magnitude;

        void onCollisionEnter (Collision collision)
        {
            float intensity= collision.relativeVelocity.magnitude;

            if (intensity> collisionthreshold)
            {
                collisionSound.pitch = Random.Range(0.9f, 1.1f);
                collisionSound.PlayOneShot(collisionSound.clip); //Using PlayOneShot function to play the collision audio when the ball hits any surface
            }
        }
=======
        
>>>>>>> Stashed changes
    }
    */

    public AudioSource rollingAudio;
    public AudioSource bounceAudio;

    Rigidbody rb;
    float rollThreshold = 0.2f;
    float bounceThreshold = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed > rollThreshold)
        {
            if (!rollingAudio.isPlaying)
                rollingAudio.Play();

            rollingAudio.volume = Mathf.Lerp(rollingAudio.volume, 1f, Time.deltaTime * 3f);
            rollingAudio.pitch = Mathf.Clamp(speed * 0.4f, 0.8f, 2f);
        }
        else
        {
            rollingAudio.volume = Mathf.Lerp(rollingAudio.volume, 0f, Time.deltaTime * 3f);
            if (rollingAudio.volume <= 0.01f)
                rollingAudio.Stop();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        float impact = collision.relativeVelocity.magnitude;

        if (impact > bounceThreshold)
        {
            bounceAudio.pitch = Random.Range(0.9f, 1.1f);
            bounceAudio.PlayOneShot(bounceAudio.clip);
        }
    }
}
