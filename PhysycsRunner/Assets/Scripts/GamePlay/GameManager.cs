using System.Collections.Generic;
using System.Linq;
using Levels;
using Levels.Objects;
using UnityEngine;
using UnityEngine.SceneManagement;
using Visartech.Progress;

public class LevelColors
{
    public Color LightColor;
    public Color DarkColor;
}

public class GameManager : Reseter
{
    static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();

            return _instance;
        }
    }

    [HideInInspector] public LevelColors CurrentColors;
    
    [Header("Testing")]
    [SerializeField] bool _isTesting;
    [SerializeField] int _testLevelNum;

    [Header("Configs")]
    [SerializeField] DifficultyConfig _difficultyConfig;
    [SerializeField] LevelsConfig _levelsConfig;
    [SerializeField] ObjectsConfig _objectsConfig;
    [SerializeField] MenuConfig _menuConfig;
    [SerializeField] TextAsset _transitionPattern;
    [SerializeField] SoundsConfig _soundsConfig;

    [Header("Controllers")]
    [SerializeField] CameraController _camera;
    [SerializeField] Player _player;
    [SerializeField] SoundController _soundController;

    [Header("Settings")]
    [SerializeField] int _minPatternsCount;
    [SerializeField] int _maxPatternsCount;
    [SerializeField] Gradient _lightColors;
    [SerializeField] Gradient _darkColors;
    
    [Header("UI")]
    [SerializeField] Transform _menusParent;
    [SerializeField] Transform _popUpsParent;
    [SerializeField] CanvasGroup _canvasGroup;
    MenuNavigationController _navigationController;
    ShowableFactory _menusFactory;
    GameScreen _gameScreen;
     
    ObjectsPool _pool;
    LevelSpawner _levelSpawner;
    ISpawner _spawner;
    
    private Vector3 _startPos;
    private float _distance = 0;

    public static GameState CurrentState = GameState.mainMenu;
    
    void Awake()
    {
        Init();
    }

    void Init()
    {
        Application.targetFrameRate = 120;
        
        _pool = new ObjectsPool(new GameObject("_ObjectsPool").transform, _objectsConfig);
        _soundController.Init(_soundsConfig);
        _spawner = new PatternSpawner(_pool);
        _levelSpawner = new LevelSpawner(_spawner);
        _menusFactory = new ShowableFactory(_menuConfig);
        _navigationController = new MenuNavigationController(_menusFactory, _menusParent, _popUpsParent, _canvasGroup);

        _soundController.PlayMusic(AudioClipType.MainMusic);  
        _startPos = _player.transform.position;
        
        AddResetable(_camera);
        UpdateState();
    }
    
    void OnMainMenu()
    {
        CurrentState = GameState.mainMenu;
        
        NotifyResetables(CurrentState);
        
        SetLevel();

        _navigationController.ShowMenuScreen<MainMenuScreen>()
            .Init(OnGame, OnSettings);
    }

    void OnSettings()
    {
        _navigationController.ShowPopup<SettingsPopup>()
            .Init((b) => _soundController.MusicEnabled = b);
    }

    void OnGame()
    {
        CurrentState = GameState.game;

        NotifyResetables(CurrentState);
        ChangeColors();
        _player.SetColor(CurrentColors.LightColor);

        var mainMenu = _navigationController.GetMenuScreen<MainMenuScreen>();

        if (mainMenu)
            mainMenu.PlayHideAnimation(() =>
            {
                _gameScreen = _navigationController.ShowMenuScreen<GameScreen>();
                _gameScreen.Init(_player.transform);
                _gameScreen.SetNewDistance(GetLevelDistance(_patterns));
            });
        else
        {
            SetLevel();
            
            _gameScreen = _navigationController.ShowMenuScreen<GameScreen>();
            _gameScreen.Init(_player.transform);
            _gameScreen.SetNewDistance(GetLevelDistance(_patterns));
            _gameScreen.ShowLevel(Progress.Levels.Level);
        }
    }

    public void OnDeath()
    {
        Restart();
    }

    void Restart()
    {
        CurrentState = GameState.game;
        SceneManager.LoadScene(0);
    }

    void Reset()
    {
        CurrentState = GameState.mainMenu;
        SceneManager.LoadScene(0);
    }
    
    void UpdateState()
    {
        switch (CurrentState)
        {
            case GameState.game: OnGame();
                break;
            case GameState.results: OnGame();
                break;
            case GameState.mainMenu: OnMainMenu();
                break;
        }
    }

    TextAsset[] _patterns;

    void SetLevel()
    {
        SetLevel(Progress.Levels.Level);
    }

    void SetLevel(int lvl)
    {
        var level = _isTesting ? _testLevelNum : lvl;

        if (level < _levelsConfig.LevelConfigs.Count)
            _patterns = _levelsConfig.LevelConfigs[level].TextAssets;
        else // if levels ended
            SpawnRandomLevel(lvl);
        
        _levelSpawner.SetLevel(_patterns, OnLevelCompleated);
    }

    void SpawnRandomLevel(int lvl)
    {
        if (Progress.Levels.RandomPattenrs == null)
        {
            var patternsCount = Random.Range(_minPatternsCount, _maxPatternsCount);
            
            Progress.Levels.RandomLevelsConfig = lvl;
            Progress.Levels.RandomPattenrs = GetRandomPatterns(lvl, patternsCount);
        }

        if (Progress.Levels.Level != lvl || Progress.Levels.NextRandomLevel == null)
        {
            var patternsCount = Random.Range(_minPatternsCount, _maxPatternsCount);
            
            Progress.Levels.NextRandomConfig = lvl;
            Progress.Levels.NextRandomLevel = GetRandomPatterns(lvl, patternsCount);
        }

        _patterns = lvl == Progress.Levels.Level
            ? GetPattenrsAssets(Progress.Levels.RandomLevelsConfig, Progress.Levels.RandomPattenrs)
            : GetPattenrsAssets(Progress.Levels.NextRandomConfig, Progress.Levels.NextRandomLevel);
    }

    TextAsset[] GetPattenrsAssets(int difficulty, int[] nums)
    {
        var patterns = new TextAsset[nums.Length];
        var difficultyConfig = _difficultyConfig.GetLevelConfig(difficulty);
        
        for (var i = 0; i < patterns.Length; i++)
            patterns[i] = difficultyConfig.TextAssets[nums[i]];

        return patterns;
    }

    int[] GetRandomPatterns(int difficulty, int pattenrsCount)
    {
        var difficultyConfig = _difficultyConfig.GetLevelConfig(difficulty);
        
        var pattenrsNum = new int[pattenrsCount];

        for (var i = 0; i < pattenrsNum.Length; i++)
            pattenrsNum[i] = Random.Range(0, difficultyConfig.TextAssets.Length);

        return pattenrsNum;
    }

    float GetLevelDistance(IEnumerable<TextAsset> patterns)
    {
        return patterns.Sum(t => ObjectSerializer.DeserializeObject(t).Size.z);
    }

    void OnLevelCompleated() // add transition pattern
    {
        ChangeColors();
        
        _spawner.SetNewPattern(ObjectSerializer.DeserializeObject(_transitionPattern));

        SetLevel(Progress.Levels.Level + 1); 
    }

    public void OnFinished()
    {       
        Progress.Levels.Level++;

        Progress.Levels.RandomLevelsConfig = Progress.Levels.NextRandomConfig;
        Progress.Levels.RandomPattenrs = Progress.Levels.NextRandomLevel;

        if (_gameScreen != null)
        {
            _gameScreen.ShowLevel(Progress.Levels.Level);
            _gameScreen.SetNewDistance(GetLevelDistance(_patterns));
        }
    }

    void ChangeColors()
    {
        var pr = Random.Range(0f, 1f);
        var colors = new LevelColors()
        {
            LightColor = _lightColors.Evaluate(pr),
            DarkColor = _darkColors.Evaluate(pr)
        };
        
        _levelSpawner?.ChangeColor(colors);
        CurrentColors = colors;
    }

    void Update()
    {
        if(CurrentState != GameState.game)
            return;
        
        _distance = _player.transform.position.z - _startPos.z;
        
        _levelSpawner.OnDistanceChanged(_distance);
    }
}
