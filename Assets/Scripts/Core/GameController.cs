using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance {get{return instance;}}

    [Header("Refs")]
    [SerializeField] EnemySpawner spawner;         
    public PlayerController player;
    [SerializeField] private List<GameObject> maps;

    [Header("Variables")]
    [SerializeField] bool autoStart = true;
    public int coinCollected = 0; // Số coin đã thu thập

    [Header("Data")]
    [SerializeField] private Data data;
    
    // --- NEW: chống race ---
    private int deathBuffer = 0;              // số enemy chết dồn trong frame
    private int pendingSpawnRequests = 0;     // số request spawn đã "đặt vé" nhưng chưa spawn xong

    public GameState State { get; private set; }
    public int Alive { get; private set; }          // Enemy còn sống

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        LoadData();

        foreach (GameObject map in maps)
        {
            map.SetActive(false);
        }
        
        maps[data.GetCurrentLevel()].SetActive(true);
    }
    
    void OnEnable()
    {
        EventObserver.OnAnyEnemyDead += OnEnemyDeadEvent;
        if (spawner != null) spawner.OnEnemySpawned += OnEnemySpawned;
    }

    void OnDisable()
    {
        EventObserver.OnAnyEnemyDead -= OnEnemyDeadEvent;
        if (spawner != null) spawner.OnEnemySpawned -= OnEnemySpawned;
    }

    void Start()
    {
        //if (player) SetPlayerControl(false);
        
        Alive = spawner.MaxSpawnCount + 1;
        EventObserver.RaiseAliveChanged(Alive);
    }

    private void Update()
    {
        if(State == GameState.Win || State == GameState.Lose)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RestartScene();
            }
            return;
        }
        
        if (State != GameState.Playing) return;

        // XỬ LÝ CHẾT DỒN MỘT LẦN / FRAME
        if (deathBuffer > 0)
        {
            int deaths = deathBuffer;
            deathBuffer = 0;

            Alive = Mathf.Max(0, Alive - deaths);
            EventObserver.RaiseAliveChanged(Alive);

            // Tính số slot có thể spawn bù (không vượt quota)
            if (spawner != null)
            {
                int available = spawner.MaxSpawnCount - spawner.TotalSpawned - pendingSpawnRequests;
                int toSpawn = Mathf.Min(deaths, Mathf.Max(0, available));

                // ĐẶT VÉ trước khi gọi TrySpawn để tránh 2 lần cùng frame vượt quota
                pendingSpawnRequests += toSpawn;
                for (int i = 0; i < toSpawn; i++)
                {
                    spawner.TrySpawnOneAfterDelay(1f); // có thể randomize delay một chút nếu muốn
                }
            }

            // Win khi không còn enemy nào
            if (Alive == 1)
            {
                EndGameWin();
            }
        }
    }
    
    // --- NEW: Lưu/Load dữ liệu ---
    public void SaveData()
    {
        string saveString = JsonUtility.ToJson(data);

        SaveSystem.Save("save", saveString);
    }
    public void LoadData()
    {
        string loadedData = SaveSystem.Load("save");
        if (loadedData != null)
        {
            data = JsonUtility.FromJson<Data>(loadedData);
        }
        else
        {
            data = new Data();
        }
    }
    public Data GetData()
    {
        return data;
    }

    // --- NEW: Quản lý trạng thái game ---
    public void StartGame()
    {
        if (State == GameState.Playing) return;
        SetState(GameState.Playing);
        
        player.SetPlayerAttackRange(true);

        //if (player) SetPlayerControl(true);
    }
    public void PauseGame()
    {
        if (State != GameState.Playing) return;
        SetState(GameState.Paused);
        Time.timeScale = 0f;
        if (player) SetPlayerControl(false);
    }
    public void ResumeGame()
    {
        if (State != GameState.Paused) return;
        SetState(GameState.Playing);
        Time.timeScale = 1f;
        if (player) SetPlayerControl(true);
    }
    public void EndGameWin()
    {
        if (State == GameState.Win || State == GameState.Lose) return;
        SetState(GameState.Win);

        data.SetCurrentLevel(data.GetCurrentLevel() + 1);

        if (data.GetCurrentLevel() >= 2)
        {
            data.SetCurrentLevel(0);
        }
        
        if (player)
        {
            //SetPlayerControl(false);
            player.WinDance();
        }
        
        SaveData();
    }
    public void EndGameLose()
    {
        if (State == GameState.Win || State == GameState.Lose) return;
        SetState(GameState.Lose);
        
        //if (player) SetPlayerControl(false);
    }
    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // --- NEW: Xử lý sự kiện ---
    void OnEnemyDeadEvent(EnemyAI ai)
    {
        // Chỉ tăng buffer, không xử lý logic tại đây để tránh race
        deathBuffer++;
        coinCollected++;
    }

    void OnEnemySpawned()
    {
        // Spawner gọi khi đã Instantiate xong
        pendingSpawnRequests = Mathf.Max(0, pendingSpawnRequests - 1);
    }

    void SetState(GameState s)
    {
        State = s;
        EventObserver.RaiseGameStateChanged(State);
    }

    void SetPlayerControl(bool enabled)
    {
        //player.EnableControl(enabled);  
    }
}
