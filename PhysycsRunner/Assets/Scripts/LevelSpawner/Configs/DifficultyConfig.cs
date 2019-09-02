using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DifficultyPatterns {
    public float Distance;
    public TextAsset[] TextAssets;
}

[CreateAssetMenu(fileName = "DifficultyPatternsConfig", menuName = "Configs/DifficultyPatternsConfig")]
public sealed class DifficultyConfig : ScriptableObject
{
    public List<DifficultyPatterns> LevelConfigs = new List<DifficultyPatterns>();

    public DifficultyPatterns GetLevelConfig(float distance)
    {
        DifficultyPatterns lvl = LevelConfigs[0];
        
        for (int i = 0; i < LevelConfigs.Count ; i++)
        {
            if (LevelConfigs[i].Distance <= distance)
                lvl = LevelConfigs[i];
        }

        return lvl;
    }
}