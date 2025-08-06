using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;

    public event Action onAttack;
    
    public bool IsPlayingSpecialAnimation { get; private set; } = false;
    public bool IsPlayingUnStopAnimation { get; private set; } = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetRunAnimation()
    {
        animator.SetBool(AnimatorParams.IsIdle, false);
        animator.SetBool(AnimatorParams.IsDead, false);
        animator.SetBool(AnimatorParams.IsAttack, false);
        animator.SetBool(AnimatorParams.IsWin, false);
    }
    
    public void SetIdleAnimation()
    {
        animator.SetBool(AnimatorParams.IsIdle, true);
        animator.SetBool(AnimatorParams.IsDead, false);
        animator.SetBool(AnimatorParams.IsAttack, false);
        animator.SetBool(AnimatorParams.IsWin, false);
        animator.SetBool(AnimatorParams.IsDance, false);
    }
    
    public void SetAttackAnimation()
    {
        animator.SetBool(AnimatorParams.IsUlti, false);
        animator.SetBool(AnimatorParams.IsDead, false);
        animator.SetBool(AnimatorParams.IsAttack, true);
        animator.SetBool(AnimatorParams.IsWin, false);
    }
    
    public void SetUltiAnimation()
    {
        IsPlayingUnStopAnimation = true;
        animator.SetBool(AnimatorParams.IsUlti, true);
        animator.SetBool(AnimatorParams.IsDead, false);
        animator.SetBool(AnimatorParams.IsAttack, true);
        animator.SetBool(AnimatorParams.IsWin, false);
    }

    public void SetDanceWinAnimation()
    {
        IsPlayingUnStopAnimation = true;
        animator.SetBool(AnimatorParams.IsWin, true);
        animator.SetBool(AnimatorParams.IsDead, false);
    }
    
    public void SetDeadAnimation()
    {
        IsPlayingUnStopAnimation = true;
        animator.SetBool(AnimatorParams.IsDead, true);
    }
    
    public void SetDanceAnimation()
    {
        IsPlayingSpecialAnimation = true;
        animator.SetBool(AnimatorParams.IsDance, true);
    }
    
    public void OnSpecialAnimationEnd()
    {
        IsPlayingSpecialAnimation = false;
    }
    
    public void OnUnStopAnimationEnd()
    {
        IsPlayingUnStopAnimation = false;
    }
    
    public void OnAttack()
    {
        onAttack?.Invoke();
    }
}
