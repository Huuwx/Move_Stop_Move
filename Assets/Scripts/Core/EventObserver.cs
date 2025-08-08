using System;
using UnityEngine;

public class EventObserver : MonoBehaviour
{
    private static EventObserver instance;
    public static EventObserver Instance { get { return instance; } }
    
    public event Action<int> OnAliveChanged;
    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RaiseAliveChanged(int alive) => OnAliveChanged?.Invoke(alive);
    public void RaiseGameStateChanged(GameState s) => OnGameStateChanged?.Invoke(s);
}
