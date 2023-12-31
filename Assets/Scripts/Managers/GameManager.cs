using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    static GameManager _instance = null;
    static TimeController timeController;
    public static int scrollSpeed = -120;
    public float incrementRate = 5.0f; // speed increase increment
    private float IncrementTimer;
    private float incrementInterval = 5.0f; // speed increase interval (seconds)
    public float maxScrollSpeed = -400;
    public bool connectedToGooglePlay;

    public static GameManager instance
    {
        get => _instance;
        set
        {
            _instance = value;
        }
    }

    public int maxLives = 5;
    private int _lives = 0;
    private int _score = 0;
    private float _fuel = 100;
    public float maxFuel = 100;

    // public PlayerController playerPrefab;
    [HideInInspector] public PlayerController playerInstance = null;
    [HideInInspector] public Level currentLevel = null;
    [HideInInspector] public Transform currentSpawnPoint;

    public int score
    {
        get { return _score; }
        set
        {

            onScoreValueChanged?.Invoke(_score + 100);
            Debug.Log("Score has been set to: " + _score.ToString());
        }
    }

    public int lives
    {
        get { return _lives; }
        set
        {
            _lives = value;

            if (_lives > maxLives)
                _lives = maxLives;

            if (_lives <= 0)
                Gameover();

            onLifeValueChanged?.Invoke(_lives);
        }
    }

    public float fuel
    {
        get { return _fuel; }
        set
        {
            _fuel = value;

            if (_fuel > maxFuel)
                _fuel = maxFuel;

            onFuelValueChanged?.Invoke(_fuel);
        }
    }

    [HideInInspector] public UnityEvent<int> onLifeValueChanged;
    [HideInInspector] public UnityEvent<int> onScoreValueChanged;
    [HideInInspector] public UnityEvent<float> onFuelValueChanged;

    private void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        _lives = maxLives;
        _fuel = 100;
        Time.timeScale = 1;
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            connectedToGooglePlay = true;
            // Continue with Play Games Services
        }
        else
        {
            connectedToGooglePlay = false;
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }

    public void SpawnPlayer(Transform spawnPoint)
    {
        currentSpawnPoint = spawnPoint;
    }

    void Gameover()
    {
        Time.timeScale = 0;
        timeController.UpdateLastTime();
        timeController.UpdateBestTime();

        if(connectedToGooglePlay)
        {
            Social.ReportScore(timeController.longLastAttemptTime, GPGSIds.leaderboard_helirun_top_runs, LeaderboardUpdate);
            Social.ShowLeaderboardUI();
        }
    
        SceneManager.LoadScene("Win");
    }

    private void LeaderboardUpdate(bool success)
    {
        if (success) Debug.Log("Updated Leaderboard");
        else Debug.Log("Unable to update Leaderboard");
    }

    public void Win()
    {
        SceneManager.LoadScene("Win");
    }

    // Update is called once per frame
    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        // Check if the current scene is the game scene
        if (currentScene.name == "Level")
        {
            _fuel -= 1.0f * Time.deltaTime; // reduce fuel over time
            if (_fuel <= 0)
                Gameover();
        }
        else
        {
            // Ignore the timeController elements in the title scene
        }

        IncrementTimer += Time.deltaTime;
        if (IncrementTimer >= incrementInterval && scrollSpeed > maxScrollSpeed) // check if interval exceeded and we haven't hit the max scroll speed
        {
            scrollSpeed -= (int)incrementRate; // increase speed by increment
            //Debug.Log("Scroll speed set to" + scrollSpeed);
            IncrementTimer = 0.0f; // Reset timer
        }
    }

    public static void SetTimeManager(TimeController TC)
    {
        timeController = TC;
    }
}
