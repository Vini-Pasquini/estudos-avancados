using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioManager
{
    [SerializeField] private AudioMixer _audioMixer = null;
    [SerializeField] private AudioSource _musicSource = null;
    [SerializeField] private AudioSource _sfxSource = null;

    [SerializeField] private int _poolSize = 10;
    [SerializeField] private List<AudioSource> _sfxSourcePool = new List<AudioSource>();
    private GameObject _audioSourceInstance;

    public delegate void AudioStop();

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

    /*
     * SFX
     */

    public void InitializeAudioPool()
    {
        this._audioSourceInstance = new GameObject("AudioSourceInstances");
        this._audioSourceInstance.transform.SetParent(GameManager.Instance.transform);

        for (int i = 0; i < this._poolSize; i++)
        {
            AudioSource newAudioSource = this._audioSourceInstance.AddComponent<AudioSource>();
            newAudioSource.outputAudioMixerGroup = this._audioMixer.FindMatchingGroups("SFX")[0];
            newAudioSource.playOnAwake = false;
            this._sfxSourcePool.Add(newAudioSource);
        }
    }

    public void PlaySFX(AudioClip sfx, float volume = 1f, float pitch = 1f)
    {
        this.InternalPlaySFX(sfx, volume, pitch, false);
    }

    public void PlaySFX(AudioCue sfx)
    {
        this.InternalPlaySFX(sfx.GetSample(), sfx.GetVolume(), sfx.GetPitch(), false);
    }

    public AudioStop PlaySFXInLoop(AudioClip sfx, float volume = 1f, float pitch = 1f)
    {
        return this.InternalPlaySFX(sfx, volume, pitch, true);
    }

    public AudioStop PlaySFXInLoop(AudioCue sfx)
    {
        return this.InternalPlaySFX(sfx.GetSample(), sfx.GetVolume(), sfx.GetPitch(), true);
    }

    public void PauseSFX(bool shouldPause)
    {
        if (shouldPause)
        {
            this._sfxSource.Pause();
            for (int i = 0; i < _sfxSourcePool.Count; i++) { this._sfxSourcePool[i].Pause(); }
            return;
        }

        this._sfxSource.UnPause();
        for (int i = 0; i < _sfxSourcePool.Count; i++) { this._sfxSourcePool[i].UnPause(); }
    }

    public void StopSFX()
    {
        this._sfxSource.Stop();
        for (int i = 0; i < _sfxSourcePool.Count; i++) { this._sfxSourcePool[i].Stop(); }
    }

    private AudioStop InternalPlaySFX(AudioClip sfx, float volume, float pitch, bool hasLoop = false)
    {
        int index = -1;

        if (this._sfxSource.isPlaying)
        {
            for(int i = 0; i < this._sfxSourcePool.Count; i++)
            {
                if (this._sfxSourcePool[i].isPlaying) continue;
                index = i;
                break;
            }
            if (index == -1)
            {
                index = CreateAudioSource();
            }
        }

        AudioSource audioSource = index == -1 ? _sfxSource : _sfxSourcePool[index];

        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.loop = hasLoop;

        if (hasLoop)
        {
            audioSource.clip = sfx;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(sfx);
        }

        AudioStop result = () => { audioSource.Stop(); };
        return result;
    }

    private int CreateAudioSource()
    {
        AudioSource newAudioSource = this._audioSourceInstance.AddComponent<AudioSource>();
        newAudioSource.outputAudioMixerGroup = this._audioMixer.FindMatchingGroups("SFX")[0];
        newAudioSource.playOnAwake = false;
        this._sfxSourcePool.Add(newAudioSource);
        return this._sfxSourcePool.Count - 1;
    }
}
