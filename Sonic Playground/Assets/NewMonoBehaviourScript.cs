using UnityEngine;
using System.IO;
using System;

public class SimpleMicRecorder2 : MonoBehaviour
{
    public UnityAudioFreqEnhancer audioEnhancer;

    private bool analyzeAfterPlay = false;

    public int recordLength = 20;   // second
    public int sampleRate = 44100;  // sample
    private AudioClip recording;
    public string device;
    private AudioSource audioSource;




    void Start() /// need a trigger now
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        //detect the device
        //print the device name,  device 0 always the Micphone in headset
        /*for (int i = 0; i < device.Length; i++)
        {
            Debug.Log("Device[" + i + "]" + device[i]);
        }
        */
        if (Microphone.devices.Length <= 0)
        {
            Debug.LogError("No detect device0");
            return;
        }
        // choose device0
        device = Microphone.devices[0]; 
        //Start function to record 
        recording = Microphone.Start(device, false, recordLength, sampleRate);
        Debug.Log("Start Record " + device);
    }

    void Update()
    {
        if (recording != null && !Microphone.IsRecording(device))
        {
            //Use SaveWav function to save audio file
            string path = SaveWav(recording);
            //Set recording to clip
            audioSource.clip = recording;

            //use a bool to print spectrum after the first frame
            analyzeAfterPlay = true;



            //To print the spectrum
            /*
            float[] spectrum = new float[256];
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
            for (int i = 0; i < spectrum.Length; i++)
            {
                Debug.Log("Spectrum Value [ " + i + " ] " + spectrum[i]);
            }
            */
            //audioSource.Play();
            OnRecordingFinished();
            Debug.Log("Play Record" + path);
            recording = null; 
            // No save repeatedly
        }

        //must print spectrum after the first frame
        if (analyzeAfterPlay)
        {
            float[] spectrum = new float[256];
            audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

            for (int i = 0; i < spectrum.Length; i++)
                Debug.Log("Spectrum Value [ " + i + " ] " + spectrum[i]);

            analyzeAfterPlay = false;
        }
    }

    private void OnRecordingFinished()
    {
        if (recording == null)
            return;

        string rawPath = SaveWav(recording);
        string enhancedPath = Application.persistentDataPath + "/Recordings/Enhanced/enhanced.wav";

        // 只执行一次
        audioEnhancer.ProcessAndEnhance(rawPath, enhancedPath);

        Debug.Log("Audio enhanced → " + enhancedPath);

        audioSource.clip = recording;
        analyzeAfterPlay = true;

    }


    //save audio file to wav
    string SaveWav(AudioClip clip)
    {
        string folder = Application.persistentDataPath + "/Recordings/";
        Directory.CreateDirectory(folder);
        string path = folder + "RecordingA.wav";

        using (var fileStream = new FileStream(path, FileMode.Create))
        {
            byte[] header = new byte[44];
            fileStream.Write(header, 0, 44);

            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            short[] intData = new short[samples.Length];
            byte[] bytesData = new byte[samples.Length * 2];
            int factor = 32767;

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * factor);
                BitConverter.GetBytes(intData[i]).CopyTo(bytesData, i * 2);
            }

            fileStream.Write(bytesData, 0, bytesData.Length);

            // WAV information
            fileStream.Seek(0, SeekOrigin.Begin);
            int hz = clip.frequency;
            int channels = clip.channels;
            int samplesCount = clip.samples;

            fileStream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
            fileStream.Write(BitConverter.GetBytes(fileStream.Length - 8), 0, 4);
            fileStream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);
            fileStream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
            fileStream.Write(BitConverter.GetBytes(16), 0, 4);
            fileStream.Write(BitConverter.GetBytes((ushort)1), 0, 2);
            fileStream.Write(BitConverter.GetBytes((ushort)channels), 0, 2);
            fileStream.Write(BitConverter.GetBytes(hz), 0, 4);
            fileStream.Write(BitConverter.GetBytes(hz * channels * 2), 0, 4);
            fileStream.Write(BitConverter.GetBytes((ushort)(channels * 2)), 0, 2);
            fileStream.Write(BitConverter.GetBytes((ushort)16), 0, 2);
            fileStream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
            fileStream.Write(BitConverter.GetBytes(samplesCount * channels * 2), 0, 4);
        }

        //print save
        Debug.Log("Save Record " + path);
        return path;
    }
}
