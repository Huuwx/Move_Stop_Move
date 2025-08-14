// CategoryToggle.cs (gắn lên mỗi Toggle)
using UnityEngine;
using UnityEngine.UI;

public class CategoryBtn : MonoBehaviour
{
    public WardrobeUI wardrobeUI;
    public OutfitCategory category;
    private Button btn;
    
    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnChanged);
    }
    
    public void OnChanged()
    {
        //btn.interactable = false; // Vô hiệu hóa nút để tránh nhấn liên tục
        wardrobeUI.ShowCategory(category);
    }
}