using System.Collections.Generic;
using UnityEngine;

public static class WeaponSkinSave
{
    static string KSel(string wid) => $"skin_{wid}";
    static string KCol(string wid, int i) => $"skin_{wid}_slot{i}";

    public static void SaveSelected(string weaponId, string skinId)
    {
        PlayerPrefs.SetString(KSel(weaponId), skinId);
        PlayerPrefs.Save();
    }
    public static string LoadSelected(string weaponId, string fallback)
        => PlayerPrefs.GetString(KSel(weaponId), fallback);

    public static void SaveCustom(string weaponId, IList<Color> colors)
    {
        for (int i = 0; i < colors.Count; i++) SaveColor(KCol(weaponId, i), colors[i]);
        PlayerPrefs.SetInt($"skin_{weaponId}_count", colors.Count);
        PlayerPrefs.Save();
    }
    public static List<Color> LoadCustom(string weaponId, int expectedCount, Color fallback)
    {
        int count = PlayerPrefs.GetInt($"skin_{weaponId}_count", expectedCount);
        var list = new List<Color>(count);
        for (int i = 0; i < count; i++) list.Add(LoadColor(KCol(weaponId, i), fallback));
        return list;
    }

    static void SaveColor(string k, Color c)
    { PlayerPrefs.SetFloat(k+"r",c.r); PlayerPrefs.SetFloat(k+"g",c.g); PlayerPrefs.SetFloat(k+"b",c.b); PlayerPrefs.SetFloat(k+"a",c.a); }
    static Color LoadColor(string k, Color fb)
    {
        if (!PlayerPrefs.HasKey(k+"r")) return fb;
        return new Color(PlayerPrefs.GetFloat(k+"r"), PlayerPrefs.GetFloat(k+"g"), PlayerPrefs.GetFloat(k+"b"), PlayerPrefs.GetFloat(k+"a",1));
    }
}