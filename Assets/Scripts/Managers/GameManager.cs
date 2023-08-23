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
            //if (_lives > value)
            //    Respawn();

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

    // Start is called before the first frame update

    public void Start()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        _lives = maxLives;
        _fuel = 100;
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
        }
        else
        {
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }
    //void OnSceneLoadedCallback(SceneloadedScene, LoadSceneMode) 
    //{
    //    if (loadedScene.name == "Level")
    //    {
    //        //do level setup stuff
    //    }
    //}


    public void SpawnPlayer(Transform spawnPoint)
    {
        currentSpawnPoint = spawnPoint;
    }

    //void Respawn()
    //{
    //    if (playerInstance)
    //        playerInstance.transform.position = currentSpawnPoint.position;
    //}

    void Gameover()
    {
        timeController.UpdateLastTime();
        timeController.UpdateBestTime();
        SceneManager.LoadScene("Win");
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
        if (IncrementTimer >= incrementInterval) // check if interval exceeded
        {
            scrollSpeed -= (int)incrementRate; // increase speed by increment
            Debug.Log("Scroll speed set to" + scrollSpeed);
            IncrementTimer = 0.0f; // Reset timer
        }
    }

    public static void SetTimeManager(TimeController TC)
    {
        timeController = TC;
    }
}
