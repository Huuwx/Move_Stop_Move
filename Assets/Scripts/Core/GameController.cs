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
    [SerializeField] PlayerController player;
    [SerializeField] private UIController uiController;
    [SerializeField] private ListWeapon listWeapon;
    [SerializeField] private List<GameObject> maps;

    [Header("Variables")]
    public int coinCollected = 0; // Số coin đã thu thập
    public float time = 3;
    public bool isPlaying = false;

    [Header("Data")]
    [SerializeField] private Data data;
    
    // --- NEW: chống race ---
    private int deathBuffer = 0;              // số enemy chết dồn trong frame
    private int pendingSpawnRequests = 0;     // số request spawn đã "đặt vé" nhưng chưa spawn xong

    public GameState State { get; private set; }
    public GameMode mode;
    public int Alive { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        State = GameState.Home; // Khởi tạo trạng thái ban đầu
        
        LoadData();

        if (mode == GameMode.Normal)
        {
            foreach (GameObject map in maps)
            {
                map.SetActive(false);
            }

            maps[data.GetCurrentLevel()].SetActive(true);
        }
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
        
        SetState(GameState.Ready);
        
        //spawner = FindObjectOfType<EnemySpawner>();
        //player = FindObjectOfType<PlayerController>();
        
        if(mode == GameMode.Normal)
            Alive = spawner.MaxSpawnCount + 1;
        else
        {
            FindObjectOfType<ZombieCity.Abilities.AbilityDraftPanel>(true).ShowDraft();
            Alive = spawner.MaxSpawnCount;
        }

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
        
        if (mode == GameMode.Zombie && State == GameState.Ready)
        {
            if(!isPlaying) return;
            time -= Time.deltaTime;
            uiController.UpdateTimeCounter(Mathf.CeilToInt(time));
            if (time <= 0)
            {
                StartGame();
                time = 5;
            }
        }

        if (State == GameState.WaitForRevive)
        {
            time -= Time.deltaTime;
            uiController.UpdateReviveTimer(Mathf.CeilToInt(time));
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
            if (mode == GameMode.Normal)
            {
                if (Alive == 1)
                {
                    EndGameWin();
                }
            }
            else if (mode == GameMode.Zombie)
            {
                if (Alive == 0)
                {
                    EndGameWin();
                }
            }
        }
    }
    
    // --- Lưu/Load dữ liệu ---
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

    // --- Quản lý trạng thái game ---
    public void StartGame()
    {
        if (State == GameState.Playing) return;
        SetState(GameState.Playing);
        
        player.SetPlayerAttackRange(true);

        //if (player) SetPlayerControl(true);
    }
    public void PauseGame()
    {
        SetState(GameState.Paused);
        uiController.DisplayPauseGamePanel(true);
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    { 
        SetState(GameState.Playing);
        uiController.DisplayPauseGamePanel(false);
        Time.timeScale = 1f;
    }
    public void EndGameWin()
    {
        if (State == GameState.Win || State == GameState.Lose) return;
        SetState(GameState.Win);

        if (mode == GameMode.Normal)
        {
            data.SetCurrentLevel(data.GetCurrentLevel() + 1);

            if (data.GetCurrentLevel() >= 2)
            {
                data.SetCurrentLevel(0);
            }
        }

        if (player)
        {
            //SetPlayerControl(false);
            player.SetPlayerAttackRange(false);
            player.WinDance();
        }
        
        SaveData();
    }
    public void EndGameLose()
    {
        if (State == GameState.Win || State == GameState.Lose) return;

        if (player.GetContext().Lives.GetLives() <= 1)
        {
            SetState(GameState.WaitForRevive);
            time = 5f;
        }
        else
        {
            player.KillPlayer();
        }
        
        //if (player) SetPlayerControl(false);
    }
    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // --- Xử lý sự kiện ---
    void OnEnemyDeadEvent(EnemyBase enemy, int coin)
    {
        // Chỉ tăng buffer, không xử lý logic tại đây để tránh race
        deathBuffer++;
        coinCollected += coin;
        if (mode == GameMode.Zombie)
        {
            player.points += coin;
            player.ZombieUpgrade();
        }
    }

    void OnEnemySpawned()
    {
        // Spawner gọi khi đã Instantiate xong
        pendingSpawnRequests = Mathf.Max(0, pendingSpawnRequests - 1);
    }

    public void SetState(GameState s)
    {
        State = s;
        EventObserver.RaiseGameStateChanged(State);
    }
    
    // Geter Setter
    public Data GetData()
    {
        return data;
    }
    public PlayerController GetPlayer()
    {
        return player;
    }
    public UIController GetUIController()
    {
        return uiController;
    }
    public ListWeapon GetListWeapon()
    {
        return listWeapon;
    }
}
