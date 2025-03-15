using UnityEngine;

public class GameManager : IPersistentSingleton<GameManager>
{
    public AudioManager audioManager = new AudioManager();
}
