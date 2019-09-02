using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    mainMenu,
    game,
    results,
    pause
}

public abstract class Reseter : MonoBehaviour{
    
    private readonly HashSet<Resetable> _observers = new HashSet<Resetable>();

    public virtual void AddResetable(Resetable observer) {
        _observers.Add(observer);
    }

    public virtual void RemoveResetable(Resetable observer) {
        _observers.Remove(observer);
    }

    public virtual void NotifyResetables(GameState state) {
        foreach (var observer in _observers) {
            observer.OnReset(state);
        }
    }
}
