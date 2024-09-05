using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundFX
{
    Click,
    Write,
    Flip,
    Match,
    Unmatch,
    Fail,
    Win

}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource music, sound;
    [SerializeField] private AudioClip click, write, flip, match, unmatch, fail, win;
    [SerializeField] private AudioClip bgMusic;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        PlayBGMusic();
    }

    public void PlayBGMusic()
    {
        music.clip = bgMusic;
        music.Play();
    }
    public void PlaySoundFX(SoundFX soundfx)
    {
        AudioClip clip = null;
        switch (soundfx)
        {
            case SoundFX.Click:
                clip = click;
                break;
            case SoundFX.Write:
                clip = write;
                break;
            case SoundFX.Flip:
                clip = flip;
                break;
            case SoundFX.Match:
                clip = match;
                break;
            case SoundFX.Unmatch:
                clip = unmatch;
                break;
            case SoundFX.Fail:
                clip = fail;
                StartCoroutine(HandleBGMusinOnWinLose());
                break;
            case SoundFX.Win:
                clip = win;
                StartCoroutine(HandleBGMusinOnWinLose());
                break;
        }
        sound.pitch = 1;
        sound.PlayOneShot(clip);
    }
    IEnumerator HandleBGMusinOnWinLose()
    {
        music.mute = true;
        yield return new WaitUntil(() => sound.isPlaying == false);
        music.mute = false;
    }
    public void PlayComboSound(int combo)
    {
        combo = Mathf.Clamp(combo, 1, 10);
        if (combo > 1) sound.pitch += combo / 10.0f;
        sound.PlayOneShot(match);
    }

}
