using Oculus.Interaction;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class onTriggerRecording : MonoBehaviour
{
    [Header("Meta Hand-Use Input")]
    public PokeInteractable pokeObject;
    private bool isPoked;


    [Header("Recording Settings")]
    public int recordLength = 7;
    public int sampleRate = 44100;

    [Header("Audio Components")]
    //public UnityAudioFreqEnhancer audioEnhancer;
    public AudioEnhencer audioEnhancer;
    

    private AudioSource audioSource;
    public AudioSource EffectSource;
    private AudioClip recording;

    private bool isRecording = false;
    private bool useLocked = false;  // 防止重复触发

    public AudioClip recording_on;
    public AudioClip recording_off;

    // 你的测试路径，Start中使用
    private string applicationPath;
    private string outputPath;
    private string inputPath;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        /// For Android
        string inputPath = Application.persistentDataPath + "/Recordings/RecordingA.wav";
        string outputPath = Application.persistentDataPath + "/Recordings/Enhanced/enhanced.wav";

        /// For PC
        //inputPath = Application.dataPath + "/Audio_Source/exhibition1_sounds/my_voice.wav";
        //outputPath = Application.dataPath + "/Audio_Source/exhibition1_sounds/Enhanced/enhanced.wav";
    }



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (pokeObject == null)
            Debug.LogError("GRAB DEBUG: HandGrabUseInteractor 未赋值！");

        //StartCoroutine(RecordAndProcess());// onStart test
    }



    void Update()
    {
        if (pokeObject == null)
            return;
        isPoked = pokeObject.State == InteractableState.Select;
        // check if button pushed:record for 7 seconds
        if (!useLocked  && isPoked)
        {
            useLocked = true;
            StartCoroutine(RecordAndProcess());

        }
        if (!isPoked && useLocked)
        {
            useLocked = false;
        }

    }

    private IEnumerator RecordAndProcess()
    {
        isRecording = true;

        // --------------------
        //   开始录音
        // --------------------
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("GRAB DEBUG:  没有检测到麦克风！");
            yield break;
        }

        string device = Microphone.devices[0];
        Debug.Log($"GRAB DEBUG: Recording with: {device}");

        if (recording_on)
            EffectSource.PlayOneShot(recording_on);

        recording = Microphone.Start(device, false, recordLength, sampleRate);
        yield return new WaitForSeconds(recordLength);

        if (Microphone.IsRecording(device))
            Microphone.End(device);

        if (recording_off)
            EffectSource.PlayOneShot(recording_off);

        Debug.Log("GRAB DEBUG: Recording finished.");

        // --------------------
        //   保存原声
        // --------------------
        string rawPath = SaveWav(recording);
        Debug.Log("GRAB DEBUG: Saved raw audio: " + rawPath);

        // --------------------
        //   增强音频
        // --------------------
        string enhancedPath = Application.persistentDataPath + "/Recordings/Enhanced/enhanced.wav";
        Directory.CreateDirectory(Path.GetDirectoryName(enhancedPath));

        if (audioEnhancer != null)
        {
            audioEnhancer.ProcessAndAnalyze(rawPath, enhancedPath);
            Debug.Log("GRAB DEBUG: Enhanced audio saved: " + enhancedPath);
        }
        else
        {
            Debug.LogWarning("GRAB DEBUG: audioEnhancer unbind!");
        }

        // --------------------
        //   播放增强音频
        // --------------------
        yield return StartCoroutine(LoadAndPlayAudio(enhancedPath));

        isRecording = false;
    }

    private IEnumerator LoadAndPlayAudio(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("GRAB DEBUG: Enhanced audio file does not exist: " + path);
            yield break;
        }

        var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(
            "file://" + path, AudioType.WAV);

        yield return www.SendWebRequest();

        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError("GRAB DEBUG: Failed to load enhanced audio: " + www.error);
            yield break;
        }

        AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = 0.1f;
        audioSource.Play();

        Debug.Log("GRAB DEBUG: Playing enhanced audio.");
    }

    private string SaveWav(AudioClip clip)
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

            fileStream.Seek(0, SeekOrigin.Begin);
            int hz = clip.frequency;
            int channels = clip.channels;
            Debug.Log("Frequency of the clip: " + hz);

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
            fileStream.Write(BitConverter.GetBytes(samples.Length * 2), 0, 4);
        }

        return path;
    }
}