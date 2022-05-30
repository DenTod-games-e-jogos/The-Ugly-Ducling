using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour 
{
    float Speed = 3.0f;

    float RotateSpeed = 3.0f;

    CharacterController Controller;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        transform.Rotate(0, Input.GetAxis("Horizontal") * RotateSpeed, 0);

        Vector3 forward = transform.TransformDirection(Vector3.forward);

        float CurSpeed = Speed * Input.GetAxis("Vertical");
        
        Controller.SimpleMove(forward * curSpeed);
    }
}