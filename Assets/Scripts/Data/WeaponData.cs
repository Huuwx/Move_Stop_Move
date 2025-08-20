using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public string id;
    public int index; // chỉ số của vũ khí trong danh sách
    
    [Header("Prefabs")]
    public GameObject modelPrefab; // prefab model (nếu khác nhau)
    public GameObject visual;

    [Header("Attributes")] 
    public float speed;
    public bool isPurchased = false; // Đã mua hay chưa
    public bool isEquipped = false; // Đã trang bị hay chưa
    public int price; // Giá của vũ khí
    public string name;
    public string description;
    public bool isRotate = false;
}