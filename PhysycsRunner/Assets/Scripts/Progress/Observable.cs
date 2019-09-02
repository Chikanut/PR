using System.Collections.Generic;

public abstract class Observable {
    
    private readonly HashSet<Observer> _observers = new HashSet<Observer>();

    public virtual void AddObserver(Observer observer) {
        _observers.Add(observer);
    }

    public virtual void RemoveObserver(Observer observer) {
        _observers.Remove(observer);
    }

    public virtual void NotifyObservers() {
        foreach (var observer in _observers) {
            observer.OnNotify(this);
        }
    }
}
