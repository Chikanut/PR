using System.Collections.Generic;
using Levels;
using UnityEngine;
using Random = UnityEngine.Random;

public class DifficultySpawner : Resetable
{
    public DifficultySpawner(DifficultyConfig config, ISpawner spawner)
    {
        _difficultyConfig = config;

        _spawner = spawner;
        _spawner.Init(GetNewPattern);

        ResetObjects();
        Enable(true);
    }

    readonly DifficultyConfig _difficultyConfig;
    readonly ISpawner _spawner;

    bool _enabled = false;

    float _currentDistance = 0;
    private TextAsset _currentPatterinTextAsset;
    float _locationStartDistance = 0;

    public void Enable(bool isEnabled)
    {
        _enabled = isEnabled;
    }

    public void OnDistanceChanged(float value)
    {
        _currentDistance = value;

        if (!_enabled)
            return;

        _spawner.OnDistanceChanged(value);
    }

    public void ChangeColor(LevelColors c)
    {
        _spawner.ChangeColor(c);
    }

    public void ResetObjects()
    {
        _spawner.ResetObjects();
    }

    void GetNewPattern()
    {
        if (_difficultyConfig == null)
            return;

        _currentPatterinTextAsset =
            GetRandomPatternNum(_difficultyConfig.GetLevelConfig(_currentDistance).TextAssets);

        _spawner.SetNewPattern(ObjectSerializer.DeserializeObject(_currentPatterinTextAsset));
    }

    private TextAsset GetRandomPatternNum(IReadOnlyList<TextAsset> targetList)
    {
        var patternNum = 0;

        if (targetList.Count < 2 || _currentPatterinTextAsset == null)
            patternNum = Random.Range(0, targetList.Count);
        else
        {
            var randomList = new List<int>();

            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] != _currentPatterinTextAsset)
                    randomList.Add(i);
            }

            patternNum = randomList[Random.Range(0, randomList.Count)];
        }

        return targetList[patternNum];
    }

    public void OnReset(GameState state)
    {
        ResetObjects();
    }
}

