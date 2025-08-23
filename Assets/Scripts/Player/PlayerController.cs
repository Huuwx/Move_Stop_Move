using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZombieCity.Abilities;

public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AnimationController animationController;
    [SerializeField] private JoystickController joystick; // Kéo JoystickBG vào đây
    [SerializeField] private WeaponAttack weaponAttack; // Kéo WeaponAttack vào đây nếu cần
    [SerializeField] private Collider playerCollider;
    [SerializeField] private GameObject attackRangeCircle;
    [SerializeField] TextMeshProUGUI pointsText; // Hiển thị điểm của người chơi
    [SerializeField] GameObject ingameUI; // Giao diện trong game
    [SerializeField] private List<GameObject> RevivePoints; // Danh sách các điểm hồi sinh
    
    [Header("Variables")]
    [SerializeField] private float moveSpeed = 5f;
    public int points = 0; // Điểm của người chơi
    
    public static event Action OnTriggerUltimate;
    public static event Action OnUltimateEnd;
    public static event Action OnPlayerDeath;
    public static event Action OnPlayerRevived;
    
    private PlayerContext ctx;
    private Rigidbody rigid;
    private Vector2 dir;
    private Vector3 move;

    private void Awake()
    {
        ctx = new PlayerContext(gameObject);
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        if(animationController == null)
            animationController = GetComponentInChildren<AnimationController>();
        if(playerCollider == null)
            playerCollider = GetComponent<Collider>();
        
        playerCollider.enabled = true; // Bật collider khi khởi tạo
        rigid.isKinematic = false;
    }

    private void OnEnable()
    {
        EventObserver.OnUpgrade += Upgrade;
        EventObserver.OnGameStateChanged += setIngameUIActive;
        OnUltimateEnd += EndUltimate;
        OnPlayerDeath += Die; // Đăng ký sự kiện khi người chơi chết
        OnPlayerRevived += Revive; // Đăng ký sự kiện khi người chơi hồi sinh
    }

    private void OnDisable()
    {
        EventObserver.OnUpgrade -= Upgrade;
        EventObserver.OnGameStateChanged -= setIngameUIActive;
        OnUltimateEnd -= EndUltimate;
        OnPlayerDeath -= Die; // Hủy đăng ký sự kiện khi người chơi chết
        OnPlayerRevived -= Revive; // Hủy đăng ký sự kiện khi người chơi hồi sinh
    }

    void Update()
    {
        if (GameController.Instance.State != GameState.Playing)
            return; // Không xử lý di chuyển nếu không phải trạng thái Playing
        
        dir = joystick.inputDir; // Lấy hướng từ joystick
        move = new Vector3(dir.x, 0, dir.y); // Di chuyển XZ
        
        if(move != Vector3.zero)
            if(!animationController.IsPlayingUnStopAnimation)
                animationController.OnSpecialAnimationEnd();
        
        if (!animationController.IsPlayingSpecialAnimation && !animationController.IsPlayingUnStopAnimation)
        {
            transform.Translate(move.normalized * moveSpeed * Time.deltaTime, Space.World);

            // Nếu muốn xoay mặt player về hướng di chuyển:
            if (move != Vector3.zero)
            {
                transform.forward = move.normalized;
                animationController.SetRunAnimation();
                weaponAttack.SetCanAttack(false);
            }
            else
            {
                animationController.SetIdleAnimation();
                weaponAttack.SetCanAttack(true);
            }
        }
    }

    public void Revive()
    {
        int index = UnityEngine.Random.Range(0, RevivePoints.Count);
        transform.position = RevivePoints[index].transform.position; // Di chuyển đến điểm hồi sinh
        rigid.isKinematic = false; // Bật vật lý lại
        playerCollider.enabled = true; // Bật collider lại
        animationController.SetIdleAnimation(); // Đặt lại trạng thái hoạt động
        CameraFollow camera = Camera.main.GetComponent<CameraFollow>();
        if (camera != null)
        {
            camera.SetCameraPos(); // Đặt lại vị trí camera
        }
        GameController.Instance.SetState(GameState.Playing);
    }

    public void Die()
    {
        GameController.Instance.SetState(GameState.Wait);
        rigid.isKinematic = true;
        playerCollider.enabled = false;
        weaponAttack.SetCanAttack(false);
        animationController.SetDeadAnimation();
    }

    public void KillPlayer()
    {
        ctx.Lives.Kill();
    }
    
    public void WinDance()
    {
        weaponAttack.SetCanAttack(false);
        animationController.SetDanceWinAnimation();
    }
    
    public void SetPlayerAttackRange(bool isActive)
    {
        if (attackRangeCircle != null)
            attackRangeCircle.SetActive(isActive);
    }
    
    public void Upgrade()
    {
        points += 1; // Tăng điểm mỗi khi người chơi giết được một đối thủ
        
        if (pointsText != null)
        {
            pointsText.text = points.ToString();
        }

        if (GameController.Instance.mode == GameMode.Normal)
        {
            transform.localScale += Vector3.one * Values.upgradeScale;
            weaponAttack.UpgradeAttackRadius(Values.upgradeRadius);
        }
        else
        {
            if (points == 8)
            {
                ctx.Stats.projCount.baseValue += 1; // Tăng số lượng projectile
                GameController.Instance.GetUIController().ShowLevelUpText();
            }
        }
    }
    
    public void Ultimate()
    {
        if (weaponAttack.IsUltimate()) return; // Nếu đã là Ultimate thì không làm gì thêm
        
        weaponAttack.SetUltimate(true);
        
        if (OnTriggerUltimate != null)
            OnTriggerUltimate.Invoke();
        transform.localScale += Vector3.one * Values.upgradeScale * 5;
        weaponAttack.UpgradeAttackRadius(Values.upgradeRadius * 5);
        
    }

    public void EndUltimate()
    {
        if (!weaponAttack.IsUltimate()) return;
        
        weaponAttack.SetUltimate(false);
        
        if (OnUltimateEnd != null)
            OnUltimateEnd.Invoke();
        transform.localScale -= Vector3.one * Values.upgradeScale * 5;
        weaponAttack.UpgradeAttackRadius(-Values.upgradeRadius * 5);
    }
    
    public static void RaiseOnPlayerDeath()
    {
        if (OnPlayerDeath != null)
            OnPlayerDeath.Invoke();
    }
    public static void RaiseOnPlayerRevived()
    {
        if (OnPlayerRevived != null)
            OnPlayerRevived.Invoke();
    }

    public void setIngameUIActive(GameState state)
    {
        if(state == GameState.Playing)
        {
            ingameUI.SetActive(true);
        }
        else
        {
            ingameUI.SetActive(false);
        }
    }

    public WeaponAttack GetWeaponAttack()
    {
        return this.weaponAttack;
    }
    
    public PlayerContext GetContext()
    {
        return ctx;
    }
}