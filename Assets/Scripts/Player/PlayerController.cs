using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private AnimationController animationController;
    [SerializeField] private JoystickController joystick; // Kéo JoystickBG vào đây
    [SerializeField] private WeaponAttack weaponAttack; // Kéo WeaponAttack vào đây nếu cần
    [SerializeField] private Collider playerCollider;
    [SerializeField] private GameObject attackRangeCircle;
    
    [Header("Variables")]
    [SerializeField] private float moveSpeed = 5f;

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

    public WeaponAttack GetWeaponAttack()
    {
        return this.weaponAttack;
    }
}