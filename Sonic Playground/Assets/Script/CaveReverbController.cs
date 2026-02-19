using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioReverbZone))]
public class CaveReverbSwitcher : MonoBehaviour
{
    private AudioReverbZone reverbZone;
    private int playerInsideCount = 0;   // Solving the problem of multiple trigger bodies
    public bool playerIsInsideCave = false;

    void Awake()
    {
        reverbZone = GetComponent<AudioReverbZone>();
        reverbZone.enabled = false;  // Initially, the reverbZone is turned off.
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInsideCount++;
        if (playerInsideCount == 1)
        {
            reverbZone.enabled = true;  // ReverbZone only opens upon first entry
            playerIsInsideCave = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInsideCount--;
        if (playerInsideCount <= 0)
        {
            playerInsideCount = 0;
            reverbZone.enabled = false;
            playerIsInsideCave = false;
        }
    }
}

