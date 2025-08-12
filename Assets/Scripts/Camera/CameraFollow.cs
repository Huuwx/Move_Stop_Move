using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;         // Player cần follow
    
    [SerializeField] private Vector3 offsetGameplay = new Vector3(0, 60, -17); // Độ cao + lùi ra sau
    [SerializeField] private Vector3 offsetWaitMenu = new Vector3(0, 60, -7); // Độ cao + lùi ra sau khi ở menu
    [SerializeField] private float smoothSpeed = 5f;   // Độ mượt khi follow
    [SerializeField] private float lookDownAngleGameplay = 45f; // Góc nhìn xuống (nếu muốn fix góc nhìn)
    [SerializeField] private float lookDownAngleWaitMenu = 30f; // Góc nhìn xuống (nếu muốn fix góc nhìn)


    private void OnEnable()
    {
        EventObserver.OnUpgrade += UpgradeOffset;
    }

    private void OnDisable()
    {
        EventObserver.OnUpgrade -= UpgradeOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition;
        if (GameController.Instance.State == GameState.Playing)
        {
            // Vị trí camera mong muốn
            desiredPosition = new Vector3(target.position.x, 0, target.position.z) + offsetGameplay;
            // Smooth di chuyển
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            // Xoay góc nhìn xuống player (nếu cần)
            transform.rotation = Quaternion.Euler(lookDownAngleGameplay, 0f, 0);
        }
        else if (GameController.Instance.State == GameState.Home)
        {
            // Vị trí camera mong muốn khi ở menu
            desiredPosition = new Vector3(target.position.x, 0, target.position.z) + offsetWaitMenu;
            // Smooth di chuyển
            transform.position = desiredPosition;
            // Xoay góc nhìn xuống player (nếu cần)
            transform.rotation = Quaternion.Euler(lookDownAngleWaitMenu, 0f, 0);
        }
    }
    
    public void UpgradeOffset()
    {
        // Cập nhật offset khi nâng cấp
        if (GameController.Instance.State == GameState.Playing)
        {
            Vector3 desiredPosition = offsetGameplay += new Vector3(0, 2f, -1f);
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, 5 * Time.deltaTime);
        }
    }
}