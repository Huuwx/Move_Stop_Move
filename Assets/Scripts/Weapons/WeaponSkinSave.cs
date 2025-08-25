using UnityEngine;

public static class WeaponSkinSave
{
    static string KeySkin(string weaponId)         => $"weapon_{weaponId}_skin";
    static string KeyCount(string weaponId)        => $"weapon_{weaponId}_custom_count";
    static string KeySlot(string weaponId, int i)  => $"weapon_{weaponId}_custom_{i}";

    public static void SaveSelected(string weaponId, string skinId)
    {
        PlayerPrefs.SetString(KeySkin(weaponId), skinId);
        PlayerPrefs.Save();
    }

    public static string LoadSelected(string weaponId, string fallback = "default")
    {
        return PlayerPrefs.GetString(KeySkin(weaponId), fallback);
    }

    public static void SaveCustom(string weaponId, Color[] colors)
    {
        if (colors == null) return;
        PlayerPrefs.SetInt(KeyCount(weaponId), colors.Length);
        for (int i = 0; i < colors.Length; i++)
            PlayerPrefs.SetString(KeySlot(weaponId, i), ColorUtility.ToHtmlStringRGBA(colors[i]));
        PlayerPrefs.Save();
    }

    public static Color[] LoadCustom(string weaponId, int expectedSlots)
    {
        int n = PlayerPrefs.GetInt(KeyCount(weaponId), expectedSlots);
        var arr = new Color[n];
        for (int i = 0; i < n; i++)
        {
            string hex = PlayerPrefs.GetString(KeySlot(weaponId, i), "FFFFFFFF");
            if (!ColorUtility.TryParseHtmlString("#" + hex, out arr[i])) arr[i] = Color.white;
        }
        return arr;
    }
}