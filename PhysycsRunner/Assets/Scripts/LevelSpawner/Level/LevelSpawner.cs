using System;
using Levels;
using UnityEngine;

public class LevelSpawner
{
    public LevelSpawner(ISpawner spawner)
    {       
        _spawner = spawner;
        _spawner.Init(GetNewPattern);
    }

    private Action _onComplete;
    
    private int _currentPattern = 0;

    private TextAsset[] _patterns;
    
    private readonly ISpawner _spawner;

    public void SetLevel(TextAsset[] patterns, Action onComplete)
    {
        _onComplete = onComplete;
     
        _currentPattern = 0;
        _patterns = patterns;
    }

    public void ChangeColor(LevelColors c)
    {
        _spawner.ChangeColor(c);
    }

    public void ResetObjects()
    {
        _spawner?.ResetObjects();
    }

    public void OnDistanceChanged(float v)
    {       
        if(_patterns != null)
            _spawner?.OnDistanceChanged(v);
    }

    void GetNewPattern()
    {       
        if(_patterns == null)
            return;

        if (_patterns.Length <= _currentPattern)
        {
           
            _onComplete?.Invoke();  
            
            return;
        }

        var currentPattern =
            ObjectSerializer.DeserializeObject(_patterns[_currentPattern]);
        
        _spawner.SetNewPattern(currentPattern);

        _currentPattern++;
    }
}
