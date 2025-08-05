using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joystickBG;     // Joystick nền
    public RectTransform joystickHandle; // Joystick tay cầm
    public float joystickRadius = 60f;   // Bán kính tối đa joystick (pixel)
    [HideInInspector] public Vector2 inputDir; // Hướng đầu ra (-1..1)

    private void Start()
    {
        if (joystickBG == null) joystickBG = GetComponent<RectTransform>();
        ResetHandle();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData); // Khi nhấn vào joystick, tính luôn vị trí kéo
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBG, eventData.position, eventData.pressEventCamera, out pos);

        Vector2 dir = pos;
        if (dir.magnitude > joystickRadius)
            dir = dir.normalized * joystickRadius;

        joystickHandle.anchoredPosition = dir;
        inputDir = dir / joystickRadius; // (-1..1)
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetHandle();
    }

    void ResetHandle()
    {
        joystickHandle.anchoredPosition = Vector2.zero;
        inputDir = Vector2.zero;
    }
}