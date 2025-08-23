using System;
using UnityEngine;

public class EventObserver : MonoBehaviour
{
    public static event Action<int> OnAliveChanged;
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<EnemyBase> OnAnyEnemyDead;
    public static event Action OnUpgrade;
    public static event Action OnGiftCollected;

    public static void RaiseAliveChanged(int alive) => OnAliveChanged?.Invoke(alive);
    public static void RaiseGameStateChanged(GameState s) => OnGameStateChanged?.Invoke(s);
    public static void RaiseOnAnyEnemyDead(EnemyBase enemy) => OnAnyEnemyDead?.Invoke(enemy);
    public static void RaiseOnUpgrade() => OnUpgrade?.Invoke();
    public static void RaiseOnGiftCollected() => OnGiftCollected?.Invoke();
    
    public static void ResetStaticEvents()
    {
        OnAliveChanged = null;
        OnGameStateChanged = null;
        OnAnyEnemyDead = null;
        OnUpgrade = null;
        OnGiftCollected = null;
    }
}
