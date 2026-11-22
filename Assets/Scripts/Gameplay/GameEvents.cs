using System;

// Static events outlive every scene, so whatever subscribes MUST unsubscribe in OnDisable (see HitFlash).
public static class GameEvents
{
    public static event Action<int, int> PlayerDamaged;

    // Only this class can invoke its own events, so everyone else raises them through this method.
    public static void RaisePlayerDamaged(int current, int max) => PlayerDamaged?.Invoke(current, max);
}