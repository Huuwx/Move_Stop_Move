using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public Sprite icon;
    public GameObject modelPrefab; // prefab model (nếu khác nhau)
    public GameObject visual;
    public float speed;
    public bool isPurchased = false; // Đã mua hay chưa
    public bool isEquipped = false; // Đã trang bị hay chưa
    public int price; // Giá của vũ khí
    public string name;
    public string description;
}