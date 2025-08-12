// Enum.cs
public static class Params
{
    public const string IsIdle = "IsIdle";
    public const string IsDead = "IsDead";
    public const string IsAttack = "IsAttack";
    public const string IsWin = "IsWin";
    public const string IsDance = "IsDance";
    public const string IsUlti = "IsUlti";
    public const string BotTag = "Bot";
    public const string PlayerTag = "Player";
    public const string WallTag = "Wall";
}

public enum EnemyState
{
    Run,     // Di chuyển tự do
    Idle,      // Dừng lại, không có mục tiêu
    Attack,     // Dừng lại và tấn công khi có đối thủ trong vùng attack
    Dead        // Đã bị loại
}

public enum SpawnState
{
    Idle,       // Chưa spawn
    Spawned     // Đã spawn xong
}

public enum GameState
{
    Home, Playing, Paused, Win, Lose
}
