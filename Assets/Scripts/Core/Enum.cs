// Enum.cs
public static class AnimatorParams
{
    public const string IsIdle = "IsIdle";
    public const string IsDead = "IsDead";
    public const string IsAttack = "IsAttack";
    public const string IsWin = "IsWin";
    public const string IsDance = "IsDance";
    public const string IsUlti = "IsUlti";
}

public enum PlayerState
{
    Idle,
    Run,
    Attack,
    Die,
    Dance,
    Win
}