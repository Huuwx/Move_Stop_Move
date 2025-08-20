using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicatorManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Camera cam;                // camera gameplay
    [SerializeField] private RectTransform canvasRect;  // RectTransform của Canvas (Screen Space - Overlay)
    [SerializeField] private OffscreenIndicator indicatorPrefab;

    [Header("UI")]
    [SerializeField] private float edgePadding = 32f;   // khoảng cách cách mép màn hình (px)

    private readonly Dictionary<Transform, OffscreenIndicator> _indicators = new();

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!canvasRect) canvasRect = GetComponent<RectTransform>();
    }

    public void RegisterTarget(EnemyAI target)
    {
        if (!target || _indicators.ContainsKey(target.transform)) return;
        var ind = Instantiate(indicatorPrefab, canvasRect);
        ind.Init(cam, canvasRect, edgePadding, target.points, target.enemySkin.material.color);
        target.OnUpgarde += ind.SetPoint;
        ind.SetTarget(target.transform);
        _indicators.Add(target.transform, ind);
    }

    public void UnregisterTarget(EnemyAI target)
    {
        if (!target) return;
        if (_indicators.TryGetValue(target.transform, out var ind))
        {
            target.OnUpgarde -= ind.SetPoint; // hủy đăng ký sự kiện
            Destroy(ind.gameObject); // hoặc trả về pool
            _indicators.Remove(target.transform);
        }
    }

    void LateUpdate()
    {
        foreach (var kv in _indicators)
        {
            var target = kv.Key;
            var ind = kv.Value;

            if (!target || !target.gameObject.activeInHierarchy)
            {
                ind.SetVisible(false);
                continue;
            }

            ind.UpdateIndicator(); // tự tính on/off + vị trí + xoay
        }
    }
}