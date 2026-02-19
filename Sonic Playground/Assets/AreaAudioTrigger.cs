using UnityEngine;
using System.Collections.Generic; // 用于多标签列表

public class StoneImpactAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clipDryImpact;
    [SerializeField] private AudioClip clipEchoImpact;
    //tags
    public string echoTag = "StoneFloor"; 
    public string dryTag = "Floor"; 


    void Start()
    {
        audioSource.loop = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. 检查被撞到的物体标签
        string hitTag = collision.gameObject.tag; // 注意：这里检查的是被撞物（地面）的 Tag

        AudioClip clipToPlay = null;

        if (hitTag == echoTag)
        {
            clipToPlay = clipEchoImpact; // 撞到 StoneFloor，播放 Echo
        }
        else if (hitTag == dryTag)
        {
            clipToPlay = clipDryImpact; // 撞到 Floor，播放 Dry
        }

        // 2. 播放单次音效
        // 使用 PlayOneShot 确保不会中断其他声音，且音效从石头的位置发出。
        if (clipToPlay != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }
}