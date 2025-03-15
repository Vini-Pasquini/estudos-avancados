using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioManager
{
    [SerializeField] private AudioMixer _audioMixer = null;
    [SerializeField] private AudioSource _musicSource = null;
    [SerializeField] private AudioSource _sfxSource = null;
    
    [SerializeField] private List<AudioSource> _sfxSourcePool = new List<AudioSource>();

    /*
     * MUSIC
     */

    public void PlayMusic(AudioClip music, bool hasLoop = true)
    {
        if (music == null) return;

        this._musicSource.Stop();
        this._musicSource.loop = hasLoop;
        this._musicSource.volume = 1f;
        this._musicSource.clip = music;
        this._musicSource.Play();
    }

    public void StopMusic()
    {
        if (!this._musicSource.isPlaying) return;

        this._musicSource.Stop();
    }

    public void PauseMusic(bool shouldPause)
    {
        if (shouldPause) this._musicSource.Pause();
        else this._musicSource.UnPause();

    }

    public bool IsPlayingMusic()
    {
        return this._musicSource.isPlaying;
    }

    public void FadeInMusic(AudioClip music, float fadeTime = .5f)
    {
        if (music == null) return;

        if (this._musicSource.isPlaying) this._musicSource.Stop();

        this._musicSource.clip = music;
        this._musicSource.volume = 0f;
        this._musicSource.Play();

        GameManager.Instance.StartCoroutine(FadeMusic(true, fadeTime));
    }

    public void FadeOutMusic(float fadeTime = .5f)
    {
        if (!this._musicSource.isPlaying) return;

        GameManager.Instance.StartCoroutine(FadeMusic(false, fadeTime));
    }

    private IEnumerator FadeMusic(bool isFadeIn, float fadeTime)
    {
        float deltaTime = 0f;
        float targetVolume = (isFadeIn ? 1f : 0f);
        float currentVolume = this._musicSource.volume;

        while (deltaTime < fadeTime)
        {
            deltaTime += Time.deltaTime;
            this._musicSource.volume = Mathf.Lerp(currentVolume, targetVolume, deltaTime/fadeTime);
            yield return null;
        }

        this._musicSource.volume = targetVolume;
    }
}
