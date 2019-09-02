using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelPatterns
{
    public TextAsset[] TextAssets;
}

[CreateAssetMenu(fileName = "LevelsPatternsConfig", menuName = "Configs/LevelsPatternsConfig")]
public class LevelsConfig : ScriptableObject
{
    public List<LevelPatterns> LevelConfigs = new List<LevelPatterns>();
}
