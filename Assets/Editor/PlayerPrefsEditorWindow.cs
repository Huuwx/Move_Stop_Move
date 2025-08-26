// #if UNITY_EDITOR
// using System;
// using System.Collections.Generic;
// using System.Globalization;
// using System.IO;
// using System.Linq;
// using System.Diagnostics;
// using System.Text;
// using UnityEditor;
// using UnityEngine;
//
// public class PlayerPrefsEditorWindow : EditorWindow
// {
//     private enum PrefType { Int, Float, String, Unknown }
//
//     [Serializable]
//     private class PrefEntry
//     {
//         public string key;
//         public PrefType type;
//         public string stringValue;   // lưu chuỗi hoặc số tạm thời (để sửa inline)
//         public bool dirty;           // đã sửa, cần Save
//     }
//
//     private List<PrefEntry> entries = new List<PrefEntry>();
//     private Vector2 scroll;
//     private string search = "";
//     private bool deepScan = false;       // Thử nghiệm: quét Registry (Win) / plist (macOS)
//     private GUIStyle headerStyle;
//     private string company;
//     private string product;
//
//     [MenuItem("Tools/PlayerPrefs Editor")]
//     public static void Open()
//     {
//         var win = GetWindow<PlayerPrefsEditorWindow>("PlayerPrefs Editor");
//         win.minSize = new Vector2(720, 420);
//         win.Show();
//     }
//
//     private void OnEnable()
//     {
//         company = PlayerSettings.companyName;
//         product = PlayerSettings.productName;
//         headerStyle = new GUIStyle(EditorStyles.boldLabel)
//         {
//             fontSize = 13
//         };
//         Refresh();
//     }
//
//     private void OnGUI()
//     {
//         EditorGUILayout.Space();
//         EditorGUILayout.LabelField("PlayerPrefs Editor", headerStyle);
//         EditorGUILayout.LabelField($"Company: {company}   |   Product: {product}");
//
//         EditorGUILayout.Space(4);
//         using (new EditorGUILayout.HorizontalScope())
//         {
//             if (GUILayout.Button("Refresh", GUILayout.Width(100)))
//                 Refresh();
//
//             if (GUILayout.Button("Save (apply changes)", GUILayout.Width(160)))
//                 SaveDirty();
//
//             if (GUILayout.Button("Add New", GUILayout.Width(100)))
//                 AddNew();
//
//             if (GUILayout.Button("Delete All", GUILayout.Width(100)))
//             {
//                 if (EditorUtility.DisplayDialog("Delete All PlayerPrefs",
//                     "Xoá TẤT CẢ PlayerPrefs của project hiện tại?\nHành động không thể hoàn tác.", "Yes", "No"))
//                 {
//                     PlayerPrefs.DeleteAll();
//                     PlayerPrefs.Save();
//                     Refresh();
//                 }
//             }
//
//             GUILayout.FlexibleSpace();
//
//             deepScan = EditorGUILayout.ToggleLeft(new GUIContent("Deep Scan (Experimental)",
//                 "Thử quét trực tiếp vùng lưu trữ hệ thống để liệt kê key.\n" +
//                 "- Windows: Registry\n- macOS: .plist\nLinux: fallback Safe mode"),
//                 deepScan, GUILayout.Width(200));
//
//             if (GUILayout.Button("Open Storage Location", GUILayout.Width(180)))
//                 OpenStorageLocation();
//         }
//
//         EditorGUILayout.Space(8);
//         using (new EditorGUILayout.HorizontalScope())
//         {
//             GUILayout.Label("Search:", GUILayout.Width(60));
//             string newSearch = EditorGUILayout.TextField(search);
//             if (newSearch != search)
//             {
//                 search = newSearch;
//             }
//         }
//
//         EditorGUILayout.Space(8);
//         DrawTable();
//     }
//
//     private void DrawTable()
//     {
//         // Header
//         using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
//         {
//             GUILayout.Label("Key", GUILayout.Width(260));
//             GUILayout.Label("Type", GUILayout.Width(80));
//             GUILayout.Label("Value (editable)", GUILayout.ExpandWidth(true));
//             GUILayout.Label("", GUILayout.Width(120)); // buttons
//         }
//
//         // Rows
//         scroll = EditorGUILayout.BeginScrollView(scroll);
//         var filtered = string.IsNullOrEmpty(search)
//             ? entries
//             : entries.Where(e => e.key.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
//
//         foreach (var e in filtered)
//         {
//             using (new EditorGUILayout.HorizontalScope())
//             {
//                 EditorGUI.BeginDisabledGroup(e.type == PrefType.Unknown);
//                 string newKey = EditorGUILayout.TextField(e.key, GUILayout.Width(260));
//                 if (newKey != e.key)
//                 {
//                     e.key = newKey;
//                     e.dirty = true;
//                 }
//
//                 var newType = (PrefType)EditorGUILayout.EnumPopup(e.type, GUILayout.Width(80));
//                 if (newType != e.type)
//                 {
//                     e.type = newType;
//                     e.dirty = true;
//                 }
//
//                 string newVal = EditorGUILayout.TextField(e.stringValue);
//                 if (newVal != e.stringValue)
//                 {
//                     e.stringValue = newVal;
//                     e.dirty = true;
//                 }
//                 EditorGUI.EndDisabledGroup();
//
//                 using (new EditorGUILayout.HorizontalScope(GUILayout.Width(120)))
//                 {
//                     if (GUILayout.Button("Save", GUILayout.Width(55)))
//                         SaveEntry(e);
//
//                     if (GUILayout.Button("Delete", GUILayout.Width(60)))
//                     {
//                         if (EditorUtility.DisplayDialog("Delete Key",
//                             $"Xoá key '{e.key}' ?", "Yes", "No"))
//                         {
//                             PlayerPrefs.DeleteKey(e.key);
//                             PlayerPrefs.Save();
//                             entries.Remove(e);
//                             Repaint();
//                             break;
//                         }
//                     }
//                 }
//             }
//         }
//         EditorGUILayout.EndScrollView();
//     }
//
//     private void AddNew()
//     {
//         entries.Add(new PrefEntry
//         {
//             key = "NewKey",
//             type = PrefType.String,
//             stringValue = "",
//             dirty = true
//         });
//     }
//
//     private void SaveDirty()
//     {
//         foreach (var e in entries.Where(x => x.dirty))
//         {
//             SaveEntry(e);
//         }
//     }
//
//     private void SaveEntry(PrefEntry e)
//     {
//         try
//         {
//             switch (e.type)
//             {
//                 case PrefType.Int:
//                     if (!int.TryParse(e.stringValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int iv))
//                         throw new Exception("Giá trị không phải số nguyên hợp lệ.");
//                     PlayerPrefs.SetInt(e.key, iv);
//                     break;
//
//                 case PrefType.Float:
//                     if (!float.TryParse(e.stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float fv))
//                         throw new Exception("Giá trị không phải số thực hợp lệ.");
//                     PlayerPrefs.SetFloat(e.key, fv);
//                     break;
//
//                 case PrefType.String:
//                     PlayerPrefs.SetString(e.key, e.stringValue ?? string.Empty);
//                     break;
//
//                 default:
//                     EditorUtility.DisplayDialog("Unknown type",
//                         "Type Unknown không thể lưu. Hãy chọn Int/Float/String.", "OK");
//                     return;
//             }
//
//             PlayerPrefs.Save();
//             e.dirty = false;
//             Repaint();
//         }
//         catch (Exception ex)
//         {
//             EditorUtility.DisplayDialog("Save error", ex.Message, "OK");
//         }
//     }
//
//     private void Refresh()
//     {
//         entries.Clear();
//
//         if (deepScan)
//         {
// #if UNITY_EDITOR_WIN
//             TryLoadFromWindowsRegistry();
// #elif UNITY_EDITOR_OSX
//             TryLoadFromMacPlist();
// #else
//             // Linux / Others: không có scan đáng tin → fallback Safe mode
//             LoadKnownFromAPI();
// #endif
//         }
//         else
//         {
//             LoadKnownFromAPI();
//         }
//
//         // Loại trùng key (nếu có), ưu tiên phần tử sau (vừa load)
//         entries = entries
//             .GroupBy(e => e.key)
//             .Select(g => g.Last())
//             .OrderBy(e => e.key, StringComparer.OrdinalIgnoreCase)
//             .ToList();
//
//         Repaint();
//     }
//
//     /// <summary>
//     /// Safe mode: Chỉ đọc các key "thường dùng" từ PlayerPrefs API.
//     /// Bạn có thể thêm các key gợi ý tại đây để tiện load nhanh.
//     /// </summary>
//     private void LoadKnownFromAPI()
//     {
//         // Gợi ý: thêm các key bạn hay dùng để hiện sẵn
//         string[] commonKeys = new[]
//         {
//             "HighScore",
//             "PlayerName",
//             "MusicVolume",
//             "SfxVolume",
//             "HasTutorialDone",
//             "SelectedWeapon",
//             "Coins",
//             "Gems"
//         };
//
//         foreach (var k in commonKeys)
//         {
//             // Vì không biết trước type chính xác, thử theo thứ tự: Int -> Float -> String
//             if (PlayerPrefs.HasKey(k))
//             {
//                 // Heuristic: nếu GetString() trả về khác default thì coi là String
//                 string s = PlayerPrefs.GetString(k, "__PP_EMPTY__");
//                 if (s != "__PP_EMPTY__")
//                 {
//                     entries.Add(new PrefEntry
//                     {
//                         key = k,
//                         type = PrefType.String,
//                         stringValue = s
//                     });
//                     continue;
//                 }
//
//                 // Thử int
//                 int i = PlayerPrefs.GetInt(k, int.MinValue);
//                 if (i != int.MinValue)
//                 {
//                     entries.Add(new PrefEntry
//                     {
//                         key = k,
//                         type = PrefType.Int,
//                         stringValue = i.ToString(CultureInfo.InvariantCulture)
//                     });
//                     continue;
//                 }
//
//                 // Thử float
//                 float f = PlayerPrefs.GetFloat(k, float.NaN);
//                 if (!float.IsNaN(f))
//                 {
//                     entries.Add(new PrefEntry
//                     {
//                         key = k,
//                         type = PrefType.Float,
//                         stringValue = f.ToString(CultureInfo.InvariantCulture)
//                     });
//                     continue;
//                 }
//
//                 // Không đoán được
//                 entries.Add(new PrefEntry
//                 {
//                     key = k,
//                     type = PrefType.Unknown,
//                     stringValue = ""
//                 });
//             }
//         }
//     }
//
// #if UNITY_EDITOR_WIN
//     private void TryLoadFromWindowsRegistry()
//     {
//         // Unity thường lưu ở:
//         // - Build: HKCU\Software\[Company]\[Product]
//         // - Editor: HKCU\Software\Unity\UnityEditor\[Company]\[Product]
//         var paths = new[]
//         {
//             $@"Software\{company}\{product}",
//             $@"Software\Unity\UnityEditor\{company}\{product}"
//         };
//
//         try
//         {
//             using (var baseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(
//                 Microsoft.Win32.RegistryHive.CurrentUser, Microsoft.Win32.RegistryView.Default))
//             {
//                 foreach (var relPath in paths)
//                 {
//                     using (var sub = baseKey.OpenSubKey(relPath, false))
//                     {
//                         if (sub == null) continue;
//                         foreach (var name in sub.GetValueNames())
//                         {
//                             var kind = sub.GetValueKind(name);
//                             var val = sub.GetValue(name);
//
//                             // Heuristic đoán type:
//                             PrefType type = PrefType.String;
//                             string str = "";
//
//                             if (kind == Microsoft.Win32.RegistryValueKind.DWord ||
//                                 kind == Microsoft.Win32.RegistryValueKind.QWord)
//                             {
//                                 type = PrefType.Int;
//                                 str = Convert.ToString(val, CultureInfo.InvariantCulture);
//                             }
//                             else if (kind == Microsoft.Win32.RegistryValueKind.String ||
//                                      kind == Microsoft.Win32.RegistryValueKind.ExpandString)
//                             {
//                                 // Có thể là string hoặc float lưu dạng chuỗi
//                                 str = val?.ToString() ?? "";
//                                 if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
//                                 {
//                                     // Cho phép người dùng chỉnh type nếu muốn
//                                     type = PrefType.String; // Mặc định coi là String để an toàn
//                                 }
//                                 else
//                                 {
//                                     type = PrefType.String;
//                                 }
//                             }
//                             else if (kind == Microsoft.Win32.RegistryValueKind.Binary)
//                             {
//                                 // Có trường hợp float được lưu binary – khó xác định chuẩn
//                                 // Ta hiển thị Hex để người dùng biết có dữ liệu
//                                 byte[] bytes = val as byte[];
//                                 str = bytes != null ? BitConverter.ToString(bytes) : "";
//                                 type = PrefType.Unknown;
//                             }
//                             else
//                             {
//                                 type = PrefType.Unknown;
//                                 str = val != null ? val.ToString() : "";
//                             }
//
//                             // Loại bỏ các key "ẩn" mà Unity dùng nội bộ (kết thúc bằng _hxxxx)
//                             if (name.EndsWith("_h", StringComparison.OrdinalIgnoreCase) ||
//                                 name.Contains("_h"))
//                             {
//                                 continue;
//                             }
//
//                             entries.Add(new PrefEntry
//                             {
//                                 key = name,
//                                 type = type,
//                                 stringValue = str
//                             });
//                         }
//                     }
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             //Debug.LogWarning("Registry scan failed: " + ex.Message);
//             LoadKnownFromAPI();
//         }
//     }
// #endif
//
// #if UNITY_EDITOR_OSX
//     private void TryLoadFromMacPlist()
//     {
//         // Đường dẫn thường gặp: ~/Library/Preferences/unity.[Company].[Product].plist
//         string plistPath = Path.Combine(
//             Environment.GetFolderPath(Environment.SpecialFolder.Personal),
//             "Library/Preferences",
//             $"unity.{company}.{product}.plist");
//
//         if (!File.Exists(plistPath))
//         {
//             // Nếu không có, fallback
//             LoadKnownFromAPI();
//             return;
//         }
//
//         try
//         {
//             // Dùng lệnh 'defaults read' để xuất plist thành text dễ parse
//             string output = RunProcess("/usr/bin/defaults", $"read {EscapeArg($"unity.{company}.{product}")}");
//             // Parse thô: mỗi dòng có dạng "key = value;"
//             // Đây chỉ là heuristic đơn giản, không xử lý mọi case phức tạp (array/dict).
//             using (var reader = new StringReader(output))
//             {
//                 string line;
//                 while ((line = reader.ReadLine()) != null)
//                 {
//                     line = line.Trim();
//                     if (string.IsNullOrEmpty(line) || line.StartsWith("{") || line.StartsWith("}"))
//                         continue;
//
//                     int eq = line.IndexOf('=');
//                     if (eq <= 0) continue;
//
//                     string key = line.Substring(0, eq).Trim();
//                     string valuePart = line.Substring(eq + 1).Trim().TrimEnd(';').Trim();
//
//                     // Loại bỏ quote nếu có
//                     if (valuePart.StartsWith("\"") && valuePart.EndsWith("\""))
//                     {
//                         valuePart = valuePart.Substring(1, valuePart.Length - 2);
//                     }
//
//                     PrefType type = PrefType.String;
//                     if (int.TryParse(valuePart, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
//                         type = PrefType.Int;
//                     else if (float.TryParse(valuePart, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
//                         type = PrefType.Float;
//
//                     if (!string.IsNullOrEmpty(key))
//                     {
//                         entries.Add(new PrefEntry
//                         {
//                             key = key,
//                             type = type,
//                             stringValue = valuePart
//                         });
//                     }
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.LogWarning("plist scan failed: " + ex.Message);
//             LoadKnownFromAPI();
//         }
//     }
//
//     private static string RunProcess(string file, string args)
//     {
//         var psi = new ProcessStartInfo
//         {
//             FileName = file,
//             Arguments = args,
//             UseShellExecute = false,
//             RedirectStandardOutput = true,
//             RedirectStandardError = true
//         };
//         using var p = Process.Start(psi);
//         string std = p.StandardOutput.ReadToEnd();
//         string err = p.StandardError.ReadToEnd();
//         p.WaitForExit();
//         if (p.ExitCode != 0) throw new Exception($"Command failed: {err}");
//         return std;
//     }
//
//     private static string EscapeArg(string s)
//     {
//         return "\"" + s.Replace("\"", "\\\"") + "\"";
//     }
// #endif
//
//     private void OpenStorageLocation()
//     {
// #if UNITY_EDITOR_WIN
//         // Mở Regedit đến nhánh Editor (ưu tiên)
//         string editorPath = $@"HKEY_CURRENT_USER\Software\Unity\UnityEditor\{company}\{product}";
//         EditorUtility.DisplayDialog("Windows Registry",
//             "Unity lưu PlayerPrefs trong Registry.\n" +
//             "Đường dẫn (Editor):\n" + editorPath + "\n\n" +
//             "Bạn có thể mở Regedit và điều hướng đến nhánh trên.", "OK");
//         // Không auto-jump được bằng code an toàn; người dùng mở Run → regedit.
// #elif UNITY_EDITOR_OSX
//         string path = Path.Combine(
//             Environment.GetFolderPath(Environment.SpecialFolder.Personal),
//             "Library/Preferences",
//             $"unity.{company}.{product}.plist");
//         if (File.Exists(path))
//         {
//             EditorUtility.RevealInFinder(path);
//         }
//         else
//         {
//             EditorUtility.DisplayDialog("Not found",
//                 "Không tìm thấy file plist.\nTool vẫn có thể chỉnh PlayerPrefs qua API.", "OK");
//         }
// #else
//         // Linux
//         string path = Path.Combine(
//             Environment.GetFolderPath(Environment.SpecialFolder.Personal),
//             $".config/unity3d/{company}/{product}/prefs");
//         if (File.Exists(path))
//         {
//             EditorUtility.RevealInFinder(path);
//         }
//         else
//         {
//             EditorUtility.DisplayDialog("Not found",
//                 "Không tìm thấy file prefs.\nTool vẫn có thể chỉnh PlayerPrefs qua API.", "OK");
//         }
// #endif
//     }
// }
// #endif
