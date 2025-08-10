using System;
using UnityEngine;

public class EventObserver : MonoBehaviour
{
    private static EventObserver instance;
    public static EventObserver Instance { get { return instance; } }
    
    public static event Action<int> OnAliveChanged;
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<EnemyAI> OnAnyEnemyDead;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public static void RaiseAliveChanged(int alive) => OnAliveChanged?.Invoke(alive);
    public static void RaiseGameStateChanged(GameState s) => OnGameStateChanged?.Invoke(s);
    public static void RaiseOnAnyEnemyDead(EnemyAI ai) => OnAnyEnemyDead?.Invoke(ai);
}
