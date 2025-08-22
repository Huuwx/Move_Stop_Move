// Enum.cs

using System.Numerics;

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
    public const string WeaponKey = "Weapon";
}

public static class Values
{
    public static float upgradeScale = 0.1f;
    public static float upgradeRadius = 0.7f;
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
    Home, Shop, Ready, Playing, Paused, WaitForRevive, Wait, Win, Lose
}

public enum GameMode
{
    Normal, Zombie
}

// OutfitCategory.cs
public enum OutfitCategory { Hat, Pants, Shield, OutfitSet, SkinFullBody, Back, Tail}

// AbilitySO.cs
public enum AbilityRarity { Common, Rare, Epic, Legendary }

// StatType.cs
public enum StatType { MaxHP, MoveSpeed, FireRate, Damage, ProjectileSpeed, ProjectileCount, Range, Scale }

