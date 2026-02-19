using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Amptifier_properties : MonoBehaviour
{
    [Tooltip("GameObjects")]
    public AudioEnhencer detector;
    public Slider volumeSlider;
    public AudioSource audioSource;
    public TextMeshProUGUI valueText;

    public float frequency;
    public float amplitude;


    void Start()
    {
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        amplitude = value;
        Debug.Log("Amptifier_properties: Amplitude update: " + amplitude);
        if (audioSource.clip != null)
        {
            audioSource.volume = value;
            valueText.text = "Volume level: "+ value.ToString("F2");
        }
    }

    void Update()
    {
        if (detector == null)
        {
            frequency = 0;
            return;
        }
        //amplitude = volumeSlider.value;

        if (detector.lastDetectionHas440)
        {
            frequency = 440f;
        }
        else
        {
            frequency = 0;
        }
    }

}

