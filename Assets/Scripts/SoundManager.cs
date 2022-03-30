using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] sounds;

    private AudioSource audio;
    private AudioClip tileBreak;
    private AudioClip tileTap;
    private AudioClip tileBigBreak;

    private List<string> audioQueue = new List<string>(); // contains sound names

    void Start()
    {
        audio = GetComponent<AudioSource>();
        tileBreak    = sounds[0];
        tileTap      = sounds[1];
        tileBigBreak = sounds[2];
    }

    public void addToQueue(string clipName)
    {
        audioQueue.Add(clipName);
    }

    public void playQueue()
    {
        foreach (string clip in audioQueue)
        {
            switch(clip)
            {
                case "tile_break":
                    triggerAudio(tileBreak, true);
                    break;
                case "tile_tap":
                    triggerAudio(tileTap, true);
                    break;
                case "tile_big_break":
                    triggerAudio(tileBigBreak, true);
                    break;

            }
        }
        audioQueue.Clear();
    }

    public void playSound(string clip)
    {
        switch (clip)
        {
            case "tile_break":
                triggerAudio(tileBreak, true);
                break;
            case "tile_tap":
                triggerAudio(tileTap, true);
                break;
            case "tile_big_break":
                triggerAudio(tileBigBreak, true);
                break;
        }
    }
    public void triggerAudio(AudioClip clip, float minDelay, float maxDelay, bool randomPitch)
    {
        float delay = Random.Range(minDelay, maxDelay);
        StartCoroutine(playSound(clip, delay, randomPitch));
    }
    public void triggerAudio(AudioClip clip, bool randomPitch)
    {
        StartCoroutine(playSound(clip, 0f, randomPitch));
    }
    public IEnumerator playSound(AudioClip clip, float delay, bool randomPitch)
    {
        yield return new WaitForSeconds(delay);

        if(randomPitch)
        {
            float pitch = Random.Range(0.75f, 1.1f);
            audio.pitch = pitch;
        } else
            audio.pitch = 1f;

        audio.PlayOneShot(clip);
    }
}
