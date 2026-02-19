// AudioAmplifyAndDetect.cs
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class AudioEnhencer : MonoBehaviour
{

    [Tooltip("增益倍数，比如 2.0 = 放大 2 倍")]
    public float gain = 10f;

    [Tooltip("Goertzel 检测 440Hz 时，平均能量门限 (0~1)")]
    public float goertzelThreshold = 0.0001f;

    [Tooltip("Goertzel 检测的 sample rate (必须与 WAV 一致)")]
    public int detectionSampleRate = 44100;

    public bool lastDetectionHas440 = false;

    public AudioSource audioSource;

    private float[] realTimeBuffer = new float[2048];

    void Update()
    {
        if (audioSource == null || !audioSource.isPlaying)
        {
            lastDetectionHas440 = false;
            return;
        }

        // 从当前播放的 AudioSource 中读取 2048 个采样
        audioSource.GetOutputData(realTimeBuffer, 0);

        // Goertzel 实时检测
        lastDetectionHas440 = DetectGoertzel(realTimeBuffer, detectionSampleRate, 440f, goertzelThreshold);
        
    }

    public void ProcessAndAnalyze(string rawWavPath, string enhancedWavPath)
    {
        float[] samples;
        int sampleRate;
        LoadWav(rawWavPath, out samples, out sampleRate);
        Debug.Log("AudioEnhencer: WAV loaded");

        // 整体增益（注意防止 clipping）
        for (int i = 0; i < samples.Length; i++)
            samples[i] = Mathf.Clamp(samples[i] * gain, -1f, 1f);

        Debug.Log("AudioEnhencer: WAV Enhenced");
        // 保存增强后的 WAV
        SaveWav(enhancedWavPath, samples, sampleRate);
        Debug.Log("AudioEnhencer: WAV Saved");

        // 用 Goertzel 检测 440 Hz
        lastDetectionHas440 = DetectGoertzel(samples, sampleRate, 440.0f, goertzelThreshold);
        Debug.Log("Goertzel 440Hz detection: " + lastDetectionHas440);
    }

    bool DetectGoertzel(float[] samples, int sampleRate, double targetFreq, double threshold)
    {
        int N = samples.Length;
        int k = (int)(0.5 + (N * targetFreq) / sampleRate);
        double omega = 2.0 * Math.PI * k / N;
        double sine = Math.Sin(omega);
        double cosine = Math.Cos(omega);
        double coeff = 2.0 * cosine;

        double q0 = 0, q1 = 0, q2 = 0;
        for (int i = 0; i < N; i++)
        {
            q0 = coeff * q1 - q2 + samples[i];
            q2 = q1;
            q1 = q0;
        }

        double real = q1 - q2 * cosine;
        double imag = q2 * sine;
        double magnitude = Math.Sqrt(real * real + imag * imag);
        double normalized = magnitude / N;
        Debug.Log("AudioEnhencer: Current Detection:" + normalized + " which is "+ (normalized >= threshold));
        return normalized >= threshold;
    }

    void LoadWav(string path, out float[] samples, out int sampleRate)
    {
        byte[] data = File.ReadAllBytes(path);
        sampleRate = BitConverter.ToInt32(data, 24);
        int start = 44;
        int count = (data.Length - start) / 2;
        samples = new float[count];
        for (int i = 0; i < count; i++)
        {
            short v = BitConverter.ToInt16(data, start + i * 2);
            samples[i] = v / 32768f;
        }
    }

    void SaveWav(string path, float[] samples, int sampleRate)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        using (var f = new FileStream(path, FileMode.Create))
        {
            int byteRate = sampleRate * 2;
            f.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, 4);
            f.Write(BitConverter.GetBytes(36 + samples.Length * 2), 0, 4);
            f.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0, 4);
            f.Write(System.Text.Encoding.ASCII.GetBytes("fmt "), 0, 4);
            f.Write(BitConverter.GetBytes(16), 0, 4);
            f.Write(BitConverter.GetBytes((ushort)1), 0, 2);
            f.Write(BitConverter.GetBytes((ushort)1), 0, 2);
            f.Write(BitConverter.GetBytes(sampleRate), 0, 4);
            f.Write(BitConverter.GetBytes(byteRate), 0, 4);
            f.Write(BitConverter.GetBytes((ushort)2), 0, 2);
            f.Write(BitConverter.GetBytes((ushort)16), 0, 2);
            f.Write(System.Text.Encoding.ASCII.GetBytes("data"), 0, 4);
            f.Write(BitConverter.GetBytes(samples.Length * 2), 0, 4);

            for (int i = 0; i < samples.Length; i++)
            {
                short v = (short)(Mathf.Clamp(samples[i], -1f, 1f) * 32767);
                f.Write(BitConverter.GetBytes(v), 0, 2);
            }
        }
    }
}
