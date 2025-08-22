using UnityEngine;

public class LoadingRotate : MonoBehaviour
{
    public float speed = 500f;
    
    private void Update()
    {
        // Rotate the object around its local Y axis at the specified speed
        transform.Rotate(Vector3.back, speed * Time.deltaTime);
    }
}
