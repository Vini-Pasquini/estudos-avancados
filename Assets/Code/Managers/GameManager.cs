using UnityEngine;

public class GameManager : IPersistentSingleton<GameManager>
{
    public AudioManager audioManager = new AudioManager();

    protected override void Awake()
    {
        base.Awake();

        this.audioManager.InitializeAudioPool();

        // exemplo de utilizacao da lambda
        //AudioManager.AudioStop audioStop = audioManager.PlaySFXInLoop(null);
        //audioStop();
    }
}
