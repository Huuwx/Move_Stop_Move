using System;
using UnityEngine;
using UnityEngine.UI;

public class SlotItemUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image colorPreview;
    [SerializeField] private GameObject selectedFrame;

    public int Index { get; private set; }

    public void Bind(int index, Color startColor, Action<int> onClick)
    {
        Index = index;
        if (colorPreview) colorPreview.color = startColor;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(Index));
    }

    public void SetSelected(bool v) { if (selectedFrame) selectedFrame.SetActive(v); }
    public void SetColor(Color c)   { if (colorPreview) colorPreview.color = c; }
}