
using UnityEngine;
using System;
using System.IO;
using Oculus.Interaction.Feedback;

public class UnityAudioFreqEnhancer : MonoBehaviour
{
    public AudioSource audioSource;
    [Header("Frequency Enhance Settings")]
    public float targetFreq = 1000;//中心频率
    public float bandWidthHz = 1000; // 频带宽度
    public double gain = 10;     // 增益倍率
    public int fftSize = 2048;        // FFT 分辨率（越大越清晰）



    void Start()
    {

        /// For Android
        //string applicationPath = Application.persistentDataPath + "/Recordings/RecordingA.wav";
        //string outputPath = Application.persistentDataPath + "/Recordings/Enhanced/enhanced.wav";

        /// For PC
        string inputPath = Application.dataPath + "/Audio_Source/exhibition1_sounds/my_voice.wav";
        string outputPath = Application.dataPath + "/Audio_Source/exhibition1_sounds/Enhanced/enhanced.wav";

        //ProcessAndEnhance(inputPath, outputPath);
    }

    // ------------------------------------------------------------
    // 核心流程：Load WAV → STFT → 增强 → iSTFT → 播放 + 保存
    // ------------------------------------------------------------

    public void ProcessAndEnhance(string wavPath, string savePath)
    {
        float[] samples;
        int sampleRate;

        Debug.Log("keydebug,wavPath" + wavPath);

        LoadWav(wavPath, out samples, out sampleRate);

        float[] enhanced = STFT_Enhance(samples, sampleRate);

        // 播放
        AudioClip clip = AudioClip.Create("Enhanced", enhanced.Length, 1, sampleRate, false);
        clip.SetData(enhanced, 0);
        //audioSource.clip = clip;
        //audioSource.Play();

        //File.Delete(wavPath);  //delete original file

        SaveWav(savePath, enhanced, sampleRate);
        Debug.Log("Enhanced WAV saved at: " + savePath);
    }

    // ------------------------------------------------------------
    // 短时 FFT（STFT）
    // ------------------------------------------------------------
    float[] STFT_Enhance(float[] samples, int sampleRate)
    {
        int hop = fftSize / 2;  // 50% overlap
        int frameCount = (samples.Length - fftSize) / hop;

        float[] window = Hanning(fftSize);
        float[] output = new float[samples.Length + fftSize];

        Complex[] buffer = new Complex[fftSize];

        for (int frame = 0; frame < frameCount; frame++)
        {
            int start = frame * hop;

            // Apply window
            for (int i = 0; i < fftSize; i++)
                buffer[i] = new Complex(samples[start + i] * window[i], 0);

            // FFT
            FFT(buffer);

            // Enhance band
            EnhanceBand(buffer, sampleRate);

            // iFFT
            IFFT(buffer);

            // overlap-add
            for (int i = 0; i < fftSize; i++)
                output[start + i] += (float)buffer[i].Real * window[i];
        }

        return output;
    }

    // ------------------------------------------------------------
    // 增强一个频率带
    // ------------------------------------------------------------
    void EnhanceBand(Complex[] spectrum, int sampleRate)
    {
        int N = spectrum.Length;

        for (int i = 0; i < N / 2; i++)
        {
            float freq = i * sampleRate / (float)N;

            if (Mathf.Abs(freq - targetFreq) < bandWidthHz)
            {
                // 在目标频率范围内，增强
                spectrum[i].Scale(gain);
                spectrum[N - 1 - i].Scale(gain);
                Debug.Log("Gain spectrum[i] :"+ spectrum[i]);
                Debug.Log("Gain spectrum[N - 1 - i] :" + spectrum[N - 1 - i]);

            }
            else
            {
                // 不在范围内，置零
                //spectrum[i] = new Complex(0, 0);
                //spectrum[N - 1 - i] = new Complex(0, 0);
            }
        }
    }


    // ------------------------------------------------------------
    // Hanning 窗
    // ------------------------------------------------------------
    float[] Hanning(int N)
    {
        float[] win = new float[N];
        for (int i = 0; i < N; i++)
            win[i] = 0.5f - 0.5f * Mathf.Cos(2f * Mathf.PI * i / (N - 1));
        return win;
    }

    // ------------------------------------------------------------
    // WAV Loader (Mono, 16-bit PCM)
    // ------------------------------------------------------------
    void LoadWav(string path, out float[] samples, out int sampleRate)
    {
        Debug.Log("keydebug,path" + path);
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

    // ------------------------------------------------------------
    // Save WAV (Mono, 16-bit PCM)
    // ------------------------------------------------------------
    void SaveWav(string path, float[] samples, int sampleRate)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        using (var f = new FileStream(path, FileMode.Create))
        {
            int byteRate = sampleRate * 2;

            // Header
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

            // PCM data
            foreach (float s in samples)
            {
                short v = (short)(Mathf.Clamp(s, -1f, 1f) * 32767);
                f.Write(BitConverter.GetBytes(v), 0, 2);
            }
        }
    }

    // ------------------------------------------------------------
    // FFT & iFFT（与你之前的一样）
    // ------------------------------------------------------------
    void FFT(Complex[] buffer) => FFT_Recursive(buffer);
    void IFFT(Complex[] buffer)
    {
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = buffer[i].Conjugate();

        FFT_Recursive(buffer);

        double N = buffer.Length;
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = buffer[i].Conjugate();
            buffer[i].Real /= N;
            buffer[i].Imag /= N;
        }
    }

    void FFT_Recursive(Complex[] buffer)
    {
        int N = buffer.Length;
        if (N <= 1) return;

        int half = N / 2;
        Complex[] even = new Complex[half];
        Complex[] odd = new Complex[half];

        for (int i = 0; i < half; i++)
        {
            even[i] = buffer[2 * i];
            odd[i] = buffer[2 * i + 1];
        }

        FFT_Recursive(even);
        FFT_Recursive(odd);

        for (int k = 0; k < half; k++)
        {
            double angle = -2.0 * Math.PI * k / N;
            Complex w = new Complex(Math.Cos(angle), Math.Sin(angle));
            buffer[k] = even[k] + w * odd[k];
            buffer[k + half] = even[k] - w * odd[k];
        }
    }
}

// ============================================================
// Complex 结构体
// ============================================================
public struct Complex
{
    public double Real;
    public double Imag;

    public Complex(double r, double i)
    {
        Real = r;
        Imag = i;
    }

    public Complex Conjugate() => new Complex(Real, -Imag);

    public void Scale(double s)
    {
        Real *= s;
        Imag *= s;
        Debug.Log("Gain value Real: " + Real);
        Debug.Log("Gain value Imag: " + Imag);
    }

    public static Complex operator +(Complex a, Complex b)
        => new Complex(a.Real + b.Real, a.Imag + b.Imag);

    public static Complex operator -(Complex a, Complex b)
        => new Complex(a.Real - b.Real, a.Imag - b.Imag);

    public static Complex operator *(Complex a, Complex b)
        => new Complex(
            a.Real * b.Real - a.Imag * b.Imag,
            a.Real * b.Imag + a.Imag * b.Real
        );
}

