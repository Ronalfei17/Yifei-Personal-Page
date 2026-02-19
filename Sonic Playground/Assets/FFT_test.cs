using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class FFT_test : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip recording;
    private bool isRecording = false;
    public AudioClip recording_on;
    public AudioClip recording_off;
    /// For Android
    //string inputPath = Application.persistentDataPath + "/Recordings/RecordingA.wav";
    //string outputPath = Application.persistentDataPath + "/Recordings/Enhanced/enhanced.wav";

    /// For PC
    private string inputPath = Application.dataPath + "/Audio_Source/exhibition1_sounds/my_voice.wav";
    private string outputPath = Application.dataPath + "/Audio_Source/exhibition1_sounds/Enhanced/enhanced.wav";
    //public UnityAudioFreqEnhancer audioEnhancer;
    public AudioEnhencer audioEnhencer;

    [Header("Recording Settings")]
    public int recordLength = 7;
    public int sampleRate = 44100;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //read audio file from path and process audio, then export enhenced audio.
        // 核心流程：Load WAV → STFT → 增强 → iSTFT → 播放 + 保存
        // audio Sample Rate : 44100, Channels : 2, Precision : 16-bit,
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(RecordAndProcess());

    }

    // Update is called once per frame
    void Update()
    {
        //nothing here
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
            audioSource.PlayOneShot(recording_on);

        recording = Microphone.Start(device, false, recordLength, sampleRate);
        yield return new WaitForSeconds(recordLength);

        if (Microphone.IsRecording(device))
            Microphone.End(device);

        if (recording_off)
            audioSource.PlayOneShot(recording_off);

        Debug.Log("GRAB DEBUG: Recording finished.");

        // --------------------
        //   保存原声
        // --------------------
        string rawPath = SaveWav(recording);
        Debug.Log("GRAB DEBUG: Saved raw audio: " + rawPath);

        // --------------------
        //   增强音频
        // --------------------
        string enhancedPath = Application.dataPath + "/Audio_Source/exhibition1_sounds/Enhanced/enhanced.wav";
        Directory.CreateDirectory(Path.GetDirectoryName(enhancedPath));

        if (audioEnhencer != null)
        {
            audioEnhencer.ProcessAndAnalyze(rawPath, enhancedPath);
            Debug.Log("GRAB DEBUG: Enhanced audio saved: " + enhancedPath);
        }
        else
        {
            Debug.LogWarning("GRAB DEBUG: audioEnhancer 未绑定！");
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
        audioSource.Play();

        Debug.Log("GRAB DEBUG: Playing enhanced audio.");
    }

    private string SaveWav(AudioClip clip)
    {
        string folder = Application.dataPath + "/Audio_Source/exhibition1_sounds/";
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
