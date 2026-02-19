using UnityEngine;

public class AreaAudioSwitcher : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clipOutside;
    [SerializeField] private AudioClip clipInside;      

    private int insideAreaCount = 0;  // 记录是否在洞内

    void Start()
    {
        audioSource.clip = clipOutside;
        audioSource.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CaveArea"))
        {
            insideAreaCount++;
            UpdateAudio();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CaveArea"))
        {
            insideAreaCount--;
            UpdateAudio();
        }
    }

    void UpdateAudio()
    {
        if (insideAreaCount > 0)
        {
            audioSource.clip = clipInside;
        }
        else
        {
            audioSource.clip = clipOutside;
        }

        audioSource.Play();
    }
}
