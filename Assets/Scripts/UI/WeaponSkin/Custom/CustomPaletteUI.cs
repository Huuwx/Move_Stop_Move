using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomPaletteUI : MonoBehaviour
{
    [Header("Build")]
    [SerializeField] private RectTransform[] gridRoot;  // Content của Grid Layout
    [SerializeField] private Button swatchPrefab;     // Prefab Button (có Image)

    [Header("Colors")]
    public Color[] colors = new Color[] {
        Color.white, Color.black, Color.red, Color.green, Color.blue,
        new Color(1f,0.5f,0f), new Color(1f,0.2f,0.6f), new Color(0.3f,0.8f,1f),
        new Color(1f,1f,0.2f), new Color(0.6f,0.4f,1f)
    };

    [System.Serializable] public class ColorEvent : UnityEvent<Color> {}
    public ColorEvent onPick; // Gọi ra màu khi người dùng bấm

    void OnEnable()
    {
        // Rebuild mỗi lần mở panel (đơn giản)
        Build();
    }

    public void Build()
    {
        if (gridRoot.Length == 0 || !swatchPrefab) return;

        foreach (Transform t in gridRoot[0]) Destroy(t.gameObject);
        foreach (Transform t in gridRoot[1]) Destroy(t.gameObject);

        int count = 0, index = 0;
        foreach (var c in colors)
        {
            if (count >= 6)
            {
                count = 0;
                index++;
            }
            var b = Instantiate(swatchPrefab, gridRoot[index]);
            var img = b.GetComponent<Image>();
            if (img) img.color = c;
            b.onClick.AddListener(() => onPick?.Invoke(c));
            count++;
        }
    }
}