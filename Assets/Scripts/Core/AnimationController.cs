using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    
    [SerializeField] GameObject player;
    [SerializeField] private ParticleSystem hittedEffect;            // Hiệu ứng attack (nếu có)

    public event Action OnAttack;
    
    public bool IsPlayingSpecialAnimation { get; private set; } = false;
    public bool IsPlayingUnStopAnimation { get; private set; } = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        IsPlayingSpecialAnimation = false;
        IsPlayingUnStopAnimation = false;
    }

    public void SetRunAnimation()
    {
        animator.SetBool(Params.IsIdle, false);
        animator.SetBool(Params.IsDead, false);
        animator.SetBool(Params.IsAttack, false);
        animator.SetBool(Params.IsWin, false);
    }
    
    public void SetIdleAnimation()
    {
        animator.SetBool(Params.IsIdle, true);
        animator.SetBool(Params.IsDead, false);
        animator.SetBool(Params.IsAttack, false);
        animator.SetBool(Params.IsWin, false);
        animator.SetBool(Params.IsDance, false);
    }
    
    public void SetAttackAnimation()
    {
        IsPlayingSpecialAnimation = true;
        animator.SetBool(Params.IsUlti, false);
        animator.SetBool(Params.IsDead, false);
        animator.SetBool(Params.IsAttack, true);
        animator.SetBool(Params.IsWin, false);
    }
    
    public void SetUltiAnimation()
    {
        IsPlayingUnStopAnimation = true;
        animator.SetBool(Params.IsUlti, true);
        animator.SetBool(Params.IsDead, false);
        animator.SetBool(Params.IsAttack, true);
        animator.SetBool(Params.IsWin, false);
    }

    public void SetDanceWinAnimation()
    {
        IsPlayingUnStopAnimation = true;
        animator.SetBool(Params.IsWin, true);
        animator.SetBool(Params.IsDead, false);
    }
    
    public void SetDeadAnimation()
    {
        hittedEffect.Play();
        IsPlayingUnStopAnimation = true;
        animator.SetBool(Params.IsDead, true);
    }
    
    public void SetDanceAnimation()
    {
        IsPlayingSpecialAnimation = true;
        animator.SetBool(Params.IsDance, true);
    }
    
    public void OnSpecialAnimationEnd()
    {
        OnAttack = null;
        IsPlayingSpecialAnimation = false;
    }
    
    public void OnUnStopAnimationEnd()
    {
        IsPlayingUnStopAnimation = false;
    }

    public void Reset()
    {
        OnAttack = null;
        IsPlayingUnStopAnimation = false;
        IsPlayingSpecialAnimation = false;
    }
    
    public void Attack()
    {
        OnAttack?.Invoke();
    }

    public void SetActionAfterDead()
    {
        IsPlayingUnStopAnimation = false;
        if(player.CompareTag(Params.PlayerTag))
        {
            GameController.Instance.EndGameLose();
        }
        else if(player.CompareTag(Params.BotTag))
        {
            EnemyAI enemyAI = player.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TriggerDeadEvent();
            }
            player.SetActive(false);
        }
    }
}
