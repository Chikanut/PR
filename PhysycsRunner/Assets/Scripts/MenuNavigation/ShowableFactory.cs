using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

public class ShowableFactory {

    private readonly Dictionary<Type, Showable> _showables = new Dictionary<Type, Showable>();
    private readonly Dictionary<Type, Showable> _instantiatedShowables = new Dictionary<Type, Showable>();
    private readonly MenuConfig _menuConfig;
    
    public ShowableFactory(MenuConfig menuConfig) {
        _menuConfig = menuConfig;
        foreach (var showable in _menuConfig.Showables) {
            _showables.Add(showable.GetType(), showable);
        }
    }

    public T Create<T>() where T : Showable {
        var type = typeof(T);
        if (_instantiatedShowables.ContainsKey(type)) return (T) _instantiatedShowables[type];
        var showable = Object.Instantiate(_showables[type]);
        _instantiatedShowables.Add(type, showable);
        return (T)showable;
    }
}
