using UnityEngine;

[System.Serializable]
public struct AudioCue
{
    [SerializeField][Range(0f, 3f)] private float _minPitch;
    [SerializeField][Range(0f, 3f)] private float _maxPitch;
    [Space(10)]
    [SerializeField][Range(0f, 1f)] private float _minVolume;
    [SerializeField][Range(0f, 1f)] private float _maxVolume;
    [Space(10)]
    [SerializeField] private AudioClip[] _audioSamples;

    public AudioClip GetSample() { return this._audioSamples[Random.Range(0, this._audioSamples.Length)]; }
    public float GetPitch() { return Random.Range(this._minPitch, this._maxPitch); }
    public float GetVolume() { return Random.Range(this._minVolume, this._maxVolume); }
}