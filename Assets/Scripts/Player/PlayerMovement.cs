using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
    CharacterController Controller;

    float Speed = 3.0f;

    public Transform Cam;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float Horizontal = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;

        float Vertical = Input.GetAxis("Vertical") * Speed * Time.deltaTime;

        Vector3 Movement = Cam.transform.right * Horizontal + Cam.transform.forward * Vertical;

        Movement.y = 0f;

        Controller.Move(Movement);

        if (Movement.magnitude != 0f)
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") 
            * Cam.GetComponent<CameraMove>().Sensivity * Time.deltaTime);

            Quaternion CamRotation = Cam.rotation;

            CamRotation.x = 0f;
            
            CamRotation.z = 0f;
            
            transform.rotation = Quaternion.Lerp(transform.rotation, CamRotation, 0.1f);
        }
    }
}