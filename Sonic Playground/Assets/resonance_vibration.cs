using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ResonanceSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject frequencyAmplifier;        // Amplifier Object
    public GameObject Animation;                 // Animator Object
    public GameObject wineGlass;                 // Resonance Object
    public Amptifier_properties amplifier;       // parameter class
    public AudioSource audioSource;
    public AudioClip resonanceSound;
    public AudioClip breakSound;
    public PokeInteractable pokeObject;
    public Grabbable grabbable;

    private Animator animator;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private bool isPlayed = false;
    private bool isBreak = false;
    private bool isReset = false;
    private bool isPoked = false;
    private bool useLocked = false;
    private bool isGrab = false;
    private bool isResonating;
    private bool isFrequency;

    [Header("WineGlass Properties")]
    public float glassNaturalFrequency = 440;      // Resonance Frequency of the object
    private float vibrationDistance = 0.2f;            // Distance to make object resonance and donot break
    public float vibrationAmplitude = 4;            // Amplitude to make object resonance and donot break

    [Header("Amplifier Properties")]
    private float ampFrequency;
    private float ampAplitude;


    [Header("Resonance Properties")]             // As resonance fork?      *** to do ***
    public float resonanceFrequency = 0;           // resonance frequeny
    public float resonanceAmplitude = 0;           // resonance amplitue

    [Header("Resonance Settings")]
    public float frequencyTolerance = 3f;        // Frequency error tolerance (for human vocie)
    public float shakeAmount = 0.002f;            // Vibration magnitude

    [Header("Break Conditions")]
    public float maxSafeAmplitude = 5f;          // maxmum amplitude before breaking
    private float dangerousDistance = 0.078f;       // maxmum distance before breaking

    void Start()
    {
        animator = Animation.GetComponent<Animator>();
        originalPos = wineGlass.transform.position;
        originalRot = wineGlass.transform.rotation;

        ampFrequency = amplifier.frequency;
        ampAplitude = amplifier.amplitude;
        isPlayed = false;
        isBreak = false;
        isReset = false;
        isFrequency = false;
        grabbable.WhenPointerEventRaised += GrabEventsAudio;
    }

    void Update()
    {
        ampFrequency = amplifier.frequency;
        ampAplitude = amplifier.amplitude;
        vibrationDistance = Mathf.Lerp(0.20f, 0.80f, Mathf.Clamp01(ampAplitude / 6f));
        dangerousDistance = Mathf.Lerp(0.078f, 0.20f, Mathf.Clamp01(ampAplitude / 6f));
        Debug.Log("ResonanceSystem: Frequency: "+ ampFrequency);
        Debug.Log("ResonanceSystem: ampAplitude: " + ampAplitude);
        // distance between objects
        float distance = Vector3.Distance(
            frequencyAmplifier.transform.position,
            wineGlass.transform.position
        );
        //Debug.Log("distance:" + distance);

        // Check resonance condition by similar frequency and close enough distance or medium amplitude
        isFrequency = Mathf.Abs(ampFrequency - glassNaturalFrequency) <= frequencyTolerance;
        isResonating = isFrequency && ((distance <= vibrationDistance && distance > dangerousDistance) || ampAplitude >= vibrationAmplitude);

        
        // Check break condition by amplitude and distance
        if (isFrequency && ampAplitude != 0 &&((ampAplitude > maxSafeAmplitude || distance <= dangerousDistance)))
        {
            DoResonanceShake();
            PlayBreakAnimation();
            return;
        }
        else
        {
            if (isResonating)
                DoResonanceShake();
            else
                Idel();
        }




        //Poke Buttion Reset
        isPoked = pokeObject.State == InteractableState.Select;
        if (!useLocked && isPoked)
        {
            useLocked = true;
            DoReset();

        }
        if (!isPoked && useLocked)
        {
            useLocked = false;
        }




    }

    private void GrabEventsAudio(PointerEvent evt)
    {

        switch (evt.Type)
        {
            case PointerEventType.Select:
                // Stop sound when grabbed

                Idel();
                isGrab = true;
                Debug.Log("On Grab Stop");


                break;

            case PointerEventType.Unselect:
                // Play sound when released

                isGrab = false;
                Debug.Log("On Grab Play");

                break;
        }
    }


    void DoResonanceShake()
    {
        /*
        Vector2 shake2D = Random.insideUnitCircle * shakeAmount;

        Vector3 pos = new Vector3(
            originalPos.x + shake2D.x,originalPos.y,
            originalPos.z + shake2D.y
        );

        wineGlass.transform.localPosition = pos;
        */
        if (!isBreak && !isGrab)
        {
            animator.SetBool("isVibrating", true);
            Debug.Log("Doing resonance");

            if (!isPlayed)
            {
                audioSource.clip = resonanceSound;
                audioSource.loop = true;
                audioSource.volume = 1f;
                audioSource.Play();

                isPlayed = true;
            }
        }

    }

    void Idel()
    {

        animator.SetBool("isVibrating", false);
        audioSource.Stop();
        isPlayed = false;
        Debug.Log("Doing idel");
    }

    void PlayBreakAnimation()
    {

        if (!isBreak && !isGrab)
        {
            //animator.ResetTrigger("isBreaking");
            animator.SetTrigger("isBreaking");



            audioSource.Stop();
            audioSource.clip = null;
            audioSource.PlayOneShot(breakSound);
            Debug.Log("Wine Glass BROKE!");

            animator.SetBool("isBreak", true);
            isBreak = true;

        }




    }

    private void DoReset()
    {
        animator.SetTrigger("isResetted");
        animator.SetBool("isVibrating", false);
        animator.SetBool("isBreak", false);
        wineGlass.transform.position = originalPos; //reset position
        wineGlass.transform.rotation = originalRot;


        isPlayed = false;
        isBreak = false;
        isReset = false;
        Debug.Log("Doing reset");
        Idel();
        animator.SetBool("isReset", false);
    }
}
