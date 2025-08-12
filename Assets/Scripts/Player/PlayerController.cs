using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AnimationController animationController;
    [SerializeField] private JoystickController joystick; // Kéo JoystickBG vào đây
    [SerializeField] private WeaponAttack weaponAttack; // Kéo WeaponAttack vào đây nếu cần
    [SerializeField] private Collider playerCollider;
    [SerializeField] private GameObject attackRangeCircle;
    [SerializeField] TextMeshProUGUI pointsText; // Hiển thị điểm của người chơi
    
    [Header("Variables")]
    [SerializeField] private float moveSpeed = 5f;
    public int points = 0; // Điểm của người chơi

    private Rigidbody rigid;

    private Vector2 dir;
    private Vector3 move;

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
    }

    private void OnDisable()
    {
        EventObserver.OnUpgrade -= Upgrade;
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

    public void Die()
    {
        rigid.isKinematic = true;
        playerCollider.enabled = false;
        weaponAttack.SetCanAttack(false);
        animationController.SetDeadAnimation();
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
        
        transform.localScale += Vector3.one * Values.upgradeScale; 
        weaponAttack.upgradeAttackRadius(Values.upgradeRadius);
    }

    public WeaponAttack GetWeaponAttack()
    {
        return this.weaponAttack;
    }
}