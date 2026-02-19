using UnityEngine;
using UnityEngine.Android;

public class AudioScript01 : MonoBehaviour
{
    public AudioSource hardSurfaceAudio;
    public AudioSource WoodenFloorAudio;

    private void OnCollisionEnter(Collision collision)
    {
        // Check the tag of the object we collided with
        if (collision.gameObject.CompareTag("HS"))
        {
            if (hardSurfaceAudio != null)
                hardSurfaceAudio.Play();
        }
        else if (collision.gameObject.CompareTag("WoodenFloor"))
        {
            if (WoodenFloorAudio != null)
                WoodenFloorAudio.Play();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
