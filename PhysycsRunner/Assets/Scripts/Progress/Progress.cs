using UnityEngine;
using System;
using System.Linq;

namespace Visartech.Progress
{
    [Serializable]
    public class Progress : ProgressBase<Progress>
    {
        
        [Serializable]
        public class PlayerData : Observable {

            public PlayerData()
            {
                BestRecord = 0;
            }
            
            private int _bestRecord;

            public int BestRecord {
                get => _bestRecord;
                set {
                    if(_bestRecord >= value) return;
                    _bestRecord = value;
                }
            }

        }
        
        [Serializable]
        public class LevelsData : Observable
        {
            public LevelsData()
            {
                Level = 0;
                NextRandomConfig = 0;
                RandomLevelsConfig = 0;
            }
            
            private int _level;

            public int Level {
                get => _level;
                set {
                    if(_level >= value) return;
                    _level = value;
                }
            }
            
            public int NextRandomConfig;
            public int[] NextRandomLevel;
            public int RandomLevelsConfig;
            public int[] RandomPattenrs;

        }

        [Serializable]
        public class GameData
        {
            public bool SoundEnabled = true;
            public bool MusicEnabled = true;
        }

        [Serializable]
        public class StatisticsData
        {
            public bool isFirstPlay = false;

            const string DateFormat = "yyyy.MM.dd";
            public string[] DaysInGame = new string[0];

            public int GetTotalDaysInGame
            {
                get
                {
                    if (DaysInGame.Any(t => t.Equals(DateTime.Now.ToString(DateFormat))))
                        return DaysInGame.Length;

                    AddDay();
                    
                    return DaysInGame.Length;
                }
            }

            public void AddDay()
            {
                var newArr = new string[DaysInGame.Length + 1];

                DaysInGame.CopyTo(newArr, 0);
                newArr[newArr.Length - 1] = DateTime.Now.ToString(DateFormat);
                DaysInGame = newArr;
            }
        }

        private PlayerData _playerData;
        private LevelsData _levelData;
        private GameData _gameData = new GameData();
        private StatisticsData _statisticsData = new StatisticsData();

        public static PlayerData Player
        {
            get => instance._playerData;
            set => instance._playerData = value;
        }
        
        public static LevelsData Levels
        {
            get => instance._levelData;
            set => instance._levelData = value;
        }

        public static GameData Game {
            get => instance._gameData;
            set => instance._gameData = value;
        }
        
        public static StatisticsData Statistics {
            get => instance._statisticsData;
            set => instance._statisticsData = value;
        }

        public void Reset()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            ClearAllFields();
        }
    }
}