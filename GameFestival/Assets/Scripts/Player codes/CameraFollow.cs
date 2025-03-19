using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    public Transform target;
    // Move the camera relative to the target 
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    // Smooth movement coefficient of the moving camera
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        
        Vector3 desiredPosition = target.position + offset;
        
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
