using UnityEngine;

public class WorldSpaceBillboardUI : MonoBehaviour
{
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        // KHÔNG xoay theo nhân vật: luôn đối mặt camera (khử roll)
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position, cam.transform.up);
        // Hoặc đơn giản: transform.rotation = cam.transform.rotation;
    }
}