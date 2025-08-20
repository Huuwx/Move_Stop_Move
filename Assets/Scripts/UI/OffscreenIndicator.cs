using System;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicator : MonoBehaviour
{
    [SerializeField] private Image arrowImage;          // ảnh mũi tên
    [SerializeField] private Image backgroundImage;    // nền (tuỳ chọn, có thể để trống nếu không cần)
    [SerializeField] private TMPro.TextMeshProUGUI pointText; // tùy chọn

    private Camera cam;
    private RectTransform canvasRect;
    private RectTransform rect;         // RectTransform của indicator
    private Transform target;
    private float padding;

    // cache screen center
    private Vector2 ScreenCenter => new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

    public void Init(Camera cam, RectTransform canvasRect, float padding, int point, Color color)
    {
        this.cam = cam;
        this.canvasRect = canvasRect;
        this.padding = padding;
        pointText.text = point.ToString();
        backgroundImage.color = color;
        arrowImage.color = color;
        rect = (RectTransform)transform;
        SetVisible(false);
    }

    public void SetTarget(Transform t) => target = t;

    public void SetVisible(bool v)
    {
        if (arrowImage) arrowImage.enabled = v;
        if (backgroundImage) backgroundImage.enabled = v;
        if (pointText) pointText.enabled = v && pointText;
    }
    
    public void SetPoint()
    {
        if (pointText)
        {
            int point = Int32.Parse(pointText.text) + 1;
            pointText.text = point.ToString();
        }
    }

    public void UpdateIndicator()
    {
        if (!target || !cam || GameController.Instance.State != GameState.Playing) { SetVisible(false); return; }

        Vector3 vp = cam.WorldToViewportPoint(target.position);

        // Nếu mục tiêu ở sau camera, “lật” sang phía trước để tính hướng đúng
        if (vp.z < 0f)
        {
            vp.x = 1f - vp.x;
            vp.y = 1f - vp.y;
            vp.z = 0.01f;
        }

        bool onScreen = vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f && vp.z > 0f;
        if (onScreen)
        {
            // Ẩn khi mục tiêu ở trong khung hình
            SetVisible(false);
            return;
        }

        // ► Off-screen: đặt indicator ở mép
        SetVisible(true);

        // Chuyển sang toạ độ màn hình
        Vector2 screenPos = new Vector2(vp.x * Screen.width, vp.y * Screen.height);

        // Vector từ tâm tới vị trí chiếu
        Vector2 fromCenter = screenPos - ScreenCenter;

        // Giới hạn vào hình chữ nhật (màn hình trừ padding)
        float halfW = Screen.width  * 0.5f - padding;
        float halfH = Screen.height * 0.5f - padding;

        // tỷ lệ cần để “đụng mép”
        float t = Mathf.Max(Mathf.Abs(fromCenter.x) / halfW, Mathf.Abs(fromCenter.y) / halfH);
        if (t < 1f) t = 1f; // đảm bảo nằm ngoài trước đã

        Vector2 edgePos = ScreenCenter + fromCenter / t;

        // Đưa toạ độ màn hình → toạ độ Canvas (Screen Space Overlay dùng 1:1, nhưng ta vẫn quy đổi an toàn)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, edgePos, null, out Vector2 canvasPos);
        rect.anchoredPosition = canvasPos;

        // Xoay mũi tên hướng từ tâm → vị trí mép
        float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
        arrowImage.transform.rotation = Quaternion.Euler(0, 0, angle -90f); // mũi tên sprite hướng lên trên? -90 cho đúng chiều
        if(angle > 90f || angle < -90f)
        {
            arrowImage.transform.localPosition = new Vector3(-70f, 0, 0);
        }
        else
        {
            arrowImage.transform.localPosition = new Vector3(70f, 0, 0);
        }
    }
}
