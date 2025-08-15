using System;
using UnityEngine;

public class EventObserver : MonoBehaviour
{
    public static event Action<int> OnAliveChanged;
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<EnemyAI> OnAnyEnemyDead;
    public static event Action OnUpgrade;

    public static void RaiseAliveChanged(int alive) => OnAliveChanged?.Invoke(alive);
    public static void RaiseGameStateChanged(GameState s) => OnGameStateChanged?.Invoke(s);
    public static void RaiseOnAnyEnemyDead(EnemyAI ai) => OnAnyEnemyDead?.Invoke(ai);
    public static void RaiseOnUpgrade() => OnUpgrade?.Invoke();
    
    public static void ResetStaticEvents()
    {
        OnAliveChanged = null;
        OnGameStateChanged = null;
        OnAnyEnemyDead = null;
        OnUpgrade = null;
    }
}
