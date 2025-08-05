using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;         // Player cần follow
    [SerializeField] private Vector3 offset = new Vector3(0, 30, -7); // Độ cao + lùi ra sau
    [SerializeField] private float smoothSpeed = 5f;   // Độ mượt khi follow
    [SerializeField] private float lookDownAngle = 45f; // Góc nhìn xuống (nếu muốn fix góc nhìn)

    void LateUpdate()
    {
        if (target == null) return;

        // Vị trí camera mong muốn
        Vector3 desiredPosition = new Vector3(target.position.x, 0, target.position.z) + offset;
        // Smooth di chuyển
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Xoay góc nhìn xuống player (nếu cần)
        transform.rotation = Quaternion.Euler(lookDownAngle, 0, 0);
    }
}