using System.ComponentModel;
using UnityEngine;

[System.Serializable]
public class Progression
{
    public float Time;
    public AnimationCurve ProgressionCurve;
    public bool Loop;

    public float MinValue;
    public float MaxValue;
    
    private float _currentTime;

    public void Update(float deltaTime)
    {
        _currentTime += deltaTime;

        _currentTime = Mathf.Clamp(_currentTime, 0, Time);
        
        if (_currentTime >= Time && Loop)
            _currentTime = 0;
    }

    public float Evaluate()
    {
        return Mathf.Lerp(MinValue, MaxValue, ProgressionCurve.Evaluate(_currentTime / Time));
    }

    public void Reset()
    {
        _currentTime = 0;
    }
}
