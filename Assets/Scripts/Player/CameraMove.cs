using UnityEngine;

public class CameraMove : MonoBehaviour
{
    const float YMin = -50.0f;
    
    const float YMax = 50.0f;

    public Transform lookAt;

    public Transform Player;

    public float distance = 10.0f;

    float currentX = 0.0f;

    float currentY = 0.0f;

    public float Sensivity = 4.0f;

    void LateUpdate()
    {
        currentX += Input.GetAxis("Mouse X") * Sensivity * Time.deltaTime;

        currentY += Input.GetAxis("Mouse Y") * Sensivity * Time.deltaTime;

        currentY = Mathf.Clamp(currentY, YMin, YMax);

        Vector3 Direction = new Vector3(0, 0, -distance);
        
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        
        transform.position = lookAt.position + rotation * Direction;

        transform.LookAt(lookAt.position);
    }
}