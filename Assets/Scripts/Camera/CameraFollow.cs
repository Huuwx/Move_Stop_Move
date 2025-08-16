using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector3 offsetGameplay = new Vector3(0, 60, -17);
    [SerializeField] private Vector3 offsetWaitMenu = new Vector3(0, 60, -7);
    [SerializeField] private Vector3 offsetSkinShop = new Vector3(0, 44, -10);
    [SerializeField] private float smoothSpeed = 25f;
    [SerializeField] private float lookDownAngleGameplay = 45f;
    [SerializeField] private float lookDownAngleWaitMenu = 30f;
    [SerializeField] private float lookDownAngleSkinShop = 33f;

    // ====== Occlusion Fade (ẩn tòa nhà khi che player) ======
    [Header("Occlusion Fade")]
    [SerializeField] private LayerMask obstructionMask;    // chỉ layer tòa nhà
    [SerializeField] private float sphereRadius = 0.35f;   // bán kính "player"
    [SerializeField] private float checkInterval = 0.1f;   // tần suất kiểm tra
    [SerializeField, Range(0f, 1f)] private float fadeTo = 0.25f; // alpha khi mờ
    [SerializeField] private float fadeSpeed = 8f;         // tốc độ mờ/hiện

    private float _nextCheckTime;
    private readonly Dictionary<Renderer, float> _currentAlpha = new(); // alpha hiện tại
    private readonly Dictionary<Renderer, float> _targetAlpha  = new(); // alpha mục tiêu
    private readonly List<Renderer> _hitsThisFrame = new();             // renderer đang che
    private MaterialPropertyBlock _mpb;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        if (obstructionMask.value == 0)
            obstructionMask = LayerMask.GetMask("Obstruction"); // fallback
    }

    private void OnEnable()
    {
        EventObserver.OnUpgrade += UpgradeOffset;
    }

    private void OnDisable()
    {
        EventObserver.OnUpgrade -= UpgradeOffset;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // ====== Follow / góc nhìn ======
        Vector3 desiredPosition;
        if (GameController.Instance.State == GameState.Playing)
        {
            desiredPosition = new Vector3(target.position.x, 0f, target.position.z) + offsetGameplay;
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(lookDownAngleGameplay, 0f, 0f);
        }
        else if (GameController.Instance.State == GameState.Home)
        {
            desiredPosition = new Vector3(target.position.x, 0f, target.position.z) + offsetWaitMenu;
            transform.position = desiredPosition;
            transform.rotation = Quaternion.Euler(lookDownAngleWaitMenu, 0f, 0f);
        }
        else if (GameController.Instance.State == GameState.Shop)
        {
            desiredPosition = new Vector3(target.position.x, 0f, target.position.z) + offsetSkinShop;
            transform.position = desiredPosition;
            transform.rotation = Quaternion.Euler(lookDownAngleSkinShop, 0f, 0f);
        }

        // ====== Occlusion cập nhật ======
        if (Time.time >= _nextCheckTime)
        {
            DoOcclusionCheck();
            _nextCheckTime = Time.time + checkInterval;
        }
        
        // cập nhật alpha mượt mỗi frame
        UpdateAlphas();
    }

    public void UpgradeOffset()
    {
        if (GameController.Instance.State == GameState.Playing && GameController.Instance.mode == GameMode.Normal)
        {
            offsetGameplay += new Vector3(0f, 2f, -1f);
            Vector3 desiredPosition = new Vector3(target.position.x, 0f, target.position.z) + offsetGameplay;
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, 5f * Time.deltaTime);
        }
    }

    // --------- OCCLUSION LOGIC ---------

    private void DoOcclusionCheck()
    {
        _hitsThisFrame.Clear();

        Vector3 camPos = transform.position;
        Vector3 dir = target.position - camPos;
        float dist = dir.magnitude;
        if (dist < 0.01f) return;
        dir /= dist;

        var hits = Physics.SphereCastAll(
            camPos, sphereRadius, dir, dist,
            obstructionMask, QueryTriggerInteraction.Ignore
        );

        // Những renderer đang che → đặt alpha mục tiêu = fadeTo
        foreach (var h in hits)
        {
            foreach (var r in h.collider.GetComponentsInChildren<Renderer>())
            {
                _hitsThisFrame.Add(r);
                _targetAlpha[r] = fadeTo;
                if (!_currentAlpha.ContainsKey(r)) _currentAlpha[r] = 1f;
            }
        }

        // Không còn che → đặt alpha mục tiêu = 1
        foreach (var r in _currentAlpha.Keys)
        {
            if (!_hitsThisFrame.Contains(r))
                _targetAlpha[r] = 1f;
        }
    }

    private void UpdateAlphas()
    {
        // cập nhật alpha tiến dần tới target
        var keys = new List<Renderer>(_targetAlpha.Keys);
        foreach (var r in keys)
        {
            float cur = _currentAlpha.TryGetValue(r, out var a) ? a : 1f;
            float next = Mathf.MoveTowards(cur, _targetAlpha[r], fadeSpeed * Time.deltaTime);
            _currentAlpha[r] = next;

            ApplyAlpha(r, next);

            // dọn sạch nếu đã về 1 và không còn trong danh sách bị che
            if (Mathf.Approximately(next, 1f) && !_hitsThisFrame.Contains(r))
            {
                _targetAlpha.Remove(r);
            }
        }
    }

    private void ApplyAlpha(Renderer r, float a)
    {
        // Yêu cầu vật liệu của tòa nhà là URP/Lit (Surface=Transparent)
        r.GetPropertyBlock(_mpb);
        Color baseCol = r.sharedMaterial.HasProperty(BaseColor)
            ? r.sharedMaterial.GetColor(BaseColor)
            : Color.white;
        baseCol.a = a;
        _mpb.SetColor(BaseColor, baseCol);
        r.SetPropertyBlock(_mpb);
    }
}
